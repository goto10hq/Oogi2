using System.IO;
using Microsoft.Extensions.Configuration;
using Oogi2.Queries;
using System.Linq;
using System;

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
            
            var con = new Connection(appSettings["endpoint"], appSettings["authorizationKey"], appSettings["database"], appSettings["collection"]);            
            var repo = new Repository<dynamic>(con);
            var start = DateTime.Now;
            var results = repo.GetList(new DynamicQuery("select top 80000 * from c"));

            System.Console.WriteLine(results.Count);
            System.Console.WriteLine((DateTime.Now - start).TotalSeconds);
            System.Console.WriteLine(results.First());
            
        }
    }
}
