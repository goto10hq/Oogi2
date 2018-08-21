using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Oogi2;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Linq;
using Oogi2.Queries;
using Microsoft.Azure.Documents;
using Oogi2.Tests.Helpers;
using static Oogi2.Tests.Helpers.Enums;
using System;
using System.Threading.Tasks;
using Microsoft.Azure.Documents.Client;
using System.Net.Http.Headers;
using System.Reflection;

namespace Tests
{
    [TestClass]
    public class BasicOperation
    {
        static Repository<Robot> _repo;
        static AggregateRepository _aggregate;
        static Connection _con;

        readonly List<Robot> _robots = new List<Robot>
            {
            new Robot("Alfred", 100, true, new List<string> { "CPU", "Laser" }, State.Ready),
            new Robot("Nausica", 220, true, new List<string> { "CPU", "Bio scanner", "DSP" }, State.Sleeping),
            new Robot("Kosuna", 190, false, new List<string>(), State.Ready) { Message = @"\'\\''" }
            };

        [TestInitialize]
        public void CreateRobots()
        {
            var appSettings = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .AddUserSecrets("oogi2")
                .AddEnvironmentVariables()
                .Build();

            _con = new Connection(appSettings["endpoint"], appSettings["authorizationKey"], appSettings["database"], appSettings["collection"]);

            _con.CreateCollection();

            _repo = new Repository<Robot>(_con);
            _aggregate = new AggregateRepository(_con);

            foreach (var robot in _robots.Take(_robots.Count - 1))
                _repo.Create(robot);

            foreach (var robot in _robots.TakeLast(1))
                _repo.Upsert(robot);
        }

        [TestCleanup]
        public void DeleteRobots()
        {
            if (_con.CollectionId.Equals("hub", StringComparison.OrdinalIgnoreCase))
                throw new Exception("Hub cannot be deleted.");

            _con.DeleteCollection();
        }

        [TestMethod]
        public void SelectAll()
        {
            var robots = _repo.GetAll();

            Assert.AreEqual(_robots.Count, robots.Count);
        }

        [TestMethod]
        public void SelectByNonExistentEnum()
        {
            var q = new DynamicQuery("select * from c where c.entity = @entity and c.state = @state",
                new
                {
                    entity = Robot.Entity,
                    state = State.Destroyed
                });

            var robots = _repo.GetList(q);

            Assert.AreEqual(_robots.Count(x => x.State == State.Destroyed), robots.Count);
        }

        [TestMethod]
        public void SelectByMoreEnumsAsList()
        {
            var q = new DynamicQuery("select * from c where c.entity = @entity and c.state in @states",
                new
                {
                    entity = Robot.Entity,
                    states = new List<State> { State.Ready, State.Sleeping }
                });

            var robots = _repo.GetList(q);

            Assert.AreEqual(_robots.Count(x => x.State != State.Destroyed), robots.Count);
        }

        [TestMethod]
        public void SelectByMoreEnumsAsFirstOrDefault()
        {
            var q = new DynamicQuery("select * from c where c.entity = @entity and c.state in @states",
                new
                {
                    entity = Robot.Entity,
                    states = new List<State> { State.Destroyed, State.Sleeping }
                });

            var robot = _repo.GetFirstOrDefault(q);

            Assert.AreEqual("Nausica", robot.Name);
        }

        [TestMethod]
        public void SelectByMoreEnumsAsFirstOrDefaultWithNoResult()
        {
            var q = new DynamicQuery("select * from c where c.entity = @entity and c.state in @states",
                new
                {
                    entity = Robot.Entity,
                    states = new List<State> { State.Destroyed, State.Destroyed, State.Destroyed }
                });

            var robot = _repo.GetFirstOrDefault(q);

            Assert.AreEqual(null, robot);
        }

        [TestMethod]
        public void SelectList()
        {
            var q = new SqlQuerySpec("select * from c where c.entity = @entity and c.artificialIq > @iq",
                new SqlParameterCollection
                {
                    new SqlParameter("@entity", Robot.Entity),
                    new SqlParameter("@iq", 120)
                });

            var robots = _repo.GetList(q);

            Assert.AreEqual(_robots.Count(x => x.ArtificialIq > 120), robots.Count);
        }

        [TestMethod]
        public void AggregateCount()
        {
            var q = new SqlQuerySpec("select count(1) from c where c.entity = @entity and c.artificialIq > @iq",
                new SqlParameterCollection
                {
                    new SqlParameter("@entity", Robot.Entity),
                    new SqlParameter("@iq", 120)
                });

            var result = _aggregate.Get(q);

            Assert.AreEqual(_robots.Count(x => x.ArtificialIq > 120), result);
        }

        [TestMethod]
        public void AggregateMax()
        {
            var q = new SqlQuerySpec("select max(c.artificialIq) from c where c.entity = @entity",
                new SqlParameterCollection
                {
                    new SqlParameter("@entity", Robot.Entity)
                });

            var result = _aggregate.Get(q);

            Assert.AreEqual(_robots.Max(x => x.ArtificialIq), result);
        }

