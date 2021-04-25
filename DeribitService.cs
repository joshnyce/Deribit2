using Deribit.Interfaces;
using Deribit.Types;
using Dispatch;
using MoreLinq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using Serilog;
using System;
using System.Linq;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Security.Authentication;
using WebSocketSharp;

namespace Deribit
{

    //this could be a singleton, depending on use case

    public class DeribitService : IDeribitService
    {
        //these inputs should be configurable
        private const string urlGetInstruments = "https://test.deribit.com/api/v2/public/get_instruments";
        private const string urlSocket = "wss://test.deribit.com/ws/api/v2";

        private readonly string[] currencies;
        private readonly (string outter, string inner)[] paths; //should allow for variable depth (number of tokens)
        private readonly Program.Writer writers;

        private readonly ConcurrentDictionary<string, SerialQueue> eventLoops = new();
        private readonly WebSocket ws;

        internal DeribitService(string[] currencies, (string outter, string inner)[] paths, Program.Writer writers)
        {
            this.currencies = currencies;
            this.paths = paths;
            this.writers = writers;

            ws = new WebSocket(urlSocket);
            ws.SslConfiguration.EnabledSslProtocols = SslProtocols.Tls12;
            //add separate sockets/callbacks for other MarketEvent types, or determine message type from the ID (but some messages don't include IDs)
            ws.OnMessage += (sender, e) => { if (e.IsText) HandleMessage<Trade>(e.Data); }; 
            ws.OnError += (sender, e) => { Log.Error(e.Message); };
            ws.Connect();

            foreach (var ccy in this.currencies)
            {
                //i'm not sure these are correct yet - need to spend more time exploring the API
                ws.Send(GenerateMessage(
                    0, 
                    "public/get_last_trades_by_instrument", //have to change this for quotes
                    new { instrument_name = $"{ccy}-PERPETUAL" } )); //this may filter out a lot of data - need better understanding of the API
                ws.Send(GenerateMessage(
                    1,
                    "public/subscribe",
                    new { channels = GetInstruments(ccy, false, "future") } )); //hard-coded to (or filtered on) futures for now
            }
        }

        private static string GenerateMessage(int ID, string Method, dynamic Parameters)
        {
            return JsonConvert.SerializeObject(new {
                jsonrpc = "2.0",
                id = ID,
                method = Method,
                @params = Parameters });
        }

        public string[] GetInstruments(string Ccy, bool Expired, string Kind)
        {
            var parameters = new Dictionary<string, string>() {
                ["currency"] = Ccy,
                ["expired"] = Expired.ToString().ToLower(),
                ["kind"] = Kind };
            var client = new RestClient(urlGetInstruments); //could use websocket for this too
            var request = new RestRequest(Method.GET);
            request.AddHeader("Content-Type", "application/json");
            parameters.ForEach(x => request.AddParameter(x.Key, x.Value));
            return JObject.Parse(client.Execute(request).Content)
                .SelectToken("result")
                ?.Select(x => $"trades.{x.SelectToken("instrument_name")}.raw")
                .ToArray();
        }

        public void HandleMessage<T>(string Data) where T : MarketEvent
        {
            //don't need to loop through (JSON) paths
            //after adding more security types and associated paths, should determine correct path as function of T
            //but initial messages are also different from streaming messages, so would need to determine message type another way
            var obj = JObject.Parse(Data);
            paths.ForEach(p => obj
                .SelectToken(p.outter)
                ?.SelectToken(p.inner)
                ?.OrderBy(x => (string)x["timestamp"])
                .ThenBy(x => (string)x["trade_seq"])
                .ForEach(x => ProcessMarketEvent<T>(x)));
        }

        public void ProcessMarketEvent<T>(JToken Token) where T : MarketEvent
        {
            var data = Token.ToObject<T>();
            if (!eventLoops.ContainsKey(data.Key()))
                eventLoops[data.Key()] = new SerialQueue();
            eventLoops[data.Key()].DispatchAsync(() => { writers(data); });
        }

        public void Dispose()
        {
            //need to unsubscribe first?
            ws.Close();
            GC.SuppressFinalize(this);
        }
    }
}