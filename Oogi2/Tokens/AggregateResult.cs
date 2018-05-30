using Newtonsoft.Json;

namespace Oogi2.Tokens
{
    public class AggregateResult
    {
        [JsonProperty("$1")]
        public long Number { get; set; }
    }
}