        [TestMethod]
        public void AggregateInvalid()
        {
            var q = new SqlQuerySpec("select min(c.somethingThatDoesntExist) from c where c.entity = @entity",
                new SqlParameterCollection
                {
                    new SqlParameter("@entity", Robot.Entity)
                });

            var result = _aggregate.Get(q);

            Assert.IsNull(result);
        }

        [TestMethod]
        public void SelectListDynamic()
        {
            var robots = _repo.GetList("select * from c where c.entity = @entity and c.artificialIq > @iq",
                new
                {
                    entity = Robot.Entity,
                    iq = 120
                });

            Assert.AreEqual(_robots.Count(x => x.ArtificialIq > 120), robots.Count);
        }

        [TestMethod]
        public void SelectFirstOrDefault()
        {
            var robot = _repo.GetFirstOrDefault();

            Assert.AreNotEqual(robot, null);
            Assert.AreEqual(100, robot.ArtificialIq);

            var q = new SqlQuerySpec("select * from c where c.entity = @entity and c.artificialIq = @iq")
            {
                Parameters = new SqlParameterCollection
                                     {
                                         new SqlParameter("@entity", Robot.Entity),
                                         new SqlParameter("@iq", 190)
                                     }
            };

            robot = _repo.GetFirstOrDefault(q);

            Assert.AreNotEqual(robot, null);
            Assert.AreEqual(190, robot.ArtificialIq);

            robot = _repo.GetFirstOrDefault("select * from c where c.entity = @entity and c.artificialIq = @iq",
                new
                {
                    entity = Robot.Entity,
                    iq = 190
                });

            Assert.AreNotEqual(robot, null);
            Assert.AreEqual(190, robot.ArtificialIq);
        }

        [TestMethod]
        public async Task SelectEscapedAsync()
        {
            var q = new DynamicQuery("select * from c where c.entity = @entity and c.message = @message",
                new
                {
                    entity = Robot.Entity,
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
        public void SelectEscaped()
        {
            var q = new DynamicQuery("select * from c where c.entity = @entity and c.message = @message",
                new
                {
                    entity = Robot.Entity,
                    message = @"\'\\''"
                }
            );

            var robot = _repo.GetFirstOrDefault(q);

            Assert.AreNotEqual(robot, null);

            var oldId = robot.Id;
            robot = _repo.GetFirstOrDefault(oldId);

            Assert.AreNotEqual(robot, null);
            Assert.AreEqual(robot.Id, oldId);
        }

        [TestMethod]
        public void Delete()
        {
            var robots = _repo.GetList("select * from c where c.entity = @entity order by c.artificialIq", new { entity = Robot.Entity });

            Assert.AreEqual(_robots.Count, robots.Count);

            var dumbestRobotId = robots[0].Id;

            _repo.Delete(dumbestRobotId);

            var smartestRobot = robots[robots.Count - 1];

            _repo.Delete(smartestRobot);

            robots = _repo.GetAll();

            Assert.AreEqual(1, robots.Count);
            Assert.AreEqual(_robots.OrderBy(x => x.ArtificialIq).Skip(1).First().ArtificialIq, robots[0].ArtificialIq);
        }

        [TestMethod]
        public void CreateDynamic()
        {
            var repo = new Repository(_con);
            repo.Create(new { Movie = "Donkey Kong Jr.", Rating = 3 });
            repo.Create(new { Movie = "King Kong", Rating = 2 });
            repo.Create(new { Movie = "Donkey Kong", Rating = 1 });

            var movies = repo.GetList("select * from c where c.rating <> null", null);

            Assert.AreEqual(3, movies.Count);

            repo.Delete(movies[0]);

            movies = repo.GetList("select * from c where c.rating <> null", null);

            Assert.AreEqual(2, movies.Count);
        }

        [TestMethod]
        public async Task OptimisticConcurrency()
        {
            var robot = await _repo.GetFirstOrDefaultAsync("select top 1 * from c where c.entity = @entity order by c.artificialIq", new { entity = Robot.Entity }).ConfigureAwait(false);

            var ro = new RequestOptions { AccessCondition = new AccessCondition { Condition = robot.ETag, Type = AccessConditionType.IfMatch } };

            var ro2 = await _repo.ReplaceAsync(robot, ro).ConfigureAwait(false);

            Assert.AreNotEqual(robot.ETag, ro2.ETag);

            // Microsoft.Azure.Documents.PreconditionFailedException is being thrown

            var ok = false;

            try
            {
                await _repo.ReplaceAsync(robot, ro).ConfigureAwait(false);
            }
            catch (DocumentClientException)
            {
                ok = true;
            }

            Assert.AreEqual(true, ok);

            ok = false;
            await _repo.ReplaceAsync(robot).ConfigureAwait(false);
            ok = true;

            Assert.AreEqual(true, ok);
        }
    }
}