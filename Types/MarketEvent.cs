using Newtonsoft.Json;
using System;
using System.Linq;

namespace Deribit.Types
{
    public abstract class MarketEvent
    {
        //may need to get instrument name (for the Key method) another way if any messages don't contain this field
        //this is abstract so the order can be controlled for text file output
        [JsonProperty("instrument_name")]
        public abstract string instrument_name { get; set; }

        public string Key()
        {
            return $"{GetType().Name}_{instrument_name}";
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
