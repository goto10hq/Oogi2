using Newtonsoft.Json;

namespace Oogi2.Tokens
{
    class AggregateResult
    {
        [JsonProperty("$1")]
        public long? Number { get; set; }
    }
}