using Newtonsoft.Json;
using Oogi2.Attributes;
using Oogi2.Tokens;
using System.Collections.Generic;
using static Oogi2.Tests.Helpers.Enums;

namespace Oogi2.Tests.Helpers
{
    [EntityType("entity", Entity)]
    public class Robot
    {
        public const string Entity = "oogi2/robot";
        public string Id { get; set; }

        public string Name { get; set; }
        public int ArtificialIq { get; set; }
        public Stamp Created { get; set; } = new Stamp();
        public bool IsOperational { get; set; }
        public IEnumerable<string> Parts { get; set; } = new List<string>();
        public string Message { get; set; }
        public State State { get; set; }

        [JsonProperty("_etag")]
        public string ETag { get; set; }

        public Robot()
        {
        }

        public Robot(string name, int artificialIq, bool isOperational, List<string> parts, State state)
        {
            Name = name;
            ArtificialIq = artificialIq;
            IsOperational = isOperational;
            Parts = parts;
            State = state;
        }
    }
}