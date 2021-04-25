using Deribit.Types;
using System;

namespace Deribit.Interfaces
{

    //interface for sending data to each destination
    //add more writers for KDB, a message queue, and/or a grid (re-use existing code)

    public interface IWriter : IDisposable
    {
        public void ProcessMessage(MarketEvent Data);
        public new void Dispose(); //this is mainly for destinations that haven't been implemented yet
    }
}
