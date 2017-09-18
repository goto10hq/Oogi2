using System;
using System.Collections.Generic;
using Microsoft.Azure.Documents;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Oogi2;
using Oogi2.Tokens;

namespace Tests
{
    [TestClass]
    public class SqlQuerySpecTest
    {        
        [TestMethod]
        public void Classic()
        {
            var q = new SqlQuerySpec("a = @a, b = @b, c = @c, d = @d, e = @e, f = @f",
                new SqlParameterCollection
                {
                    new SqlParameter("@a", "!''!"),
                    new SqlParameter("@b", 'x'),
                    new SqlParameter("@c", null),
                    new SqlParameter("@d", true),
                    new SqlParameter("@e", 13),
                    new SqlParameter("@f", 13.99)
                });

            var sql = q.ToSqlQuery();
            
            Assert.AreEqual("a = '!\\'\\'!', b = 'x', c = null, d = true, e = 13, f = 13.99", sql);            
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
            var q = new SqlQuerySpec("state = @state",
                new SqlParameterCollection
                {
                    new SqlParameter("@state", State.Processing),                    
                });

            var sql = q.ToSqlQuery();

            Assert.AreEqual("state = 20", sql);
        }

        [TestMethod]
        public void Stamps()
        {
            var q = new SqlQuerySpec("epoch = @stamp, epoch2 in (@stamp2)",
                new SqlParameterCollection
                {
                    new SqlParameter("@stamp", new Stamp(new DateTime(2000, 1, 1))),
                    new SqlParameter("@stamp2", new SimpleStamp(new DateTime(2001, 1, 1))),
                });

            var sql = q.ToSqlQuery();

            Assert.AreEqual("epoch = 946684800, epoch2 in (9466848002)", sql);
        }

        [TestMethod]
        public void ListOfInts()
        {
            var ids = new List<int> { 4, 5, 2 };

            var q = new SqlQuerySpec("items in @ids",
                new SqlParameterCollection
                {
                    new SqlParameter("@ids", ids),
                });

            var sql = q.ToSqlQuery();

            Assert.AreEqual("items in (4,5,2)", sql);
        }

        [TestMethod]
        public void ListOfStrings()
        {
            var ids = new List<string> { "abra", "ca", "da'bra" };

            var q = new SqlQuerySpec("items in @ids",
                new SqlParameterCollection
                {
                    new SqlParameter("@ids", ids),
                });

            var sql = q.ToSqlQuery();

            Assert.AreEqual("items in ('abra','ca','da\\'bra')", sql);
        }

        [TestMethod]
        public void ListOfEnums()
        {
            var ids = new List<State> { State.Ready, State.Finished };

            var q = new SqlQuerySpec("items in @ids",
                new SqlParameterCollection
                {
                    new SqlParameter("@ids", ids),
                });

            var sql = q.ToSqlQuery();

            Assert.AreEqual("items in (10,30)", sql);
        }

        [TestMethod]
        public void ListOfFloats()
        {
            var ids = new List<float> { 6.3f, 8.2f };

            var q = new SqlQuerySpec("items in @ids",
                new SqlParameterCollection
                {
                    new SqlParameter("@ids", ids),
                });

            var sql = q.ToSqlQuery();

            Assert.AreEqual("items in (6.3,8.2)", sql);
        }

        [TestMethod]
        public void EmptyList()
        {
            var ids = new List<State>();

            var q = new SqlQuerySpec("items in @ids",
                new SqlParameterCollection
                {
                    new SqlParameter("@ids", ids),
                });

            var sql = q.ToSqlQuery();

            Assert.AreEqual("items in (null)", sql);
        }
    }
}
