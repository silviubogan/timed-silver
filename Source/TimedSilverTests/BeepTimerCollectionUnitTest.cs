using System;
using System.Linq;
using System.Windows.Forms;
using cs_timed_silver;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TimedSilverTests
{
    [TestClass]
    public class BeepTimerCollectionUnitTest
    {
        [TestMethod]
        public void BeepTimerCollectionTest_Delete()
        {
            var b = new BeepTimerCollection();

            b.BeepTimers = (from x in Enumerable.Range(10000, 5)
                            select new Timer() { Interval = x }).ToArray();

            var t = b.BeepTimers[0];

            bool disp = false;

            t.Disposed += delegate (object sender, EventArgs e)
            {
                disp = true;
            };

            t.Start();

            b.Delete();

            Assert.IsTrue(disp);
            Assert.IsNull(b.BeepTimers);
        }

        [TestMethod]
        public void BeepTimerCollectionTest_IndexOf()
        {
            var b = new BeepTimerCollection();

            b.BeepTimers = (from x in Enumerable.Range(10000, 5)
                            select new Timer() { Interval = x }).ToArray();

            var t0 = b.BeepTimers[0];
            var t3 = b.BeepTimers[3];

            Assert.AreEqual(0, b.IndexOf(t0));
            Assert.AreEqual(3, b.IndexOf(t3));
        }
    }
}
