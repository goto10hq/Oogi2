using Newtonsoft.Json;
using Oogi2.Entities;
using Oogi2.Tokens;
using System.Collections.Generic;
using static Oogi2.Tests.Helpers.Enums;

namespace Oogi2.Tests.Helpers
{
    public class FakeRobot : BaseEntity
    {             
        public string Name { get; set; }
        public int ArtificialIq { get; set; }
        public Stamp Created { get; set; } = new Stamp();
        public bool IsOperational { get; set; }
        public IEnumerable<string> Parts { get; set; } = new List<string>();
        public string Message { get; set; }
        public State State { get; set; }
        
        public override string PartitionKey => "fakeoogi2";

        public override string Entity => Entities.FakeRobot;

        public FakeRobot()
        {
        }

        public FakeRobot(string name, int artificialIq, bool isOperational, List<string> parts, State state)
        {
            Name = name;
            ArtificialIq = artificialIq;
            IsOperational = isOperational;
            Parts = parts;
            State = state;
        }
    }
}