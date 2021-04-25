using Deribit.Interfaces;
using Deribit.Types;
using Serilog;
using System;

namespace Deribit.Writers
{
    public class Logger : IWriter
    {
        public void ProcessMessage(MarketEvent Data)
        {
            Log.Information(Data.TextSerialize(false));
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
