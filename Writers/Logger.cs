using Deribit.Interfaces;
using Deribit.Types;
using Serilog;

namespace Deribit.Writers
{

    //add option to batch lines (since the text files probably won't be consumed in real-time)

    public class Logger : IWriter
    {
        public void ProcessMessage(MarketEvent Data)
        {
            Log.Information(Data.TextSerialize(false));
        }
    }
}
