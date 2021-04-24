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
    public class DeribitService : IDeribitService, IDisposable
    {
        //todo:
        //use a windows service or need keep-alive system
        //listen to more events and log all activity (like subscriptions starting and stopping)
        //listen to heartbeat and add timer to try reconnecting until successful
        //use separate socket connections for each subscription
        //process quotes (in addition to trades) and any other types of events that are of interest
        //public method to ubsubscribe
        //way to request replays from the API in case of outages (looks like sequence number can be used to specify ranges of historical data)

        //these inputs should be configurable
        private const string urlGetInstruments = "https://test.deribit.com/api/v2/public/get_instruments";
        private const string urlSocket = "wss://test.deribit.com/ws/api/v2";

        private readonly string[] currencies;
        private readonly (string outter, string inner)[] paths; //should allow for variable depth (number of tokens)
        private readonly Program.Writer[] writers;

        private ConcurrentDictionary<string, SerialQueue> eventLoops = new();
        private WebSocket ws;

        internal DeribitService(string[] currencies, (string outter, string inner)[] paths, Program.Writer[] writers)
        {
            this.currencies = currencies;
            this.paths = paths;
            this.writers = writers;
            Init();
        }
         
        private void Init()
        { 
            ws = new WebSocket(urlSocket);
            ws.SslConfiguration.EnabledSslProtocols = SslProtocols.Tls12;
            ws.OnMessage += (sender, e) => { if (e.IsText) HandleMessage(e.Data); };
            ws.OnError += (sender, e) => { Log.Error(e.Message); };
            ws.Connect();

            foreach (var ccy in currencies)
            {
                ws.Send(JsonConvert.SerializeObject(new {
                    jsonrpc = "2.0",
                    id = 0,
                    method = "public/get_last_trades_by_instrument", //have to change this for quotes
                    @params = new { instrument_name = $"{ccy}-PERPETUAL" } }));
                ws.Send(JsonConvert.SerializeObject(new {
                    jsonrpc = "2.0",
                    id = 1,
                    method = "public/subscribe",
                    @params = new { channels = GetInstruments(Ccy: ccy, Expired: false, Kind: "future") } }));
            }
        }

        public string[] GetInstruments(string Ccy, bool Expired, string Kind)
        {
            var parameters = new Dictionary<string, string>() {
                ["currency"] = Ccy,
                ["expired"] = Expired.ToString().ToLower(),
                ["kind"] = Kind };
            var client = new RestClient(urlGetInstruments);
            var request = new RestRequest(Method.GET);
            request.AddHeader("Content-Type", "application/json");
            parameters.ForEach(x => request.AddParameter(x.Key, x.Value));
            return JObject.Parse(client.Execute(request).Content)
                .SelectToken("result")
                ?.Select(x => $"trades.{x.SelectToken("instrument_name")}.raw")
                .ToArray();
        }

        public void HandleMessage(string Data)
        {
            //if this remains generic, need to determine message type (from the ID or using separate callbacks and/or socket connections)
            var obj = JObject.Parse(Data);
            paths.ForEach(p => obj
                .SelectToken(p.outter)
                ?.SelectToken(p.inner)
                ?.OrderBy(x => (string)x["timestamp"])
                .ThenBy(x => (string)x["trade_seq"])
                .ForEach(x => ProcessMarketEvent(new Trade(x))) );
        }

        public void ProcessMarketEvent(MarketEvent Data)
        {
            if (!eventLoops.ContainsKey(Data.Key()))
                eventLoops[Data.Key()] = new SerialQueue();
            eventLoops[Data.Key()].DispatchAsync(() => { writers.ForEach(x => x(Data)); });
        }

        public void Dispose()
        {
            //need to unsubscribe first?
            ws.Close();
        }
    }
}