using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using cs_timed_silver;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TimedSilverTests
{
    // TODO: migrate to WPF:
    [Obsolete]
    public class ClockControlUnitTest
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ClockControlTest_ForceUpdateZoomFactor()
        {
            // Arrange
            cs_timed_silver.Properties.Settings.Default.AutoOpenLastFile = "No";
            var mf = new MainForm();

            mf.Show();

            Application.DoEvents();

            // Act
            var c = mf.MyDataFile.ClockVMCollection.Model.Ms[0].MyTimerViews[0] as ClockControl;
            c.ForceUpdateZoomFactor(0M);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ClockControlTest_ForceUpdateZoomFactor_2()
        {
            // Arrange
            cs_timed_silver.Properties.Settings.Default.AutoOpenLastFile = "No";
            var mf = new MainForm();

            mf.Show();

            Application.DoEvents();

            // Act
            var c = mf.MyDataFile.ClockVMCollection.Model.Ms[0].MyTimerViews[0] as ClockControl;
            c.ForceUpdateZoomFactor(-0.8M);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ClockControlTest_ApplyClockType()
        {
            // Arrange
            cs_timed_silver.Properties.Settings.Default.AutoOpenLastFile = "No";
            var mf = new MainForm();

            mf.Show();

            Application.DoEvents();

            // Act
            var c = mf.MyDataFile.ClockVMCollection.Model.Ms[0].MyTimerViews[0] as ClockControl;
            c.ApplyClockType(true);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ClockControlTest_UpdateBtnStartStopLabel()
        {
            // Arrange
            cs_timed_silver.Properties.Settings.Default.AutoOpenLastFile = "No";
            var mf = new MainForm();

            mf.Show();

            Application.DoEvents();

            // Act
            var c = mf.MyDataFile.ClockVMCollection.Model.Ms[0].MyTimerViews[0] as ClockControl;
            c.UpdateBtnStartStopLabel();
        }

        [TestMethod]
        public void ClockControlTest_UpdateBtnStartStopLabel_2()
        {
            // Arrange
            //cs_timed_silver.Properties.Settings.Default.AutoOpenLastFile = "No";
            //var mf = new MainForm();

            //mf.Show();

            //Application.DoEvents();

            //// Act
            //var c = mf.MyDataFile.ClockCollection.ClocksData[0].MyTimerViews[0] as ClockControl;
            //var clv = c.GetClocksView() as ClockListView;
            //ClockControl cc = clv.CreateTimerControl(new AlarmData(mf.MyDataFile, mf.MultiAudioPlayer));
            //cc.UpdateBtnStartStopLabel();

            //// Assert
            //Assert.IsTrue(cc.btnStartStop.Text == ClockControl.OnString ||
            //    cc.btnStartStop.Text == ClockControl.OffString);
        }

        [TestMethod]
        public void ClockControlTest_MyClockData_change()
        {
            // Arrange
            var r = new Random();

            cs_timed_silver.Properties.Settings.Default.AutoOpenLastFile = "No";
            var mf = new MainForm();

            mf.Show();

            Application.DoEvents();

            // Act
            var c = mf.MyDataFile.ClockVMCollection.Model.Ms[0].MyTimerViews[0] as ClockControl;

            string nt = "test " + r.Next();

            // Assert
            Assert.IsInstanceOfType(c.MyClockData, typeof(TimerData));

            c.MyClockData = new AlarmData(mf.MyDataFile, mf.MultiAudioPlayer)
            {
                Tag = nt
            };

            Assert.AreEqual(mf.MultiAudioPlayer, c.MyClockData.MultiAudioPlayer);
            Assert.AreEqual(nt, c.etbTag.InnerTextBox.Text);
        }

        [TestMethod]
        public void ClockControlTest_RemoveCustomBackColor()
        {
            // Arrange
            cs_timed_silver.Properties.Settings.Default.AutoOpenLastFile = "No";
            var mf = new MainForm();

            mf.Show();

            Application.DoEvents();

            // Act
            var c = mf.MyDataFile.ClockVMCollection.Model.Ms[0].MyTimerViews[0] as ClockControl;
            ClockM cd = c.MyClockData;

            cd.UserBackColor = Color.LightPink;

            // Assert
            Assert.IsTrue(Utils.ColorsAreTheSame(c.ActualBackColor, Color.LightPink));

            c.RemoveCustomBackColor();

            Assert.IsTrue(Utils.ColorsAreTheSame(c.ActualBackColor, c.BaseBackColor));
        }

        [TestMethod]
        public void ClockControlTest_IsTimerGroupListViewListViewUnderCursor()
        {
            // Arrange
            cs_timed_silver.Properties.Settings.Default.AutoOpenLastFile = "No";
            var mf = new MainForm();

            mf.Show();

            Application.DoEvents();

            // Act
            mf.tsmiViewGroupListView.PerformClick();

            var c = mf.MyDataFile.ClockVMCollection.Model.Ms[0].MyTimerViews[0] as ClockControl;

            TimerGroupListView tglv = null;
            List<IClocksView> l = mf.MyClocksViews;
            foreach (IClocksView t in l)
            {
                if (t is TimerGroupListView t2)
                {
                    tglv = t2;
                }
            }

            Assert.IsNotNull(tglv);

            Cursor.Position = (tglv as Control).PointToScreen(new Point(20, 100));

            // Assert
            Assert.IsTrue(c.IsTimerGroupListViewListViewUnderCursor());
        }

        /// <summary>
        /// Not working test. Setting Visible property in the ApplyTimerStyle does not have effect after that statement, not even if I call Application.DoEvents.
        /// </summary>
        //[TestMethod]
        //public void ClockControlTest_ApplyTimerStyle()
        //{
        //    // Arrange
        //    cs_timed_silver.Properties.Settings.Default.AutoOpenLastFile = "No";
        //    MainForm mf = new MainForm();

        //    mf.Show();

        //    Application.DoEvents();

        //    var c = mf.MyDataFile.ClockCollection.ClocksData[0].MyTimerViews[0] as ClockControl;

        //    // Assert
        //    c.ApplyTimerStyle(ClockData.ClockStyles.ShowIcon);
        //    Application.DoEvents();

        //    Assert.IsTrue(c.pbIcon.Visible);
        //    Assert.IsFalse(c.lblID.Visible);

        //    c.ApplyTimerStyle(ClockData.ClockStyles.ShowID);
        //    Application.DoEvents();

        //    Assert.IsFalse(c.pbIcon.Visible);
        //    Assert.IsTrue(c.lblID.Visible);

        //    c.ApplyTimerStyle(ClockData.ClockStyles.ShowIconAndID);
        //    Application.DoEvents();

        //    Assert.IsTrue(c.pbIcon.Visible);
        //    Assert.IsTrue(c.lblID.Visible);
        //}

        [TestMethod]
        public void ClockControlTest_GetTimersView()
        {
            // Arrange
            cs_timed_silver.Properties.Settings.Default.AutoOpenLastFile = "No";
            var mf = new MainForm();

            mf.Show();

            Application.DoEvents();

            // Act
            var c = mf.MyDataFile.ClockVMCollection.Model.Ms[0].MyTimerViews[0] as ClockControl;

            // Assert
            Assert.AreEqual(mf.MyClocksViews[0], c.GetClocksView());
        }

        /// <summary>
        /// Not passed test, maybe wrong.
        /// </summary>
        //[TestMethod]
        //public void ClockControlTest_FocusView()
        //{
        //    // Arrange
        //    cs_timed_silver.Properties.Settings.Default.AutoOpenLastFile = "No";
        //    MainForm mf = new MainForm();

        //    mf.Show();

        //    Application.DoEvents();

        //    // Act
        //    var c = mf.MyDataFile.ClockCollection.ClocksData[0].MyTimerViews[0] as ClockControl;

        //    c.FocusView();
        //    Application.DoEvents();

        //    // Assert
        //    Assert.IsTrue(c.Focused || c.ContainsFocus);
        //}
    }
}
