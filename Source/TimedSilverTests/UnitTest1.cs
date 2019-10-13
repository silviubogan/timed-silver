using cs_timed_silver;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Xml;

namespace TimedSilverTests
{
    // TODO: break UnitTest1 in multiple test classes (after the Form/Control/class they test), for easier find
    [TestClass]
    public class UnitTest1
    {
        /// <summary>
        /// DataFile.MoveTimerFromIndexToIndex reaction in List view.
        /// </summary>
        [TestMethod]
        public void TestMethod2()
        {
            // Arrange
            cs_timed_silver.Properties.Settings.Default.AutoOpenLastFile = "No";
            var mf = new MainForm();
            mf.Show();
            Application.DoEvents();

            // Act
            var td = new TimerData(mf.MyDataFile, mf.MultiAudioPlayer)
            {
                Tag = "test tag 1"
            };
            mf.MyDataFile.ClockVMCollection.Model.AddClock(td);

            Application.DoEvents();

            mf.MyDataFile.ClockVMCollection.Model.MoveClockFromIndexToIndex(0, 1);

            // these ClockData-s are taken in the order after the move above
            ClockM td1 = mf.MyDataFile.ClockVMCollection.Model.Ms[0]
                as TimerData;
            ClockM td2 = mf.MyDataFile.ClockVMCollection.Model.Ms[1]
                as TimerData;


            td2.Tag = "test tag 2";

            Application.DoEvents();

            // Assert
            Assert.AreEqual("test tag 2", td2.Tag);
            Assert.AreEqual("test tag 1", td1.Tag);
        }

        /// <summary>
        /// tsmiFocusListView click
        /// </summary>
        [TestMethod]
        public void TestMethod3()
        {
            // Arrange
            cs_timed_silver.Properties.Settings.Default.AutoOpenLastFile = "No";
            var mf = new MainForm();
            mf.Show();

            // Act
            Application.DoEvents();
            mf.tsmiFocusListView_Click(null, EventArgs.Empty);
            while (!mf.MyClocksViewProvider.GetExistingOrNewClockListView().CanFocus)
            {
                Application.DoEvents();
            }

            // Assert
            Assert.AreEqual(mf.MyClocksViewProvider.GetExistingOrNewClockListView(), mf.RootSplitterView.c1);
            Assert.IsTrue(mf.MyClocksViewProvider.GetExistingOrNewClockListView().HasFocus);
            Assert.IsTrue(mf.MyClocksViewProvider.GetExistingOrNewClockListView().HasFocus);
        }

        /// <summary>
        /// MainForm.MyDataFile.IsDirty should be false on form load.
        /// </summary>
        [TestMethod]
        public void TestMethod4()
        {
            // Arrange
            cs_timed_silver.Properties.Settings.Default.AutoOpenLastFile = "No";
            var mf = new MainForm();
            mf.Show();

            // Act
            Application.DoEvents();

            // Assert
            Assert.IsFalse(mf.MyDataFile.IsUnsaved);
        }

        /// <summary>
        /// Loading an XML file after form construction, should result in a non-dirty state.
        /// Also, the loaded base file name should be correct.
        /// </summary>
        [TestMethod]
        public void TestMethod5()
        {
            // Arrange
            cs_timed_silver.Properties.Settings.Default.AutoOpenLastFile = "No";
            var mf = new MainForm();
            mf.Show(); // LoadFromFile below does not work if mf is not loaded (shown)
            mf.MyDataFile.LoadFromFile("test.xml");

            // Act
            Application.DoEvents();

            // Assert
            Assert.AreEqual("test.xml", Utils.BaseFileNameInPath(mf.MyDataFile.FilePath));
            Assert.IsFalse(mf.MyDataFile.IsUnsaved);
        }

        /// <summary>
        /// Open a file, change list view zoom level, save once, and IsDirty of DataFile
        /// should be false.
        /// </summary>
        [TestMethod]
        public void TestMethod6()
        {
            // Arrange
            cs_timed_silver.Properties.Settings.Default.AutoOpenLastFile = "No";
            var mf = new MainForm();
            mf.Show();
            mf.MyDataFile.LoadFromFile("test.xml");

            Application.DoEvents();

            mf.tsListViewZoom.SetZoomPercentIfValid((decimal)mf.MyDataFile.GetValue("GlobalZoomPercent") + 1M);

            Application.DoEvents();
            
            mf.Save();

            // Act
            Application.DoEvents();

            // Assert
            Assert.IsFalse(mf.MyDataFile.IsUnsaved);

            // Finally
            mf.Close();

            cs_timed_silver.Properties.Settings.Default.AutoOpenLastFile = "No";
            mf = new MainForm();
            mf.Show();
            mf.MyDataFile.LoadFromFile("test.xml");

            Application.DoEvents();

            mf.tsListViewZoom.SetZoomPercentIfValid((decimal)mf.MyDataFile.GetValue("GlobalZoomPercent") - 1M);

            Application.DoEvents();

            mf.Save();
        }

