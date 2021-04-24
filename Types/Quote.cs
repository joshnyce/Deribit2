using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Deribit.Types
{
    public class Quote : MarketEvent
    {
        public Quote(JToken data) : base(data) { }

        //properties are JSON-serializable for other destinations (which are not implemented yet)
        //using strings because data is serialized, but that might change when adding other destinations

        [JsonProperty("instrument_name")]
        public string instrument_name { get; private set; }

        [JsonProperty("best_bid_price")]
        public string best_bid_price { get; private set; }

        [JsonProperty("best_bid_amount")]
        public string best_bid_amount { get; private set; }

        [JsonProperty("best_ask_price")]
        public string best_ask_price { get; private set; }

        [JsonProperty("best_ask_amount")]
        public string best_ask_amount { get; private set; }

        [JsonProperty("timestamp")]
        public string timestamp { get; private set; }

        [JsonProperty("channel")]
        public string channel { get; private set; }
    }
}
