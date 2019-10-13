using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;
using cs_timed_silver;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TimedSilverTests
{
    [TestClass]
    public class DataFileUnitTest
    {
        [TestMethod]
        public void DataFileTest_IsUnsaved_change()
        {
            cs_timed_silver.Properties.Settings.Default.AutoOpenLastFile = "No";
            var mf = new MainForm();
            mf.Show();

            //string s = "";
            //foreach (KeyValuePair<string, SettingData> d in mf.MyDataFile.Settings)
            //{
            //    if ((d.Value.Value == null &&
            //        d.Value.DefaultValue != null) ||
            //        (d.Value.Value != null &&
            //        d.Value.DefaultValue == null))
            //    {

            //    }
            //    s += d.Key + ": " + d.Value.Value + " != " + d.Value.DefaultValue + "\n";
            //}


            Assert.IsTrue(!mf.MyDataFile.IsUnsaved);

            mf.MyDataFile.IsUnsaved = true;

            Assert.IsTrue(mf.MyDataFile.ClockVMCollection.Model.IsUnsaved);
            Assert.IsTrue(mf.MyDataFile.Settings.IsUnsaved);
        }

        [TestMethod]
        public void DataFileTest_SettingsAreUnsaved_change()
        {
            var mf = new MainForm();
            mf.Show();

            Assert.IsTrue(!mf.MyDataFile.IsUnsaved);

            mf.MyDataFile.SettingsAreUnsaved = true;

            Assert.IsTrue(mf.MyDataFile.IsUnsaved);
        }

        [TestMethod]
        public void DataFileTest_UpdateAutoSaveTimerInterval()
        {
            var mf = new MainForm();
            mf.Show();

            mf.MyDataFile.SetValue("AutosaveEnabled", true);
            mf.MyDataFile.SetValue("AutosaveEvery", TimeSpan.FromSeconds(30));

            // Automatically called:
            //mf.MyDataFile.UpdateAutoSaveTimerInterval();

            Assert.IsTrue(mf.MyDataFile.AutoSaveTimerIsEnabled);
            Assert.AreEqual(30 * 1000, mf.MyDataFile.AutoSaveTimerInterval);
        }

        [TestMethod]
        public void DataFileTest_UpdateAutoSaveTimerInterval_2()
        {
            var mf = new MainForm();
            mf.Show();
            
            mf.MyDataFile.UpdateAutoSaveTimerInterval(true, 30 * 1000);

            Assert.IsTrue(mf.MyDataFile.AutoSaveTimerIsEnabled);
            Assert.AreEqual(30 * 1000, mf.MyDataFile.AutoSaveTimerInterval);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void DataFileTest_AutoSaveTimerInterval_change()
        {
            var mf = new MainForm();
            mf.Show();

            mf.MyDataFile.AutoSaveTimerInterval = 0;
        }

        public class MockSaveable : ISaveable
        {
            internal bool SaveDone = false;

            public bool Save()
            {
                SaveDone = true;

                return true; // not relevant
            }
        }

        [TestMethod]
        public void DataFileTest_DoAutoSave()
        {
            var mf = new MainForm();
            mf.Show();
            
            var s = new MockSaveable();
            mf.MyDataFile.SetValue("AutosaveEnabled", true);
            mf.MyDataFile.DoAutoSave(s);

            Assert.IsTrue(s.SaveDone);
        }

        [TestMethod]
        public void DataFileTest_LoadFromString()
        {
            var mf = new MainForm();
            mf.Show();

            mf.MyDataFile.LoadFromString(File.ReadAllText("test.xml"));

            Assert.AreEqual(2, mf.MyDataFile.ClockVMCollection.Model.Groups.Count);
            Assert.AreEqual(12, mf.MyDataFile.ClockVMCollection.Model.Ms.Count);
            Assert.AreEqual(98M,
                (decimal)mf.MyDataFile.GetValue("GroupListZoomPercent"));
            Assert.AreEqual("", mf.MyDataFile.ErrorString);
        }

        [TestMethod]
        public void DataFileTest_LoadFromString_2()
        {
            var mf = new MainForm();
            mf.Show();

            bool success = mf.MyDataFile.LoadFromString("sdadasfsdfa");

            Assert.IsFalse(success);
            Assert.AreNotEqual("", mf.MyDataFile.ErrorString);
        }

        [TestMethod]
        public void DataFileTest_LoadFromString_3()
        {
            var mf = new MainForm();
            mf.Show();

            bool success = mf.MyDataFile.LoadFromString("");

            Assert.IsFalse(success);
            Assert.AreNotEqual("", mf.MyDataFile.ErrorString);
        }

        internal static string GetOnePixelBmpBase64()
        {
            return "Qk06AAAAAAAAADYAAAAoAAAAAQAAAAEAAAABACAAAAAAAAAAAADEDgAAxA4AAAAAAAAAAAAATbEk/w==";
        }

        internal static XmlElement CreateTestAlarmClockXmlElement()
        {
            var doc = new XmlDocument();
            var el = doc.CreateElement("Clock");
            el.SetAttribute("Type", "Alarm");
            el.SetAttribute("Tag", "abcdefghijklmnopqrstuvwxyz");
            el.SetAttribute("UserBackColor", "#0055FF00");
            el.SetAttribute("ResetToValueLocked", "False");
            el.SetAttribute("Icon", GetOnePixelBmpBase64());
            el.SetAttribute("GroupName", "gg");
            el.SetAttribute("Style", "ShowIconAndID");

            return el;
        }

        internal static XmlElement CreateTestTimerClockXmlElement()
        {
            var doc = new XmlDocument();
            var el = doc.CreateElement("Clock");
            el.SetAttribute("Type", "Timer");
            el.SetAttribute("Tag", "abcdefghijklmnopqrstuvwxyz !");
            el.SetAttribute("UserBackColor", "#0055FF00");
            el.SetAttribute("ResetToValueLocked", "True");
            el.SetAttribute("Icon", GetOnePixelBmpBase64());
            el.SetAttribute("GroupName", "gg");
            el.SetAttribute("Style", "ShowIconAndID");

            return el;
        }

        [TestMethod]
        public void DataFileTest_LoadClock()
        {
            var mf = new MainForm();
            mf.Show();
            
            var el = CreateTestAlarmClockXmlElement();

            var acm = new List<ClockM>();
            mf.MyDataFile.LoadClock(el, acm);

            ClockM cd = acm[acm.Count - 1];

            Assert.IsInstanceOfType(cd, typeof(AlarmData));
            Assert.AreEqual("abcdefghijklmnopqrstuvwxyz", cd.Tag);
            Assert.IsTrue(Utils.ColorsAreTheSame(
                Utils.StringToColor("#0055FF00"), cd.UserBackColor));
            Assert.AreEqual(false, cd.ResetToValueLocked);
            Assert.AreEqual(Utils.ImageToBase64String(cd.Icon), el.GetAttribute("Icon"));
            Assert.AreEqual("gg", cd.GroupName);
            Assert.AreEqual(ClockM.ClockStyles.ShowIconAndID, cd.Style);

            Assert.AreEqual("", mf.MyDataFile.ErrorString);
        }

        [TestMethod]
        public void DataFileTest_InitializeAlarm()
        {
            var mf = new MainForm();
            mf.Show();

            var el = CreateTestAlarmClockXmlElement();
            el.SetAttribute("CurrentValue", "11/16/2019 12:00:00");
            el.SetAttribute("ResetToValue", "11/16/2018 12:00:00");
            var ad = mf.MyDataFile.InitializeAlarm(el) as AlarmData;

            Assert.AreEqual(new DateTime(2019, 11, 16, 12, 0, 0), ad.CurrentValue);
            Assert.AreEqual(new DateTime(2018, 11, 16, 12, 0, 0), ad.ResetToValue);

            Assert.AreEqual("", mf.MyDataFile.ErrorString);
        }

        [TestMethod]
        public void DataFileTest_InitializeTimer()
        {
            var mf = new MainForm();
            mf.Show();
            
            var el = CreateTestTimerClockXmlElement();
            el.SetAttribute("CurrentValue", "12:00:00");
            el.SetAttribute("ResetToValue", "10:00:00");
            var td = mf.MyDataFile.InitializeTimer(el) as TimerData;

            Assert.AreEqual(new TimeSpan(12, 0, 0), td.CurrentValue);
            Assert.AreEqual(new TimeSpan(10, 0, 0), td.ResetToValue);

            Assert.AreEqual("", mf.MyDataFile.ErrorString);
        }

        [TestMethod]
        public void DataFileTest_LoadBeepsDataTable_XmlElement()
        {
            var mf = new MainForm();
            mf.Show();

            var d = new XmlDocument();
            d.LoadXml(@"	<BeepsDataTable>
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
            mf.MyDataFile.LoadBeepsDataTable(d.DocumentElement);

            Assert.AreEqual(mf.MyDataFile.GetBeepsDataTable(),  d.DocumentElement.InnerXml);

            Assert.AreEqual("", mf.MyDataFile.ErrorString);
        }

        [TestMethod]
        public void DataFileTest_LoadBeepsDataTable_string()
        {
            var mf = new MainForm();
            mf.Show();

            var d = new XmlDocument();
            d.LoadXml(@"	<BeepsDataTable>
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
            mf.MyDataFile.LoadBeepsDataTable(d.DocumentElement.InnerXml);

            Assert.AreEqual(mf.MyDataFile.GetBeepsDataTable(), d.DocumentElement.InnerXml);

            Assert.AreEqual("", mf.MyDataFile.ErrorString);
        }

        [TestMethod]
        public void DataFileTest_LoadBeepsDataTable_BeepDataTable()
        {
            var mf = new MainForm();
            mf.Show();

            var d = new XmlDocument();
            d.PreserveWhitespace = false;
            d.LoadXml(@"	<BeepsDataTable>
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

            var dt = new BeepDataTable();
            dt.LoadFromString(d.DocumentElement.InnerXml);

            mf.MyDataFile.LoadBeepsDataTable(dt);
            
            Assert.AreEqual(Utils.RemoveWhitespaceFromXML(mf.MyDataFile.GetBeepsDataTable()),
                d.DocumentElement.InnerXml);

            Assert.AreEqual("", mf.MyDataFile.ErrorString);
        }

        [TestMethod]
        public void DataFileTest_LoadGroup()
        {
            var mf = new MainForm();
            mf.Show();

            var d = new XmlDocument();
            d.LoadXml($"<Group Name=\"asd\" Icon=\"{GetOnePixelBmpBase64()}\"/>");

            mf.MyDataFile.LoadGroup(d.DocumentElement);

            Assert.IsTrue(mf.MyDataFile.ClockVMCollection.Model.Groups.Contains("asd"));
            Assert.IsTrue(mf.MyDataFile.ClockVMCollection.Model.Groups.HasIcon("asd"));

            Assert.IsFalse(mf.MyDataFile.ClockVMCollection.Model.Groups.Contains("abc"));
            Assert.IsFalse(mf.MyDataFile.ClockVMCollection.Model.Groups.HasIcon("abc"));

            Assert.AreEqual("", mf.MyDataFile.ErrorString);
        }

        [TestMethod]
        public void DataFileTest_Close()
        {
            var mf = new MainForm();
            mf.Show();

            mf.MyDataFile.LoadFromFile("test2.xml");
            mf.MyDataFile.Close();

            var mf2 = new MainForm();
            mf2.Show();

            bool eq = mf.MyDataFile == mf2.MyDataFile;

            Assert.IsTrue(eq);
        }

        [TestMethod]
        public void DataFileTest_UpdateGlobalIsDirty()
        {
            var mf = new MainForm();
            mf.Show();

            mf.MyDataFile.LoadFromFile("test2.xml");
            mf.MyDataFile.AvailableToUpdateOrApplyGlobalIsUnsaved = true;
            
            mf.MyDataFile.Settings.IsUnsaved = true;
            mf.MyDataFile.ClockVMCollection.Model.IsUnsaved = true;
            mf.MyDataFile.BeepsDataTableIsUnsaved = true;

            mf.MyDataFile.UpdateGlobalIsUnsaved();

            Assert.IsTrue(mf.MyDataFile.IsUnsaved);

            mf.MyDataFile.Settings.IsUnsaved = true;
            mf.MyDataFile.ClockVMCollection.Model.IsUnsaved = false;
            mf.MyDataFile.BeepsDataTableIsUnsaved = false;

            mf.MyDataFile.UpdateGlobalIsUnsaved();

            Assert.IsTrue(mf.MyDataFile.IsUnsaved);

            mf.MyDataFile.Settings.IsUnsaved = false;
            mf.MyDataFile.ClockVMCollection.Model.IsUnsaved = true;
            mf.MyDataFile.BeepsDataTableIsUnsaved = false;

            mf.MyDataFile.UpdateGlobalIsUnsaved();

            Assert.IsTrue(mf.MyDataFile.IsUnsaved);

            mf.MyDataFile.Settings.IsUnsaved = false;
            mf.MyDataFile.ClockVMCollection.Model.IsUnsaved = false;
            mf.MyDataFile.BeepsDataTableIsUnsaved = true;

            mf.MyDataFile.UpdateGlobalIsUnsaved();

            Assert.IsTrue(mf.MyDataFile.IsUnsaved);

            mf.MyDataFile.Settings.IsUnsaved = false;
            mf.MyDataFile.ClockVMCollection.Model.IsUnsaved = false;
            mf.MyDataFile.BeepsDataTableIsUnsaved = false;

            mf.MyDataFile.UpdateGlobalIsUnsaved();

            Assert.IsFalse(mf.MyDataFile.IsUnsaved);
        }

        [TestMethod]
        public void DataFileTest_LoadFromFile()
        {
            var mf = new MainForm();
            mf.Show();

            mf.MyDataFile.LoadFromFile("test2.xml");

            Assert.AreEqual("test2.xml", Utils.BaseFileNameInPath(mf.MyDataFile.FilePath));

            Assert.IsFalse(mf.MyDataFile.IsUnsaved);
        }

        [TestMethod]
        public void DataFileTest_Save()
        {
            var mf = new MainForm();
            mf.Show();

            mf.MyDataFile.LoadFromFile("test2.xml");

            DateTime dt = DateTime.Now;

            mf.MyDataFile.Save();

            Assert.IsTrue(File.GetLastWriteTime("test2.xml") >= dt);
        }

        [TestMethod]
        public void DataFileTest_SaveAs()
        {
            var mf = new MainForm();
            mf.Show();

            mf.MyDataFile.LoadFromFile("test2.xml");

            DateTime dt = DateTime.Now;

            mf.MyDataFile.SaveAs("delete_me.xml");

            Assert.AreEqual("delete_me.xml", Utils.BaseFileNameInPath(mf.MyDataFile.FilePath));
            Assert.IsFalse(mf.MyDataFile.IsUnsaved);

            File.Delete("delete_me.xml");
        }

        [TestMethod]
        public void DataFileTest_ResetAll()
        {
            var mf = new MainForm();
            mf.Show();

            var d = new XmlDocument();
            d.LoadXml(@"	<BeepsDataTable>
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
            mf.MyDataFile.LoadBeepsDataTable(d.DocumentElement.InnerXml);

            mf.MyDataFile.ClockVMCollection.Model.AddClock(new TimerData(mf.MyDataFile, mf.MultiAudioPlayer));

            mf.MyDataFile.Settings["MainFormWindowState"].Value = "Minimized";

            mf.MyDataFile.ResetAll();

            Assert.AreEqual("<Beeps></Beeps>", mf.MyDataFile.GetBeepsDataTable());
            Assert.AreEqual(0, mf.MyDataFile.ClockVMCollection.Model.Ms.Count);
            Assert.AreEqual("Normal", mf.MyDataFile.Settings["MainFormWindowState"].Value);
        }

        [TestMethod]
        public void DataFileTest_CloseAndOpenOtherFile()
        {
            var mf = new MainForm();
            mf.Show();

            mf.MyDataFile.LoadFromFile("test2.xml");

            Assert.IsFalse(mf.MyDataFile.IsUnsaved);

            mf.MyDataFile.CloseAndOpenOtherFile("test.xml");

            Assert.IsFalse(mf.IsFileBeingClosed);
        }

        [TestMethod]
        public void DataFileTest_Equals()
        {
            var mf = new MainForm();
            mf.Show();

            mf.MyDataFile.LoadFromFile("test2.xml");

            var mf2 = new MainForm();
            mf2.Show();

            mf2.MyDataFile.LoadFromFile("test2.xml");

            Assert.AreEqual(mf.MyDataFile, mf.MyDataFile);

            mf2 = new MainForm();
            mf2.Show();

            mf2.MyDataFile.LoadFromFile("test.xml");

            Assert.AreNotEqual(mf2.MyDataFile, mf.MyDataFile);
        }

        [TestMethod]
        public void DataFileTest_SetValueWithoutApply()
        {
            var mf = new MainForm();
            mf.Show();

            mf.MyDataFile.LoadFromFile("test2.xml");

            mf.MyDataFile.SetWithoutApply = false;
            mf.MyDataFile.SetValueWithoutApply("AutosaveEnabled", true);
            Assert.IsFalse(mf.MyDataFile.SetWithoutApply);

            mf.MyDataFile.SetWithoutApply = true;
            mf.MyDataFile.SetValueWithoutApply("AutosaveEnabled", false);
            Assert.IsTrue(mf.MyDataFile.SetWithoutApply);
        }
    }
}
