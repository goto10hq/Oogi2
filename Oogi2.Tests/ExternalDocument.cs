using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Oogi2;
using Microsoft.Extensions.Configuration;

namespace Tests
{
    // SKIP THIS ONE
    //[TestClass]
    //public class ExternalDocument
    //{        
    //    static Connection _con;
        
    //    [TestInitialize]
    //    public void CreateRobots()
    //    {
    //        var appSettings = new ConfigurationBuilder()
    //            .SetBasePath(Directory.GetCurrentDirectory())
    //            .AddJsonFile("appsettings.json")
    //            .AddUserSecrets("oogi2")
    //            .AddEnvironmentVariables()
    //            .Build();

    //        _con = new Connection(appSettings["endpoint"], appSettings["authorizationKey"], appSettings["database"], appSettings["collection"]);                        
    //    }

    //    [TestMethod]
    //    public void UpsertExternalDocument()
    //    {
    //        var file = File.ReadAllText("document.json");            
    //        _con.UpsertJson(file);

    //        var repo = new Repository<dynamic>(_con);
    //        var dummy = repo.GetFirstOrDefault("select top 1 * from c where c['this-is-oogi-2'] = true and c.name = 'frohikey'", null);

    //        Assert.AreNotEqual(null, dummy);
    //        Assert.AreEqual("umiko", dummy.aoba);
    //        Assert.AreNotEqual(null, dummy.id);

    //        repo.Delete(dummy.id);
    //        // TODO: this one fails -> check it out
    //        //repo.Delete(dummy);

    //        dummy = repo.GetFirstOrDefault("select top 1 * from c where c[\"this-is-oogi-2\"] = true and c.name = 'frohikey'", null);

    //        Assert.AreEqual(null, dummy);
    //    }
    //}
}
