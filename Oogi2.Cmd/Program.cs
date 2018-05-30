using System.IO;
using Microsoft.Extensions.Configuration;
using Oogi2.Attributes;
using Newtonsoft.Json;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

using Oogi2.Attributes;

namespace Oogi2.Cmd
{
    class Program
    {
        const string _entity = "oogi2/robot";

        [EntityType("entity", _entity)]
        public class Robot
        {
            public string Id { get; set; }
            public string Name { get; set; }

            public Robot()
            {
            }

            public Robot(string name)
            {
                Name = name;
            }
        }

        static void Main(string[] args)
        {
            var appSettings = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .AddUserSecrets("oogi2")
                .AddEnvironmentVariables()
                .Build();

            var _con = new Connection(appSettings["endpoint"], appSettings["authorizationKey"], appSettings["database"], appSettings["collection"]);
            _con.CreateCollection();

            var repo = new Repository(_con);
            repo.Create(new { Movie = "Donkey Kong Jr.", Rating = 3 });
            repo.Create(new { Movie = "King Kong", Rating = 2 });
            repo.Create(new { Movie = "Donkey Kong", Rating = 1 });

            var movies = repo.GetList("select * from c where c.rating <> null", null);

            repo.Delete(movies[0]);
            //var _robots = new List<Robot>
            //{
            //new Robot("Alfred"),
            //new Robot("Nausica"),
            //    new Robot("Kosuna")
            //};

            //var _repo = new Repository<Robot>(_con);

            //foreach (var robot in _robots)
            //    _repo.Create(robot);

            //var _robots2 = _repo.GetList("select * from c", null);

            //_repo.Delete(_robots[0]);

            _con.DeleteCollection();
        }
    }
}