using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Oogi2.Tokens;
using Sushi2;

namespace Tests
{
    [TestClass]
    public class Various
    {
        [TestMethod]
        public void StampToString()
        {            
            var now = DateTime.Now;
            var stamp = new Stamp(now);
            var simpleStamp = new SimpleStamp(now);

            Assert.AreEqual(now.ToString(), stamp.ToString());
            Assert.AreEqual(now.ToString(), simpleStamp.ToString());            
        }

        [TestMethod]
        public void StampInterfaces()
        {
            var now = DateTime.Now;
            var simpleStamp = new SimpleStamp(now);
            var simpleStamp2 = new SimpleStamp(now);
            var stamp = new Stamp(now);            

            Assert.AreEqual(simpleStamp, simpleStamp2);
            Assert.AreEqual(stamp, simpleStamp2);                        
        }

        [TestMethod]
        public void Stamp()
        {
            var now = DateTime.Now;
            var stamp = new Stamp(now);            

            Assert.AreEqual(now.Year, stamp.Year);
            Assert.AreEqual(now.Month, stamp.Month);
            Assert.AreEqual(now.Day, stamp.Day);
            Assert.AreEqual(now.Hour, stamp.Hour);
            Assert.AreEqual(now.Minute, stamp.Minute);
            Assert.AreEqual(now.Second, stamp.Second);
            Assert.AreEqual(now.ToEpoch(), stamp.Epoch);            
        }

        [TestMethod]
        public void SimpleStamp()
        {
            var now = DateTime.Now;
            var stamp = new SimpleStamp(now);

            Assert.AreEqual(now.ToEpoch(), stamp.Epoch);
        }
    }
}
