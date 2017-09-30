using System.IO;
using Microsoft.Extensions.Configuration;


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
            con.DeleteCollection();

            var repo = new Repository<dynamic>(con);
        }
    }
}
