using MoreLinq.Extensions;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;

namespace Deribit.Types
{
    public abstract class MarketEvent
    {
        private readonly string symbol;
        private readonly Type type;

        private readonly static ConcurrentDictionary<Type, PropertyInfo[]> props = new();

        public MarketEvent(JToken msg)
        {
            //would normally use bespoke classes to parse JSON, but this is faster for prototyping
            type = GetType();
            if (!props.ContainsKey(type))
                props[type] = type.GetProperties();
            props[type].ForEach(x => x.SetValue(this, (string)msg[x.Name]));
            symbol = (string)msg["instrument_name"];
        }

        public string Key()
        {
            return $"{type.Name}_{symbol}";
        }

        public string TextSerialize(bool includeHeader)
        {
            var s = includeHeader
                ? string.Join(",", props[type].Select(x => x.Name)) + Environment.NewLine
                : "";
            return $"{s}{string.Join(",", props[type].Select(x => x.GetValue(this)))}";
        }
    }
}
