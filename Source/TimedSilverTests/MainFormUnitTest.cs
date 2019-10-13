using cs_timed_silver;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Windows.Forms;

namespace TimedSilverTests
{
    [TestClass]
    public class MainFormUnitTest
    {
        [TestMethod]
        public void MainFormTest1()
        {
            // Arrange
            cs_timed_silver.Properties.Settings.Default.AutoOpenLastFile = "No";
            var mf = new MainForm();
            mf.Show();

            // Act
            Application.DoEvents();

            // Assert
            Assert.AreEqual(100, mf.MyFocusedClocksViewProvider.GetZoomPercent(), $"Initial zoom of focused view should be 100%, not {mf.MyFocusedClocksViewProvider.GetZoomPercent()}%");
        }

        /// <summary>
        /// Open program with new unsaved file,
        /// load a file (my personal file only?),
        /// select New,
        /// and 2 timers are shown instead of 1.
        /// </summary>
        [TestMethod]
        public void MainFormTest2()
        {
            // Arrange
            cs_timed_silver.Properties.Settings.Default.AutoOpenLastFile = "No";
            var mf = new MainForm();
            mf.Show();

            Application.DoEvents();

            mf.MyDataFile.LoadFromFile("test.xml");

            Application.DoEvents();

            mf.tsmiNew.PerformClick();

            // Act
            Application.DoEvents();

            // Assert
            Assert.AreEqual(1, (mf.LastFocusedTimersView as ClockListView).MyClockFlowLayoutPanel.Controls.Count);
        }
    }
}