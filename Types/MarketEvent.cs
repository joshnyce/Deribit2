using MoreLinq.Extensions;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;

namespace Deribit.Types
{
    public abstract class MarketEvent
    {
        private string symbol;

        public MarketEvent(JToken msg)
        {
            //would normally use bespoke classes to parse JSON, but this is faster for prototyping
            var props = GetType().GetProperties();
            props.ForEach(x => x.SetValue(this, (string)msg[x.Name]));
            symbol = (string)msg["instrument_name"];
        }

        public string Key()
        {
            return $"{GetType().Name}_{symbol}";
        }

        public string TextSerialize(bool includeHeader)
        {
            var props = GetType().GetProperties();
            var s = includeHeader
                ? string.Join(",", props.Select(x => x.Name)) + Environment.NewLine
                : "";
            return $"{s}{string.Join(",", props.Select(x => x.GetValue(this)))}";
        }
    }
}
