using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Deribit.Types
{

    //properties are JSON-serializable for other destinations (which are not implemented yet)
    //using strings because data is serialized, but that might change when adding other destinations

    public class Trade : MarketEvent
    {
        public Trade(JToken data) : base(data) { }

        [JsonProperty("trade_seq")]
        public string trade_seq { get; private set; }

        [JsonProperty("trade_id")]
        public string trade_id { get; private set; }

        [JsonProperty("timestamp")]
        public string timestamp { get; private set; }

        [JsonProperty("tick_direction")]
        public string tick_direction { get; private set; }

        [JsonProperty("mark_price")]
        public string mark_price { get; private set; }

        [JsonProperty("price")]
        public string price { get; private set; }

        [JsonProperty("instrument_name")]
        public string instrument_name { get; private set; }

        [JsonProperty("index_price")]
        public string index_price { get; private set; }

        [JsonProperty("direction")]
        public string direction { get; private set; }

        [JsonProperty("amount")]
        public string amount { get; private set; }
    }
}
