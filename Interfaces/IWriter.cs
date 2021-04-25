using Deribit.Types;

namespace Deribit.Interfaces
{

    //interface for sending data to each destination
    //add more writers for KDB, a message queue, and/or a grid (re-use existing code)

    public interface IWriter
    {
        public void ProcessMessage(MarketEvent Data);
    }
}
