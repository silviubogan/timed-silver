using System;
using cs_timed_silver;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TimedSilverTests
{
    [TestClass]
    public class TimerDataUnitTest
    {
        [TestMethod]
        public void TimerDataTest_SubstractSeconds()
        {
            // Arrange
            var mf = new MainForm();
            mf.Show();

            // Act
            var td = new TimerData(mf.MyDataFile, mf.MultiAudioPlayer)
            {
                CurrentTimeSpan = TimeSpan.FromSeconds(15)
            };
            td.SubstractSeconds(5);

            // Assert
            Assert.AreEqual(TimeSpan.FromSeconds(10), td.CurrentTimeSpan);
        }

        [TestMethod]
        public void TimerDataTest_GetSeconds()
        {
            // Arrange
            var mf = new MainForm();
            mf.Show();

            // Act
            var td = new TimerData(mf.MyDataFile, mf.MultiAudioPlayer)
            {
                CurrentTimeSpan = TimeSpan.FromSeconds(15.591)
            };

            // Assert
            Assert.AreEqual(15, td.GetSeconds());
        }

        [TestMethod]
        public void TimerDataTest_UpdateBeepTimers()
        {
            // Arrange
            cs_timed_silver.Properties.Settings.Default.AutoOpenLastFile = "No";
            var mf = new MainForm();
            mf.OnLoadCreateBasicViewDataWithView = false;
            mf.OnLoadUpdateViewType = false;
            mf.Show();

            var dt = new BeepDataTable();
            dt.LoadFromString(@"	<BeepsDataTable>
<DocumentElement>
  <Beeps>
    <MsBeforeRinging>400</MsBeforeRinging>
    <BeepDuration>500</BeepDuration>
    <BeepFrequency>600</BeepFrequency>
  </Beeps>
  <Beeps>
    <MsBeforeRinging>100</MsBeforeRinging>
    <BeepDuration>200</BeepDuration>
    <BeepFrequency>300</BeepFrequency>
  </Beeps>
</DocumentElement>
	</BeepsDataTable>");
            mf.MyDataFile.LoadBeepsDataTable(dt);

            // Act
            var td = new TimerData(mf.MyDataFile, mf.MultiAudioPlayer)
            {
                CurrentTimeSpan = TimeSpan.FromSeconds(15.591)
            };

            td.UpdateBeepTimers();

            Assert.AreEqual(2, td.MyBeepTimers.BeepDurations.Length);
            Assert.AreEqual(2, td.MyBeepTimers.BeepFrequecies.Length);
            Assert.AreEqual(2, td.MyBeepTimers.BeepMsBeforeRinging.Length);
            Assert.AreEqual(2, td.MyBeepTimers.BeepTimers.Length);
        }

        [TestMethod]
        public void TimerDataTest_HandleTick()
        {
            // Arrange
            cs_timed_silver.Properties.Settings.Default.AutoOpenLastFile = "No";
            var mf = new MainForm();
            mf.OnLoadCreateBasicViewDataWithView = false;
            mf.OnLoadUpdateViewType = false;
            mf.Show();

            // Act
            var td = new TimerData(mf.MyDataFile, mf.MultiAudioPlayer)
            {
                CurrentTimeSpan = TimeSpan.FromSeconds(15.591)
            };

            td.HandleTick();

            Assert.AreEqual(14, td.GetSeconds());
        }

        [TestMethod]
        public void TimerDataTest_HandleTick_2()
        {
            // Arrange
            cs_timed_silver.Properties.Settings.Default.AutoOpenLastFile = "No";
            var mf = new MainForm();
            mf.OnLoadCreateBasicViewDataWithView = false;
            mf.OnLoadUpdateViewType = false;
            mf.Show();

            // Act
            var td = new TimerData(mf.MyDataFile, mf.MultiAudioPlayer)
            {
                CurrentTimeSpan = TimeSpan.FromSeconds(1)
            };

            bool invoked = false;
            td.TimerStopped += delegate (object sender, ClockEventArgs e)
            {
                invoked = true;
            };

            td.HandleTick();

            Assert.AreEqual(0, td.GetSeconds());
            Assert.IsFalse(td.FormsTimer.Enabled);
            Assert.IsTrue(td.t2.Enabled);
            Assert.IsTrue(invoked);
        }

        [TestMethod]
        public void TimerDataTest_DestroyBeepTimers()
        {
            // Arrange
            cs_timed_silver.Properties.Settings.Default.AutoOpenLastFile = "No";
            var mf = new MainForm();
            mf.OnLoadCreateBasicViewDataWithView = false;
            mf.OnLoadUpdateViewType = false;
            mf.Show();
            
            mf.MyDataFile.LoadBeepsDataTable(@"	<BeepsDataTable>
                <DocumentElement>
                  <Beeps>
                    <MsBeforeRinging>400</MsBeforeRinging>
                    <BeepDuration>500</BeepDuration>
                    <BeepFrequency>600</BeepFrequency>
                  </Beeps>
                  <Beeps>
                    <MsBeforeRinging>100</MsBeforeRinging>
                    <BeepDuration>200</BeepDuration>
                    <BeepFrequency>300</BeepFrequency>
                  </Beeps>
                </DocumentElement>
	                </BeepsDataTable>");

            // Act
            var td = new TimerData(mf.MyDataFile, mf.MultiAudioPlayer)
            {
                CurrentTimeSpan = TimeSpan.FromSeconds(1)
            };

            td.UpdateBeepTimers();
            td.DestroyBeepTimers();

            Assert.IsNull(td.MyBeepTimers);
        }

        [TestMethod]
        public void TimerDataTest_CurrentTimeSpan_change()
        {
            // Arrange
            cs_timed_silver.Properties.Settings.Default.AutoOpenLastFile = "No";
            var mf = new MainForm();
            mf.OnLoadCreateBasicViewDataWithView = false;
            mf.OnLoadUpdateViewType = false;
            mf.Show();

            // Act
            var td = new TimerData(mf.MyDataFile, mf.MultiAudioPlayer);

            td.CurrentTimeSpan = TimeSpan.FromMinutes(15);

            Assert.AreEqual(td.CurrentTimeSpan, td.CurrentValue);
        }


        [TestMethod]
        public void TimerDataTest_CurrentTimeSpan_initial_read()
        {
            // Arrange
            cs_timed_silver.Properties.Settings.Default.AutoOpenLastFile = "No";
            var mf = new MainForm();
            mf.OnLoadCreateBasicViewDataWithView = false;
            mf.OnLoadUpdateViewType = false;
            mf.Show();

            // Act
            var td = new TimerData(mf.MyDataFile, mf.MultiAudioPlayer);

            object cv = td.CurrentTimeSpan;

            Assert.AreEqual(TimeSpan.Zero, td.CurrentValue);
        }
    }
}
