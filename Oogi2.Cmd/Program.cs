using System.IO;
using Microsoft.Extensions.Configuration;
using Oogi2.Attributes;
using Newtonsoft.Json;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Oogi2.Cmd
{
    class Program
    {
        static void Main(string[] args)
        {
            var appSettings = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .AddUserSecrets("oogi2")
                .AddEnvironmentVariables()
                .Build();

            var _con = new Connection(appSettings["endpoint"], appSettings["authorizationKey"], appSettings["database"], "hub");

            var content = File.ReadAllText(@"d:\TEMP\db_hub_20171125.json");
            var documents = JsonConvert.DeserializeObject<List<JObject>>(content);
            var start = documents.Count;

            foreach (var d in documents)
            {
                _con.UpsertJson("[" + JsonConvert.SerializeObject(d) + "]");
                start--;
                System.Console.WriteLine(start);
            }
        }
    }
}