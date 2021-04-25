using Newtonsoft.Json;

namespace Deribit.Types
{

    //using strings because data is serialized, but that might change when adding other destinations

    public class Quote : MarketEvent
    {
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
