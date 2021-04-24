using Deribit.Types;

namespace Deribit.Interfaces
{
    public interface IDeribitService
    {
        //haven't used this yet but it can be used for dependency injection
        string[] GetInstruments(string Ccy, bool Expired, string Kind);
        public void HandleMessage(string Data);
        public void ProcessMarketEvent(MarketEvent Data);
    }
}
