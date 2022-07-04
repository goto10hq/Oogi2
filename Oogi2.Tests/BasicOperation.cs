using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Oogi2;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Linq;
using Oogi2.Queries;
using Oogi2.Tests.Helpers;
using static Oogi2.Tests.Helpers.Enums;
using System;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Newtonsoft.Json.Linq;

namespace Tests
{
    [TestClass]
    public class BasicOperation
    {
        static Repository<Robot> _repo;
        static Repository<FakeRobot> _fakeRepo;
        static AggregateRepository _aggregate;
        static CommonRepository<dynamic> _dynamic;
        static Connection _con;

        readonly List<Robot> _robots = new List<Robot>
            {
            new Robot("Alfred", 100, true, new List<string> { "CPU", "Laser" }, State.Ready) { Id = "12d8bfe4-498f-432d-862b-425b0843509c" },
            new Robot("Nausica", 220, true, new List<string> { "CPU", "Bio scanner", "DSP" }, State.Sleeping),
            new Robot("Kosuna", 190, false, new List<string>(), State.Ready) { Message = @"\'\\''" }
            };

        [TestInitialize]
        public async Task CreateRobots()
        {
            var appSettings = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .AddUserSecrets("oogi2")
                .AddEnvironmentVariables()
                .Build();

            _con = new Connection(appSettings["endpoint"], appSettings["authorizationKey"], appSettings["database"], appSettings["collection"], "/partitionKey");

            _repo = new Repository<Robot>(_con);
            _fakeRepo = new Repository<FakeRobot>(_con);
            _aggregate = new AggregateRepository(_con);
            _dynamic = new CommonRepository<dynamic>(_con);

            await DeleteRobots();

            foreach (var robot in _robots)
                await _repo.UpsertAsync(robot);            
        }

        [TestCleanup]
        public async Task DeleteRobots()
        {
            //if (_con.ContainerId.Equals("hub", StringComparison.OrdinalIgnoreCase))
            //    throw new Exception("Hub cannot be deleted.");

            //await _con.DeleteContainerAsync();

            var robots = await _repo.GetAllAsync();

            foreach(var r in robots)
            {
                await _repo.DeleteAsync(r);
            }
        }

        [TestMethod]
        public async Task SelectAll()
        {
            var robots = await _repo.GetAllAsync();

            Assert.AreEqual(_robots.Count, robots.Count);
        }

        [TestMethod]
        public async Task SelectByNonExistentEnum()
        {
            var q = new DynamicQuery("select * from c where c.entity = @entity and c.state = @state",
                new
                {
                    entity = Entities.Robot,
                    state = State.Destroyed
                });

            var robots = await _repo.GetListAsync(q);

            Assert.AreEqual(_robots.Count(x => x.State == State.Destroyed), robots.Count);
        }

        [TestMethod]
        public async Task SelectByMoreEnumsAsList()
        {
            var q = new DynamicQuery("select * from c where c.entity = @entity and c.state in @states",
                new
                {
                    entity = Entities.Robot,
                    states = new List<State> { State.Ready, State.Sleeping }
                });

            var robots = await _repo.GetListAsync(q);

            Assert.AreEqual(_robots.Count(x => x.State != State.Destroyed), robots.Count);
        }

        [TestMethod]
        public async Task SelectByMoreEnumsAsFirstOrDefault()
        {
            var q = new DynamicQuery("select * from c where c.entity = @entity and c.state in @states",
                new
                {
                    entity = Entities.Robot,
                    states = new List<State> { State.Destroyed, State.Sleeping }
                });

            var robot = await _repo.GetFirstOrDefaultAsync(q);

            Assert.AreEqual("Nausica", robot.Name);
        }

        [TestMethod]
        public async Task SelectByMoreEnumsAsFirstOrDefaultWithNoResult()
        {
            var q = new DynamicQuery("select * from c where c.entity = @entity and c.state in @states",
                new
                {
                    entity = Entities.Robot,
                    states = new List<State> { State.Destroyed, State.Destroyed, State.Destroyed }
                });

            var robot = await _repo.GetFirstOrDefaultAsync(q);

            Assert.AreEqual(null, robot);
        }

