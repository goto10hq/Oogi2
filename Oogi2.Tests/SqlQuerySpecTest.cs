﻿using System;
using System.Collections.Generic;
using Microsoft.Azure.Cosmos;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Oogi2;
using Oogi2.Tests.Helpers;
using Oogi2.Tokens;

namespace Tests
{
    [TestClass]
    public class SqlQuerySpecTest
    {
        [TestMethod]
        public void Classic()
        {
            var q = new QueryDefinition("a = @a, b = @b, c = @c, d = @d, e = @e, f = @f, g = @g, h = @h")
                    .WithParameter("@a", "!''!")
                    .WithParameter("@b", 'x')
                    .WithParameter("@c", null)
                    .WithParameter("@d", true)
                    .WithParameter("@e", 13)
                    .WithParameter("@f", 13.99)
                    .WithParameter("@g", Guid.Empty)
                    .WithParameter("@h", new Uri("https://www.goto10.cz"));


            var sql = q.ToSqlQuery();

            Assert.AreEqual("a = '!\\'\\'!', b = 'x', c = null, d = true, e = 13, f = 13.99, g = '00000000-0000-0000-0000-000000000000', h = 'https://www.goto10.cz/'", sql);
        }

        public enum State
        {
            Ready = 10,
            Processing = 20,
            Finished = 30
        }

        [TestMethod]
        public void Enum()
        {
            var q = new QueryDefinition("state = @state").WithParameter("@state", State.Processing);

            var sql = q.ToSqlQuery();

            Assert.AreEqual("state = 20", sql);
        }

        [TestMethod]
        public void Stamps()
        {
            var q = new QueryDefinition("epoch = @stamp, epoch2 in (@stamp2)")
                    .WithParameter("@stamp", new Stamp(new DateTime(2000, 1, 1)))
                    .WithParameter("@stamp2", new SimpleStamp(new DateTime(2001, 1, 1)));

            var sql = q.ToSqlQuery();

            Assert.AreEqual("epoch = 946684800, epoch2 in (9466848002)", sql);
        }

        [TestMethod]
        public void ListOfInts()
        {
            var ids = new List<int> { 4, 5, 2 };

            var q = new QueryDefinition("items in @ids")
                .WithParameter("@ids", ids);

            var sql = q.ToSqlQuery();

            Assert.AreEqual("items in (4,5,2)", sql);
        }

        [TestMethod]
        public void ListOfStrings()
        {
            var ids = new List<string> { "abra", "ca", "da'bra" };

            var q = new QueryDefinition("items in @ids")
                .WithParameter("@ids", ids);

            var sql = q.ToSqlQuery();

            Assert.AreEqual("items in ('abra','ca','da\\'bra')", sql);
        }

        [TestMethod]
        public void ListOfEnums()
        {
            var ids = new List<State> { State.Ready, State.Finished };

            var q = new QueryDefinition("items in @ids")
                .WithParameter("@ids", ids);

            var sql = q.ToSqlQuery();

            Assert.AreEqual("items in (10,30)", sql);
        }

        [TestMethod]
        public void ListOfFloats()
        {
            var ids = new List<float> { 6.3f, 8.2f };

            var q = new QueryDefinition("items in @ids")
                .WithParameter("@ids", ids);

            var sql = q.ToSqlQuery();

            Assert.AreEqual("items in (6.3,8.2)", sql);
        }

        [TestMethod]
        public void EmptyList()
        {
            var ids = new List<State>();

            var q = new QueryDefinition("items in @ids")
                .WithParameter("@ids", ids);

            var sql = q.ToSqlQuery();

            Assert.AreEqual("items in (null)", sql);
        }

        [TestMethod]
        public void Nulls()
        {
            const string a = null;
            float? b = null;
            Guid? c = null;

            var q = new QueryDefinition("a = @a, b = @b, c = @c")
                .WithParameter("@a", a)
                .WithParameter("@b", b)
                .WithParameter("@c", c);

            var sql = q.ToSqlQuery();

            Assert.AreEqual("a = null, b = null, c = null", sql);
        }

        [TestMethod]
        public void Escaping()
        {
            var q = new QueryDefinition("select * from c where c.entity = @entity and c.message = @message")
                .WithParameter("@entity", Entities.Robot)
                .WithParameter("@message", @"\'\\''");

            var sql = q.ToSqlQuery();

            Assert.AreEqual(@"select * from c where c.entity = 'oogi2/robot' and c.message = '\\\'\\\\\'\''", sql);
        }
    }
}