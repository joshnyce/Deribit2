using Newtonsoft.Json;
using System;
using System.Linq;

namespace Deribit.Types
{
    public abstract class MarketEvent
    {
        //may need to get instrument name (for the Key method) another way if any messages don't contain this field
        //also can't control the order for the output (this property is last) for text files, but text file output is usually not important
        [JsonProperty("instrument_name")]
        public string instrument_name { get; private set; }

        public string Key()
        {
            return $"{this.GetType().Name}_{instrument_name}";
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
