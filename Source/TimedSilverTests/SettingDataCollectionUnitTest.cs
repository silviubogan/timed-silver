using System;
using System.Drawing;
using System.Xml;
using cs_timed_silver;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TimedSilverTests
{
    [TestClass]
    public class SettingDataCollectionUnitTest
    {
        [TestMethod]
        public void SettingDataCollectionTest_ExportAsAttributes()
        {
            var mf = new MainForm();
            mf.Show();

            var c = new SettingDataMCollection(mf.MyDataFile);

            c.Add(new SettingDataM(typeof(string), "test", "val1")
            {

            }, new SettingDataM(typeof(int), "testing", 15)
            {
            });

            c.SetValue("test", "val2");

            var d = new XmlDocument();
            var e = d.CreateElement("root");
            c.ExportAsAttributes(e);

            Assert.AreEqual(e.GetAttribute("test"), "val2");
            Assert.AreEqual(e.GetAttribute("testing"), "15");
        }

        [TestMethod]
        public void SettingDataCollectionTest_SettingsCollectionsHaveEqualValues()
        {
            var mf = new MainForm();
            mf.Show();

            var c = new SettingDataMCollection(mf.MyDataFile);
            c.Add(new SettingDataM(typeof(string), "test", "val1")
            {

            }, new SettingDataM(typeof(int), "testing", 15)
            {
            });

            var c2 = new SettingDataMCollection(mf.MyDataFile);
            c2.Add(new SettingDataM(typeof(string), "test", "val1")
            {

            }, new SettingDataM(typeof(int), "testing", 15)
            {
            });

            Assert.IsTrue(SettingDataMCollection.
                SettingsCollectionsHaveEqualValues(
                    c.SettingsData, c2.SettingsData));
        }

        [TestMethod]
        public void SettingDataCollectionTest_SettingsCollectionsHaveEqualValues_2()
        {
            var mf = new MainForm();
            mf.Show();

            var c = new SettingDataMCollection(mf.MyDataFile);
            c.Add(new SettingDataM(typeof(string), "test", "val1")
            {

            }, new SettingDataM(typeof(int), "testing", 16)
            {
            });

            var c2 = new SettingDataMCollection(mf.MyDataFile);
            c2.Add(new SettingDataM(typeof(string), "test", "val1")
            {

            }, new SettingDataM(typeof(int), "testing", 15)
            {
            });

            Assert.IsFalse(SettingDataMCollection.
                SettingsCollectionsHaveEqualValues(
                    c.SettingsData, c2.SettingsData));
        }

        [TestMethod]
        public void SettingDataCollectionTest_CheckIfDirtySettingExists()
        {
            var mf = new MainForm();
            mf.Show();

            var c = new SettingDataMCollection(mf.MyDataFile);
            c.Add(new SettingDataM(typeof(string), "test", "val1")
            {

            }, new SettingDataM(typeof(int), "testing", 16)
            {
            });

            Assert.IsFalse(c.CheckIfDirtySettingExists());

            c.SetValue("test", "val1");

            Assert.IsFalse(c.CheckIfDirtySettingExists());

            c.SetValue("test", "val2");

            Assert.IsTrue(c.CheckIfDirtySettingExists());
        }

        [TestMethod]
        public void SettingDataCollectionTest_GetValue()
        {
            var mf = new MainForm();
            mf.Show();

            var c = new SettingDataMCollection(mf.MyDataFile);
            c.Add(new SettingDataM(typeof(string), "test", "val1")
            {

            }, new SettingDataM(typeof(int), "testing", 16)
            {
            });

            Assert.AreEqual("val1", c.GetValue("test"));

            c.SetValue("test", "val2");

            Assert.AreEqual("val2", c.GetValue("test"));

            Assert.AreEqual(16, c.GetValue("testing"));

            Assert.IsNull(c.GetValue("abc"));
        }

        [TestMethod]
        public void SettingDataCollectionTest_SetValue()
        {
            var mf = new MainForm();
            mf.Show();

            var c = new SettingDataMCollection(mf.MyDataFile);
            c.Add(new SettingDataM(typeof(string), "test", "val1")
            {

            }, new SettingDataM(typeof(int), "testing", 16)
            {
            });
            
            c.SetValue("test", "val2");

            Assert.AreEqual("val2", c.GetValue("test"));
        }

        [TestMethod]
        public void SettingDataCollectionTest_LoadBoolAttribute()
        {
            cs_timed_silver.Properties.Settings.Default.AutoOpenLastFile = "No";
            var mf = new MainForm();
            mf.Show();

            mf.MyDataFile.Settings.Add(new SettingDataM(typeof(bool), "test", false)
            {
            });
            mf.MyDataFile.Settings.Add(new SettingDataM(typeof(bool), "test2", false)
            {
            });

            XmlDocument d = new XmlDocument();
            d.AppendChild(d.CreateElement("MyRoot"));
            d.DocumentElement.SetAttribute("test", "False");
            d.DocumentElement.SetAttribute("test2", "True");

            mf.MyDataFile.Settings.LoadBoolAttribute(d, "test");
            mf.MyDataFile.Settings.LoadBoolAttribute(d, "test2");

            Assert.AreEqual(false, mf.MyDataFile.GetValue("test"));
            Assert.AreEqual(true, mf.MyDataFile.GetValue("test2"));
        }

        [TestMethod]
        public void SettingDataCollectionTest_LoadFloatAttribute()
        {
            var mf = new MainForm();
            mf.Show();

            mf.MyDataFile.Settings.Add(new SettingDataM(typeof(float), "test", 0.5f)
            {
            });

            var d = new XmlDocument();
            d.AppendChild(d.CreateElement("MyRoot"));
            d.DocumentElement.SetAttribute("test", "8.99");

            mf.MyDataFile.Settings.LoadFloatAttribute(d, "test");

            Assert.IsTrue(Utils.NearlyEqual(8.99d, (float)mf.MyDataFile.GetValue("test"), 0.01d));
        }

        [TestMethod]
        public void SettingDataCollectionTest_LoadIntAttribute()
        {
            var mf = new MainForm();
            mf.Show();

            mf.MyDataFile.Settings.Add(new SettingDataM(typeof(int), "test", 50)
            {
            });

            var d = new XmlDocument();
            d.AppendChild(d.CreateElement("MyRoot"));
            d.DocumentElement.SetAttribute("test", "89");

            mf.MyDataFile.Settings.LoadIntAttribute(d, "test");

            Assert.AreEqual(89, (int)mf.MyDataFile.GetValue("test"));
        }

        [TestMethod]
        public void SettingDataCollectionTest_LoadTimeSpanAttribute()
        {
            var mf = new MainForm();
            mf.Show();

            mf.MyDataFile.Settings.Add(new SettingDataM(typeof(TimeSpan), "test", TimeSpan.FromMinutes(10))
            {
            });

            var d = new XmlDocument();
            d.AppendChild(d.CreateElement("MyRoot"));
            d.DocumentElement.SetAttribute("test", "9:58:09");

            mf.MyDataFile.Settings.LoadTimeSpanAttribute(d, "test");

            Assert.AreEqual(TimeSpan.Parse("9:58:09"),
                mf.MyDataFile.GetValue("test"));
        }

        [TestMethod]
        public void SettingDataCollectionTest_LoadStringAttribute()
        {
            var mf = new MainForm();
            mf.Show();

            mf.MyDataFile.Settings.Add(new SettingDataM(typeof(string), "test", "brb")
            {
            });

            var d = new XmlDocument();
            d.AppendChild(d.CreateElement("MyRoot"));
            d.DocumentElement.SetAttribute("test", "test value");

            mf.MyDataFile.Settings.LoadStringAttribute(d, "test");

            Assert.AreEqual("test value", mf.MyDataFile.GetValue("test"));
        }

        [TestMethod]
        public void SettingDataCollectionTest_LoadDecimalAttribute()
        {
            var mf = new MainForm();
            mf.Show();

            mf.MyDataFile.Settings.Add(new SettingDataM(typeof(decimal), "test", 19M)
            {
            });

            var d = new XmlDocument();
            d.AppendChild(d.CreateElement("MyRoot"));
            d.DocumentElement.SetAttribute("test", "8.98");

            mf.MyDataFile.Settings.LoadDecimalAttribute(d, "test");

            Assert.AreEqual(8.98M, mf.MyDataFile.GetValue("test"));
        }

        [TestMethod]
        public void SettingDataCollectionTest_LoadRectangleAttribute()
        {
            var mf = new MainForm();
            mf.Show();

            mf.MyDataFile.Settings.Add(new SettingDataM(typeof(Rectangle), "test", Rectangle.FromLTRB(8, 9, 100, 150))
            {
            });

            var d = new XmlDocument();
            d.AppendChild(d.CreateElement("MyRoot"));
            d.DocumentElement.SetAttribute("test", "5|11|100|120");

            mf.MyDataFile.Settings.LoadRectangleAttribute(d, "test", Rectangle.Empty);

            Assert.AreEqual(Rectangle.FromLTRB(5, 11, 100, 120), mf.MyDataFile.GetValue("test"));
        }

        [TestMethod]
        public void SettingDataCollectionTest_ImportFromAttributes()
        {
            var mf = new MainForm();
            mf.Show();

            var c = new SettingDataMCollection(mf.MyDataFile);


            c.Add(new SettingDataM(typeof(Rectangle), "test", Rectangle.FromLTRB(8, 9, 100, 150))
            {
            }, new SettingDataM(typeof(int), "int_test", 9)
            {

            });

            var d = new XmlDocument();
            d.AppendChild(d.CreateElement("MyRoot"));
            d.DocumentElement.SetAttribute("test", "5|11|100|120");
            d.DocumentElement.SetAttribute("int_test", "10");

            c.ImportFromAttributes(d);

            Assert.AreEqual(Rectangle.FromLTRB(5, 11, 100, 120), c.GetValue("test"));
            Assert.AreEqual(10, c.GetValue("int_test"));
        }
    }
}
