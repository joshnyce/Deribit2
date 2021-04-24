using Deribit.Types;

namespace Deribit.Interfaces
{
    public interface IWriter
    {
        //use interfaces for code that sends data to each destination, so they can be mocked for testing
        //add more writers for KDB, a message queue, and/or a grid
        public void ProcessMessage(MarketEvent Data);
    }
}