        /// <summary>
        /// NOTE: jump over this test, others and new could be infinitely more useful.
        /// 
        /// Do zoom out on the table, and then check if the text in the Tag column is not clipped out.
        /// NOTE: unable to pass this test, the Padding prop of the Row Headers does not works
        /// completely in my understanding.
        /// </summary>
        //[TestMethod]
        //public void TestMethod7()
        //{
        //    // Arrange
        //    cs_timed_silver.Properties.Settings.Default.AutoOpenLastFile = "No";
        //    var mf = new MainForm();
        //    mf.Show();
        //    mf.MyDataFile.LoadFromFile("test.xml");
        //    mf.SelectedViewType = EasyViewType.DataGrid;

        //    Application.DoEvents();

        //    mf.tsDataGridZoom.SetZoomPercentIfValid(75);

        //    // Act
        //    Application.DoEvents();

        //    // Assert
        //    int ph = mf.timerDataGridView1.Rows[0].GetPreferredHeight(0, DataGridViewAutoSizeRowMode.AllCells, false);
        //    int ch = mf.timerDataGridView1.Rows[0].Height;
        //    Assert.IsTrue(ch >= ph, "Current row height {0} should pe greater than or equal to  preferred row height {1}.", ch, ph);
        //}

        /// <summary>
        ///  horizontal split view selected from View menu means
        ///  timerListView1 & timerDataGridView1 are visible
        /// </summary>
        [TestMethod]
        public void TestMethod8()
        {
            // Arrange
            cs_timed_silver.Properties.Settings.Default.AutoOpenLastFile = "No";
            var mf = new MainForm();
            mf.Show();

            Application.DoEvents();

            mf.SelectedViewType = EasyViewType.HorizontalSplit;

            // Act
            Application.DoEvents();

            // Assert
            Assert.AreEqual(mf.MyClocksViewProvider.GetExistingOrNewClockListView(), mf.RootViewData.GetFirstChild().View, "New timer list view is created instead of using existing one.");
            Assert.IsTrue(mf.MyClocksViewProvider.GetExistingOrNewClockListView().Visible, "First timer list view is not visible.");
            Assert.IsTrue(mf.MyClocksViewProvider.GetExistingOrNewClockDataGridView().Visible, "First timer data grid view is not visible.");
        }

        /// <summary>
        /// Reset zoom, increase zoom 10%, then the factor should be 1.1 (110%).
        /// </summary>
        [TestMethod]
        public void TestMethod9()
        {
            // Arrange
            cs_timed_silver.Properties.Settings.Default.AutoOpenLastFile = "No";
            var mf = new MainForm();
            mf.Show();

            Application.DoEvents();
            mf.tsListViewZoom.Visible = true;
            Application.DoEvents();
            mf.tsListViewZoom.tsbZoomReset.PerformClick();
            Application.DoEvents();

            // a sort of PerformClick because of the MouseDown
            // handler that cancels the normal click
            mf.tsListViewZoom.ChangeZoomOnButtonClick(0.1M);
            Application.DoEvents();

            // Act
            Application.DoEvents();

            // Assert
            Assert.AreEqual(110, mf.MyClocksViewProvider.GetExistingOrNewClockListView().ZoomPercent, $"Expected zoom 110% and actual zoom is: {mf.MyClocksViewProvider.GetExistingOrNewClockListView().ZoomPercent}%.");
        }

        /// <summary>
        /// 
        /// </summary>
        [TestMethod]
        public void TestMethod10()
        {
            // Arrange
            cs_timed_silver.Properties.Settings.Default.AutoOpenLastFile = "No";
            var mf = new MainForm();
            mf.Show();

            Application.DoEvents();

            // Act
            Application.DoEvents();

            // Assert
            Assert.IsFalse(mf.tsmiNew.Enabled, $"No file opened and no change but the New (file) menu item is enabled.");
        }

        /// <summary>
        /// NOTE: this test is for when SettingsForm will be non-modal.
        /// </summary>
        //[TestMethod]
        //public void TestMethod11()
        //{
        //    // Arrange
        //    cs_timed_silver.Properties.Settings.Default.AutoOpenLastFile = "No";
        //    MainForm mf = new MainForm();
        //    mf.Show();

        //    Application.DoEvents();

        //    // Act
        //    mf.tsmiSettings.PerformClick();

        //    Application.DoEvents();

        //    // Assert
        //    Assert.IsTrue(mf.MySettingsForm.Visible, $"Clicked on Settings but the form is not visible.");
        //}

        /// <summary>
        /// by showing the settings form as non-modal, test if setting the file
        /// in its audio file chooser results in the correct text displayed by
        /// the label.
        /// </summary>
        [TestMethod]
        public void TestMethod12()
        {
            // Arrange
            cs_timed_silver.Properties.Settings.Default.AutoOpenLastFile = "No";
            var mf = new MainForm();
            mf.Show();

            Application.DoEvents();

            // Act
            var sf = new SettingsForm(mf);
            sf.Show();
            sf.audioFileChooser1.SetAudioPath("test inexistent file path.wav");

            Application.DoEvents();

            // Assert
            Assert.AreEqual("Default sound", sf.audioFileChooser1.lblAudio.Text,
                "Label text for default sound is wrong.");
        }

