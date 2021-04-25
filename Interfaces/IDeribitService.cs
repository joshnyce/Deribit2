using Deribit.Types;
using Newtonsoft.Json.Linq;
using System;

namespace Deribit.Interfaces
{

    //for dependency injection (to mock this service in other applications)

    public interface IDeribitService : IDisposable
    {
        string[] GetInstruments(string Ccy, bool Expired, string Kind);
        public void HandleMessage<T>(string Data) where T : MarketEvent;
        public void ProcessMarketEvent<T>(JToken Token) where T : MarketEvent;
        public new void Dispose();
    }
}
