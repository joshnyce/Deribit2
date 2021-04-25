using Deribit.Types;
using Deribit.Writers;
using Serilog;
using System;
using System.Threading;

namespace Deribit

{
    class Program
    {
        //all inputs should be configurable or derived automatically

        private static readonly string[] currencies = new[] {
            "BTC",
            "ETH" };

        //these paths have only been tested for trade messages (didn't finish implementing quotes yet)
        private static readonly (string outter, string inner)[] paths = new[] {
            (outter: "params", inner: "data"),
            (outter: "result", inner: "trades") };

        public delegate void Writer(MarketEvent Data);


        private static void Main()
        {
            Log.Logger = new LoggerConfiguration()
               .WriteTo.Console()
               .CreateLogger();
            Writer writers = new Logger().ProcessMessage;
            writers += new TextFile().ProcessMessage; //multicast delegate supports multiple writers
            //could create instances of IWriters dynamically using Activator.CreateInstance(), so the list of destinations/processors could be configured externally
            try
            {
                var service = new DeribitService(currencies, paths, writers);                
                Thread.CurrentThread.Join();
            }
            catch (Exception e) { Log.Error(e.Message); }
        }
    }
} 