        /// <summary>
        /// Must set the MainFormRectangle setting manually because MainForm does not reach that point in execution in which it remembers its bounds.
        /// 
        /// OBSOLETE:
        /// by showing the settings form as non-modal, test if setting the file
        /// in its audio file chooser results in the correct text displayed by
        /// the label.
        /// </summary>
        [TestMethod]
        public void TestMethod13()
        {
            // Arrange
            var doc = new XmlDocument();
            doc.AppendChild(doc.CreateElement("test"));
            doc.DocumentElement.SetAttribute("MainFormRectangle", "100|200|300|400");

            cs_timed_silver.Properties.Settings.Default.AutoOpenLastFile = "No";
            var mf = new MainForm();
            mf.Show();

            mf.MyDataFile.Settings.LoadRectangleAttribute(doc, "MainFormRectangle", Rectangle.Empty);

            mf.MyDataFile.SaveAs("test2.xml");
            DataFile df = mf.MyDataFile;
            

            var mf2 = new MainForm();
            mf2.Show();
            mf2.MyDataFile.LoadFromFile("test2.xml");
            DataFile df2 = mf.MyDataFile;

            // Act
            Application.DoEvents();

            // Assert
            Assert.AreEqual(df.Settings, df2.Settings,
                "New file and saved new file have different settings.");
        }

        /// <summary>
        /// set the program to start with windows, then check if set correctly
        /// </summary>
        [TestMethod]
        public void TestMethod14()
        {
            // Arrange
            cs_timed_silver.Properties.Settings.Default.AutoOpenLastFile = "No";
            var mf = new MainForm();
            mf.Show();
            Utils.SetStartProgramWithWindows(true);

            // Act
            Application.DoEvents();

            // Assert
            Assert.IsTrue(Utils.ProgramStartsWithWindows(),
                "Setting the program to start with Windows does not work.");

            // Finally
            Utils.SetStartProgramWithWindows(false);
        }

        /// <summary>
        /// set the program to show the group list, then check its visibility
        /// </summary>
        [TestMethod]
        public void TestMethod15()
        {
            // Arrange
            cs_timed_silver.Properties.Settings.Default.AutoOpenLastFile = "No";
            var mf = new MainForm();
            mf.Show();
            mf.tsmiViewGroupListView.PerformClick();

            // Act
            Application.DoEvents();

            // Assert
            Assert.IsTrue(mf.tsmiViewGroupListView.Checked,
                "<View group list> menu item should be checked.");
            Assert.IsTrue(mf.MyClocksViewProvider.GetExistingOrNewClockGroupListView().Visible,
                "Timer group list view should be visible.");
        }

        /// <summary>
        /// set the program to show easy view "data grid", then check the visibility of data grid
        /// </summary>
        [TestMethod]
        public void TestMethod16()
        {
            // Arrange
            cs_timed_silver.Properties.Settings.Default.AutoOpenLastFile = "No";
            var mf = new MainForm();
            mf.Show();
            mf.tsmiDataGridView.PerformClick();

            // Act
            Application.DoEvents();

            // Assert
            Assert.IsTrue(mf.tsmiDataGridView.Checked,
                "<Data grid> menu item should be checked.");
            Assert.IsTrue(mf.MyClocksViewProvider.GetExistingOrNewClockDataGridView().Visible,
                "Timer data grid view should be visible.");
        }

        ///// <summary>
        ///// open a file with at least one clock, set its tag like a user
        ///// and check the dirty state of the file (should be true).
        ///// </summary>
        //[TestMethod]
        //public void TestMethod17()
        //{
        //    // Arrange
        //    cs_timed_silver.Properties.Settings.Default.AutoOpenLastFile = "No";
        //    var mf = new MainForm();
        //    mf.Show();
        //    mf.MyDataFile.LoadFromFile("test.xml");

        //    // Act
        //    Application.DoEvents();

        //    IClockView v = mf.MyDataFile.ClockCollection.
        //        ClocksData[0].MyTimerViews[0];
        //    var c = v as ClockControl;
        //    c.etbTag.Select();

        //    Application.DoEvents();

        //    var r = new Random();

        //    c.etbTag.InnerTextBox.Text = "new tag " + r.Next();

        //    Application.DoEvents();

        //    // Assert
        //    Assert.IsTrue(mf.MyDataFile.ClockCollection.IsUnsaved,
        //        "The clock collection should be dirty.");
        //    Assert.IsTrue(mf.MyDataFile.IsUnsaved,
        //        "The data file (the clock collection in it, exactly) should be dirty.");
        //}
    }
}
