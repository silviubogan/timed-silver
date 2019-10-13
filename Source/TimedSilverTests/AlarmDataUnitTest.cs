using System;
using cs_timed_silver;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TimedSilverTests
{
    [TestClass]
    public class AlarmDataUnitTest
    {
        [TestMethod]
        public void AlarmDataTest_Enabled_change()
        {
            var f = new MainForm();
            var ad = new AlarmData(f.MyDataFile, f.MyDataFile.MultiAudioPlayer);

            ad.Enabled = false;

            bool pass = false;

            ad.EnabledChanged += delegate (object sender, ClockEventArgs e)
            {
                pass = e.Clock.Enabled;
            };

            ad.Enabled = true;

            Assert.IsTrue(pass);
        }

        [TestMethod]
        public void AlarmDataTest_CurrentDateTime_change()
        {
            // Arrange
            var f = new MainForm();
            var ad = new AlarmData(f.MyDataFile, f.MyDataFile.MultiAudioPlayer);

            object val = ad.CurrentDateTime;

            // Assert
            Assert.AreEqual(ad.CurrentDateTime, ad.CurrentValue);
        }

        [TestMethod]
        public void AlarmDataTest_EnableOrDisable()
        {
            // Arrange
            var f = new MainForm();
            var ad = new AlarmData(f.MyDataFile, f.MyDataFile.MultiAudioPlayer);

            ad.Enabled = false;
            ad.EnableOrDisable();

            // Assert
            Assert.IsTrue(ad.Enabled);
        }

        [TestMethod]
        public void AlarmDataTest_ctor()
        {
            // Arrange
            var f = new MainForm();
            var ad = new AlarmData(f.MyDataFile, f.MyDataFile.MultiAudioPlayer);

            // Assert
            Assert.IsFalse(ad.IsUnsavedLocked);
            Assert.IsFalse(ad.IsUnsaved);
        }
    }
}
