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
            con.CreateCollection();


            var repo = new Repository<dynamic>(con);


            
        }
    }
}
