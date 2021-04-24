using Deribit.Types;
using Newtonsoft.Json.Linq;

namespace Deribit.Interfaces
{

    //haven't used this yet but it can be used for dependency injection

    public interface IDeribitService
    {
        string[] GetInstruments(string Ccy, bool Expired, string Kind);
        public void HandleMessage<T>(string Data) where T : MarketEvent;
        public void ProcessMarketEvent<T>(JToken Token) where T : MarketEvent;
    }
}