        [TestMethod]
        public async Task SelectList()
        {
            var q = new QueryDefinition("select * from c where c.entity = @entity and c.artificialIq > @iq")
                .WithParameter("@entity", Entities.Robot)
                .WithParameter("@iq", 120);

            var robots = await _repo.GetListAsync(q);

            Assert.AreEqual(_robots.Count(x => x.ArtificialIq > 120), robots.Count);
        }

        [TestMethod]
        public async Task AggregateCount()
        {
            var q = new QueryDefinition("select count(1) from c where c.entity = @entity and c.artificialIq > @iq")
                .WithParameter("@entity", Entities.Robot)
                .WithParameter("@iq", 120);

            var result = await _aggregate.GetAsync(q);

            Assert.AreEqual(_robots.Count(x => x.ArtificialIq > 120), result);
        }

        [TestMethod]
        public async Task AggregateMax()
        {
            var q = new QueryDefinition("select max(c.artificialIq) from c where c.entity = @entity")
                .WithParameter("@entity", Entities.Robot);

            var result = await _aggregate.GetAsync(q);

            Assert.AreEqual(_robots.Max(x => x.ArtificialIq), result);
        }

        [TestMethod]
        public async Task AggregateInvalid()
        {
            var q = new QueryDefinition("select min(c.somethingThatDoesntExist) from c where c.entity = @entity")
                .WithParameter("@entity", Entities.Robot);

            var result = await _aggregate.GetAsync(q);

            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task SelectListDynamic()
        {
            var robots = await _repo.GetListAsync("select * from c where c.entity = @entity and c.artificialIq > @iq",
                new
                {
                    entity = Entities.Robot,
                    iq = 120
                });

            Assert.AreEqual(_robots.Count(x => x.ArtificialIq > 120), robots.Count);
        }

        [TestMethod]
        public async Task SelectFirstOrDefault()
        {
            var robot = await _repo.GetFirstOrDefaultAsync();

            Assert.AreNotEqual(robot, null);
            Assert.AreEqual(100, robot.ArtificialIq);

            var q = new DynamicQuery("select * from c where c.entity = @entity and c.artificialIq = @iq",
                new
                {
                    entity = Entities.Robot,
                    iq = 190

                });

            robot = await _repo.GetFirstOrDefaultAsync(q);

            Assert.AreNotEqual(robot, null);
            Assert.AreEqual(190, robot.ArtificialIq);

            robot = await _repo.GetFirstOrDefaultAsync("select * from c where c.entity = @entity and c.artificialIq = @iq",
                new
                {
                    entity = Entities.Robot,
                    iq = 190
                });

            Assert.AreNotEqual(robot, null);
            Assert.AreEqual(190, robot.ArtificialIq);

            var fakeRobot = new FakeRobot("Sagiri", 111, true, new List<string>(), State.Sleeping);
            fakeRobot = await _fakeRepo.CreateAsync(fakeRobot);

            Assert.AreEqual(fakeRobot.Id, (await _fakeRepo.GetFirstOrDefaultAsync(fakeRobot.Id)).Id);
            Assert.IsNull((await _repo.GetFirstOrDefaultAsync(fakeRobot.Id)));

            var d = await _dynamic.GetFirstOrDefaultAsync(fakeRobot.Id);
            
            Assert.AreEqual(fakeRobot.Id, ((JObject)d).SelectToken("id"));
        }

        [TestMethod]
        public async Task SelectEscapedAsync()
        {
            var q = new DynamicQuery("select * from c where c.entity = @entity and c.message = @message",
                new
                {
                    entity = Entities.Robot,
                    message = @"\'\\''"
                }
            );

            var robot = await _repo.GetFirstOrDefaultAsync(q).ConfigureAwait(false);

            Assert.AreNotEqual(robot, null);

            var oldId = robot.Id;
            robot = await _repo.GetFirstOrDefaultAsync(oldId).ConfigureAwait(false);

            Assert.AreNotEqual(robot, null);
            Assert.AreEqual(robot.Id, oldId);
        }

        [TestMethod]
        public async Task SelectEscaped()
        {
            var q = new DynamicQuery("select * from c where c.entity = @entity and c.message = @message",
                new
                {
                    entity = Entities.Robot,
                    message = @"\'\\''"
                }
            );

            var robot = await _repo.GetFirstOrDefaultAsync(q);

            Assert.AreNotEqual(robot, null);

            var oldId = robot.Id;
            robot = await _repo.GetFirstOrDefaultAsync(oldId, "oogi2");

            Assert.AreNotEqual(robot, null);
            Assert.AreEqual(robot.Id, oldId);
        }

        [TestMethod]
        public async Task Delete()
        {
            var robots = await _repo.GetListAsync("select * from c where c.entity = @entity order by c.artificialIq", new { entity = Entities.Robot });

            Assert.AreEqual(_robots.Count, robots.Count);

            var dumbestRobotId = robots[0].Id;

            await _repo.DeleteAsync(dumbestRobotId, "oogi2");

            var smartestRobot = robots[robots.Count - 1];

            await _repo.DeleteAsync(smartestRobot);

            robots = await _repo.GetAllAsync();

            Assert.AreEqual(1, robots.Count);
            Assert.AreEqual(_robots.OrderBy(x => x.ArtificialIq).Skip(1).First().ArtificialIq, robots[0].ArtificialIq);
        }

        [TestMethod]
        public async Task CreateDynamic()
        {
            var repo = new CommonRepository<dynamic>(_con);
            
            var d1 = await repo.CreateAsync(new { Id = (string)null, Movie = "Donkey Kong Jr.", Rating = 3, PartitionKey = "oogi2", Entity = "movie" });            
            Assert.IsNotNull(((JObject)d1).SelectToken("id"));
            Assert.AreEqual("Donkey Kong Jr.", ((JObject)d1).SelectToken("movie"));

            var d2 = await repo.CreateAsync(new { Movie = "King Kong", Rating = 2, PartitionKey = "oogi2", Entity = "movie" });
            Assert.IsNotNull(((JObject)d2).SelectToken("id"));
            Assert.AreEqual("King Kong", ((JObject)d2).SelectToken("movie"));

            var d3 = await repo.CreateAsync(new { Id = "3a80fdb6-6f34-4ac7-a4d0-df96592e6eda", Movie = "Donkey Kong", Rating = 1, PartitionKey = "oogi2", Entity = "movie" });
            Assert.IsNotNull(((JObject)d3).SelectToken("id"));
            Assert.AreEqual("3a80fdb6-6f34-4ac7-a4d0-df96592e6eda", ((JObject)d3).SelectToken("id"));

            var movies = await repo.GetListAsync("select * from c where c.entity = 'movie' and c.rating <> null", null);
            Assert.AreEqual(3, movies.Count);

            await repo.DeleteAsync(new { Id = ((JObject)d1).SelectToken("id"), PartitionKey = "oogi2" });
            movies = await repo.GetListAsync("select * from c where c.entity = 'movie' and c.rating <> null", null);
            Assert.AreEqual(2, movies.Count);

            await repo.DeleteAsync(new { Id = ((JObject)d2).SelectToken("id"), PartitionKey = "oogi2" });
            movies = await repo.GetListAsync("select * from c where c.entity = 'movie' and c.rating <> null", null);
            Assert.AreEqual(1, movies.Count);

            await repo.DeleteAsync("3a80fdb6-6f34-4ac7-a4d0-df96592e6eda", "oogi2");
            movies = await repo.GetListAsync("select * from c where c.entity = 'movie' and c.rating <> null", null);
            Assert.AreEqual(0, movies.Count);
        }

        // [TestMethod]
        // public async Task OptimisticConcurrency()
        // {
        //     var robot = await _repo.GetFirstOrDefaultAsync("select top 1 * from c where c.entity = @entity order by c.artificialIq", new { entity = Robot.Entity }).ConfigureAwait(false);

        //     var ro = new RequestOptions { AccessCondition = new AccessCondition { Condition = robot.ETag, Type = AccessConditionType.IfMatch } };

        //     var ro2 = await _repo.ReplaceAsync(robot, ro).ConfigureAwait(false);

        //     Assert.AreNotEqual(robot.ETag, ro2.ETag);

        //     var ok = false;

        //     try
        //     {
        //         await _repo.ReplaceAsync(robot, ro).ConfigureAwait(false);
        //     }
        //     catch (OogiException)
        //     {
        //         ok = true;
        //     }

        //     Assert.AreEqual(true, ok);

        //     ok = false;
        //     await _repo.ReplaceAsync(robot).ConfigureAwait(false);
        //     ok = true;

        //     Assert.AreEqual(true, ok);
        // }
    }
}