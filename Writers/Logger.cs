using Deribit.Interfaces;
using Deribit.Types;
using Serilog;

namespace Deribit.Writers
{
    public class Logger : IWriter
    {
        public void ProcessMessage(MarketEvent Data)
        {
            Log.Information(Data.TextSerialize(false));
        }
    }
}
