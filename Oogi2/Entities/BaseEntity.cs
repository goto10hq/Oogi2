using System;
using Newtonsoft.Json;
using Oogi2.Helpers;

namespace Oogi2.Entities
{
    public abstract class BaseEntity
    {
        public string Id { get; set; }
        public abstract string PartitionKey { get; }
        public abstract string Entity { get;  }

        [JsonProperty("_etag")]
        public string Etag { get; set; }
        
        [JsonConverter(typeof(UnixDateTimeConverter))]
        [JsonProperty("_ts")]
        public DateTime Timestamp { get; set; }
    }
}