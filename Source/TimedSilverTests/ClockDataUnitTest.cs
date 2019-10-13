using System;
using System.Drawing;
using System.Windows.Forms;
using cs_timed_silver;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TimedSilverTests
{
    [TestClass]
    public class ClockDataUnitTest
    {
        [TestMethod]
        public void ClockDataTest_Reset()
        {
            // Arrange
            var f = new MainForm();
            var td = new TimerData(f.MyDataFile, f.MultiAudioPlayer);

            td.CurrentTimeSpan = TimeSpan.FromMinutes(9);
            td.ResetToValue = TimeSpan.FromMinutes(5);
            td.Reset();

            // Assert
            Assert.AreEqual(td.ResetToValue, td.CurrentValue);
        }

        [TestMethod]
        public void ClockDataTest_ShowTimeOutForm()
        {
            // Arrange
            var f = new MainForm();
            var td = new TimerData(f.MyDataFile, f.MultiAudioPlayer);

            td.CurrentTimeSpan = TimeSpan.FromMinutes(9);
            td.ResetToValue = TimeSpan.FromMinutes(5);
            td.ShowTimeOutForm();

            Application.DoEvents();

            // Assert
            Assert.IsTrue(td.MultiAudioPlayer.HasClockData(td));
            Assert.IsTrue(td.MultiAudioPlayer.IsPlaying);
            Assert.IsTrue(f.MyTimeOutFormsManager.HasVisibleForm);
        }

        [TestMethod]
        public void ClockDataTest_MyTimeOutForm_Visible_changed()
        {
            // Arrange
            var f = new MainForm();
            var td = new TimerData(f.MyDataFile, f.MultiAudioPlayer);

            td.CurrentTimeSpan = TimeSpan.FromMinutes(9);
            td.ResetToValue = TimeSpan.FromMinutes(5);
            td.ShowTimeOutForm();

            Application.DoEvents();

            f.MyTimeOutFormsManager.HideForm(td);

            // Assert
            Assert.IsFalse(f.MyTimeOutFormsManager.HasVisibleForm);
            Assert.IsFalse(td.MultiAudioPlayer.HasClockData(td));
            Assert.IsTrue(!f.MyTimeOutFormsManager.MyForms.ContainsKey(td) || Utils.IsNullOrDisposed(f.MyTimeOutFormsManager.MyForms[td]));
        }

        [TestMethod]
        public void ClockDataTest_GetIndex()
        {
            // Arrange
            var f = new MainForm();
            var td = new TimerData(f.MyDataFile, f.MultiAudioPlayer);

            Assert.AreEqual(-1, td.GetIndex());

            f.MyDataFile.ClockVMCollection.Model.AddClock(td);

            Assert.AreEqual(0, td.GetIndex());
        }

        [TestMethod]
        public void ClockDataTest_Equals()
        {
            // Arrange
            var f = new MainForm();

            var td = new TimerData(f.MyDataFile, f.MultiAudioPlayer)
            {
                Enabled = false,
                FilteredOut = false,
                GroupName = "",
                Icon = null,
                IsUnsaved = true,
                IsUnsavedLocked = false,
                ResetToValueLocked = false,
                Style = ClockM.ClockStyles.ShowIconAndID,
                Tag = "Test.",
                UserBackColor = Color.Green,
                CurrentValue = TimeSpan.FromSeconds(10),
                ResetToValue = TimeSpan.FromMilliseconds(10 * 60 * 1000)
            };

            var td2 = new TimerData(f.MyDataFile, f.MultiAudioPlayer)
            {
                Enabled = false,
                FilteredOut = false,
                GroupName = "",
                Icon = null,
                IsUnsaved = true,
                IsUnsavedLocked = false,
                ResetToValueLocked = false,
                Style = ClockM.ClockStyles.ShowID,
                Tag = "Test.",
                UserBackColor = Color.Green,
                CurrentValue = TimeSpan.FromSeconds(10),
                ResetToValue = TimeSpan.FromMilliseconds(10 * 60 * 1000)
            };

            Assert.AreEqual(td, td);
            Assert.AreEqual(td2, td2);
            Assert.AreNotEqual(td, td2);
        }

        [TestMethod]
        public void ClockDataTest_Equals_2()
        {
            // Arrange
            var f = new MainForm();

            var td = new TimerData(f.MyDataFile, f.MultiAudioPlayer)
            {
                Enabled = false,
                FilteredOut = false,
                GroupName = "",
                Icon = null,
                IsUnsaved = true,
                IsUnsavedLocked = false,
                ResetToValueLocked = false,
                Style = ClockM.ClockStyles.ShowIconAndID,
                Tag = "Test.",
                UserBackColor = Color.Green,
                CurrentValue = TimeSpan.FromSeconds(10),
                ResetToValue = TimeSpan.FromMilliseconds(10 * 60 * 1000)
            };

            var td2 = new 
            {
                Enabled = false,
                FilteredOut = false,
                GroupName = "",
                IsUnsaved = true,
                IsUnsavedLocked = false,
                ResetToValueLocked = false,
                Style = ClockM.ClockStyles.ShowID,
                Tag = "Test.",
                UserBackColor = Color.Green,
                CurrentValue = TimeSpan.FromSeconds(10),
                ResetToValue = TimeSpan.FromMilliseconds(10 * 60 * 1000)
            };

            Assert.AreNotEqual(td, null);
            Assert.AreNotEqual(td, td2);
        }
    }
}
