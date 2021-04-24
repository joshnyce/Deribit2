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
        public static Writer[] writers = new[] { (Writer)new TextFile().ProcessMessage }; //this is ugly


        private static void Main()
        {
            Log.Logger = new LoggerConfiguration()
               .WriteTo.Console()
               .CreateLogger();
            try
            {
                var service = new DeribitService(currencies, paths, writers);                
                Thread.CurrentThread.Join();
            }
            catch (Exception e) { Log.Error(e.Message); }
        }
    }
} 