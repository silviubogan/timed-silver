using System;
using cs_timed_silver;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TimedSilverTests
{
    [TestClass]
    public class SettingDataUnitTest
    {
        [TestMethod]
        public void SettingDataTest_Equals()
        {
            SettingDataM s = new SettingDataM(typeof(string), "MySetting", "test default")
            {
                Header = "My Header",
                Priority = 2.9f
            };

            SettingDataM s2 = new SettingDataM(typeof(string), "MySetting", "test default")
            {
                Header = "My Header",
                Priority = 5.8f
            };

            Assert.AreEqual(s, s2);

            s2.Header = "My Header!";

            Assert.AreNotEqual(s, s2);
        }
    }
}
