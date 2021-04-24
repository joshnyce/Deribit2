using Deribit.Interfaces;
using Deribit.Types;
using System;
using System.IO;

namespace Deribit.Writers
{
    public class TextFile : IWriter
    {
        public void ProcessMessage(MarketEvent Data)
        {
            var path = Path.Combine(Path.GetTempPath(), $"{Data.Key()}_{DateTime.Today:yyyyMMdd}.csv"); //start new file at midnight
            using var writer = new StreamWriter(path, true);
            writer.WriteLine(Data.TextSerialize(new FileInfo(path).Length == 0));
        }
    }
}
