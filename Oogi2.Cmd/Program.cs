using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Oogi2.Attributes;
using Oogi2.Tokens;

namespace Oogi2.Cmd
{
    class Program
    {
        static Repository<Robot> _repo;
        static Connection _con;
        const string _entity = "oogi2/robot";

        public enum State
        {
            Ready = 10,
            Sleeping = 20,
            Destroyed = 30
        }

        [EntityType("entity", _entity)]
        public class Robot
        {
            public string Id { get; set; }

            public string Name { get; set; }
            public int ArtificialIq { get; set; }
            public Stamp Created { get; set; } = new Stamp();
            public bool IsOperational { get; set; }
            public List<string> Parts { get; set; } = new List<string>();
            public string Message { get; set; }

            private State state;

            public State GetState()
            {
                return state;
            }

            public void SetState(State value)
            {
                state = value;
            }

            public Robot()
            {
            }

            public Robot(string name, int artificialIq, bool isOperational, List<string> parts, State state)
            {
                Name = name;
                ArtificialIq = artificialIq;
                IsOperational = isOperational;
                Parts = parts;
                SetState(state);
            }
        }

        static List<Robot> _robots = new List<Robot>
            {
            new Robot("Alfred", 100, true, new List<string> { "CPU", "Laser" }, State.Ready),
            new Robot("Nausica", 220, true, new List<string> { "CPU", "Bio scanner", "DSP" }, State.Sleeping),
            new Robot("Kosuna", 190, false, new List<string>(), State.Ready) { Message = @"\'\\''" }
            };

        static void Main(string[] args)
        {
            var appSettings = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .AddUserSecrets("oogi2")
                .AddEnvironmentVariables()
                .Build();
            
            _con = new Connection(appSettings["endpoint"], appSettings["authorizationKey"], appSettings["database"], appSettings["collection"]);

            _repo = new Repository<Robot>(_con);

            foreach (var robot in _robots.Take(_robots.Count - 1))
                _repo.Create(robot);

            foreach (var robot in _robots.TakeLast(1))
                _repo.Upsert(robot);
        }
    }
}
