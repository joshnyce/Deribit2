using Deribit.Types;

namespace Deribit.Interfaces
{

    //interface for sending data to each destination, for mocking/testing
    //add more writers for KDB, a message queue, and/or a grid
    //reuse existing code for writing to databases, message queues, and/or grids

    public interface IWriter
    {
        public void ProcessMessage(MarketEvent Data);
    }
}
