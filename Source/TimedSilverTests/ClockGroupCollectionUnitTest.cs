using System;
using System.Drawing;
using cs_timed_silver;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TimedSilverTests
{
    [TestClass]
    public class ClockGroupCollectionUnitTest
    {
        [TestMethod]
        public void ClockGroupCollectionTest_Add()
        {
            var mf = new MainForm();
            var df = mf.MyDataFile;
            var d = new ClockVMCollection.Model(df);
            ClockGroupMCollection c = d.Groups;

            Assert.IsTrue(c.Add("g1"));
            Assert.IsFalse(c.Add("g1"));
            Assert.IsTrue(c.Contains("g1"));
            Assert.IsTrue(d.IsUnsaved);
        }

        [TestMethod]
        public void ClockGroupCollectionTest_Clear()
        {
            var mf = new MainForm();
            var df = mf.MyDataFile;
            var d = new ClockVMCollection.Model(df);
            ClockGroupMCollection c = d.Groups;

            d.AppliedFilter = new FilterM(d);

            var cl = new TimerData(df, mf.MultiAudioPlayer)
            {
                GroupName = "g1"
            };
            d.AddClock(cl);

            Assert.IsFalse(c.Add("g1"));
            Assert.IsTrue(c.Clear());
            Assert.AreEqual(0, c.Count);
            Assert.AreEqual("", cl.GroupName);
            Assert.IsTrue(d.IsUnsaved);
        }

        [TestMethod]
        public void ClockGroupCollectionTest_ClearWithoutChangingClocks()
        {
            var mf = new MainForm();
            var df = mf.MyDataFile;
            var d = new ClockVMCollection.Model(df);
            ClockGroupMCollection c = d.Groups;

            d.AppliedFilter = new FilterM(d);

            var cl = new TimerData(df, mf.MultiAudioPlayer)
            {
                GroupName = "g1"
            };
            d.AddClock(cl);

            Assert.IsFalse(c.Add("g1"));
            Assert.IsTrue(c.ClearWithoutChangingClocks());
            Assert.AreEqual(0, c.Count);
            Assert.AreEqual("g1", cl.GroupName);
            Assert.IsTrue(d.IsUnsaved);
        }

        [TestMethod]
        public void ClockGroupCollectionTest_Contains()
        {
            var mf = new MainForm();
            var df = mf.MyDataFile;
            var d = new ClockVMCollection.Model(df);
            ClockGroupMCollection c = d.Groups;

            d.AppliedFilter = new FilterM(d);

            var cl = new TimerData(df, mf.MultiAudioPlayer)
            {
                GroupName = "g1"
            };
            d.AddClock(cl);

            Assert.IsFalse(c.Add("g1"));
            Assert.IsTrue(c.Contains("g1"));
            Assert.IsFalse(c.Contains("g2"));
            Assert.AreEqual(1, c.Count);
        }

        [TestMethod]
        public void ClockGroupCollectionTest_Remove()
        {
            var mf = new MainForm();
            var df = mf.MyDataFile;
            var d = new ClockVMCollection.Model(df);
            ClockGroupMCollection c = d.Groups;

            d.AppliedFilter = new FilterM(d);

            var cl = new TimerData(df, mf.MultiAudioPlayer)
            {
                GroupName = "g1"
            };
            d.AddClock(cl);

            Assert.IsTrue(c.Remove("g1"));
            Assert.IsFalse(c.Contains("g1"));
            Assert.IsTrue(!c.Icons.ContainsKey("g1") ||
                c.Icons["g1"] == null);
            Assert.AreEqual("", cl.GroupName);
        }

        [TestMethod]
        public void ClockGroupCollectionTest_Rename()
        {
            var mf = new MainForm();
            var df = mf.MyDataFile;
            var d = new ClockVMCollection.Model(df);
            ClockGroupMCollection c = d.Groups;

            d.AppliedFilter = new FilterM(d);

            var cl = new TimerData(df, mf.MultiAudioPlayer)
            {
                GroupName = "g1"
            };
            d.AddClock(cl);

            Assert.IsTrue(c.Rename("g1", "g2"));
            Assert.IsFalse(c.Contains("g1"));
            Assert.IsTrue(c.Contains("g2"));
        }

        [TestMethod]
        public void ClockGroupCollectionTest_Move()
        {
            var mf = new MainForm();
            var df = mf.MyDataFile;
            var d = new ClockVMCollection.Model(df);
            ClockGroupMCollection c = d.Groups;

            d.AppliedFilter = new FilterM(d);

            var cl = new TimerData(df, mf.MultiAudioPlayer)
            {
                GroupName = "g1"
            };
            d.AddClock(cl);

            c.Add("g2");
            c.Add("g3");

            c.Move("g1", 2);

            Assert.AreEqual(c.GroupNames[0], "g2");
            Assert.AreEqual(c.GroupNames[1], "g3");
            Assert.AreEqual(c.GroupNames[2], "g1");
        }

        [TestMethod]
        public void ClockGroupCollectionTest_ClearGroup()
        {
            var mf = new MainForm();
            var df = mf.MyDataFile;
            var d = new ClockVMCollection.Model(df);
            ClockGroupMCollection c = d.Groups;

            d.AppliedFilter = new FilterM(d);

            var td1 = new TimerData(df, mf.MultiAudioPlayer)
            {
                GroupName = "g1"
            };
            var td2 = new TimerData(df, mf.MultiAudioPlayer)
            {
                GroupName = "g2"
            };
            var td3 = new TimerData(df, mf.MultiAudioPlayer)
            {
                GroupName = "g2"
            };
            d.AddClocks(td1, td2, td3);

            c.ClearGroup("g2");

            Assert.AreEqual("g1", td1.GroupName);
            Assert.AreEqual("", td2.GroupName);
            Assert.AreEqual("", td3.GroupName);
        }

        [TestMethod]
        public void ClockGroupCollectionTest_SetIcon()
        {
            var mf = new MainForm();
            var df = mf.MyDataFile;
            var d = new ClockVMCollection.Model(df);
            ClockGroupMCollection c = d.Groups;

            d.AppliedFilter = new FilterM(d);

            var cl = new TimerData(df, mf.MultiAudioPlayer)
            {
                GroupName = "g1"
            };
            d.AddClock(cl);

            // Act
            c.SetIcon("g1", SystemIcons.Information.ToBitmap());

            Assert.IsTrue(c.HasIcon("g1"));
            Assert.IsNotNull(c.Icons["g1"]);
        }

        [TestMethod]
        public void ClockGroupCollectionTest_RemoveIcon()
        {
            var mf = new MainForm();
            var df = mf.MyDataFile;
            var d = new ClockVMCollection.Model(df);
            ClockGroupMCollection c = d.Groups;

            d.AppliedFilter = new FilterM(d);

            var cl = new TimerData(df, mf.MultiAudioPlayer)
            {
                GroupName = "g1"
            };
            d.AddClock(cl);

            // Act
            c.SetIcon("g1", SystemIcons.Information.ToBitmap());
            c.RemoveIcon("g1");

            Assert.IsTrue(!c.HasIcon("g1"));
        }

        [TestMethod]
        public void ClockGroupCollectionTest_Equals()
        {
            var mf = new MainForm();
            var df = mf.MyDataFile;
            var d = new ClockVMCollection.Model(df);
            ClockGroupMCollection c = d.Groups;

            var cc = new ClockGroupMCollection(d);

            d.AppliedFilter = new FilterM(d);

            // Act
            c.Add("a");
            c.Add("b");
            c.Add("c");

            cc.Add("a");
            cc.Add("b");
            cc.Add("c");
            
            Assert.IsTrue(c.Equals(cc));

            Bitmap bmp = SystemIcons.Asterisk.ToBitmap();
            c.SetIcon("b", bmp); // SetIcon creates a new image from the given image
            cc.SetIcon("b", bmp);

            Assert.IsFalse(c.Equals(cc));

            c.RemoveIcon("b");
            cc.RemoveIcon("b");

            Assert.IsTrue(c.Equals(cc));

            cc.Move("b", 2);

            Assert.IsTrue(!c.Equals(cc));
        }

        [TestMethod]
        public void ClockGroupCollectionTest_GetIcon()
        {
            var mf = new MainForm();
            var df = mf.MyDataFile;
            var d = new ClockVMCollection.Model(df);
            ClockGroupMCollection c = d.Groups;

            d.AppliedFilter = new FilterM(d);

            var cl = new TimerData(df, mf.MultiAudioPlayer)
            {
                GroupName = "g1"
            };
            d.AddClock(cl);

            // Act
            c.SetIcon("g1", SystemIcons.Information.ToBitmap());

            // Assert
            Assert.IsTrue(c.GetIcon("g1") != null);
        }

        [TestMethod]
        public void ClockGroupCollectionTest_HasIcon()
        {
            var mf = new MainForm();
            var df = mf.MyDataFile;
            var d = new ClockVMCollection.Model(df);
            ClockGroupMCollection c = d.Groups;

            d.AppliedFilter = new FilterM(d);

            var cl = new TimerData(df, mf.MultiAudioPlayer)
            {
                GroupName = "g1"
            };
            d.AddClock(cl);

            // Act
            c.SetIcon("g1", SystemIcons.Information.ToBitmap());

            // Assert
            Assert.IsTrue(c.HasIcon("g1"));
        }
    }
}
