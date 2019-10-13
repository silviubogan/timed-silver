using System;
using System.Data;
using System.Xml;
using cs_timed_silver;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TimedSilverTests
{
    [TestClass]
    public class BeepDataTableTest
    {
        [TestMethod]
        public void BeepDataTableTest_ctor()
        {
            // Arrange
            var bdt = new BeepDataTable("table " + (new Random()).Next());

            // Act
            DataRow r = bdt.NewRow();
            r[0] = 1000;
            r[1] = 500;
            r[2] = 200;

            DataRow r2 = bdt.NewRow();
            r2[0] = 5000;
            r2[1] = 100;
            r2[2] = 100;

            bdt.Rows.Add(r);
            bdt.Rows.Add(r2);

            // Assert
            Assert.AreEqual(3, bdt.Columns.Count);
            Assert.AreEqual(2, bdt.Rows.Count);
        }

        [TestMethod]
        public void BeepDataTableTest_CloneAsBeepDataTable()
        {
            // Arrange
            var bdt = new BeepDataTable("table " + (new Random()).Next());

            // Act
            DataRow r = bdt.NewRow();
            r[0] = 1000;
            r[1] = 500;
            r[2] = 200;

            DataRow r2 = bdt.NewRow();
            r2[0] = 5000;
            r2[1] = 100;
            r2[2] = 100;

            bdt.Rows.Add(r);
            bdt.Rows.Add(r2);

            BeepDataTable bdt2 = bdt.CloneAsBeepDataTable();

            // Assert
            Assert.AreEqual(3, bdt2.Columns.Count);
            Assert.AreEqual(2, bdt2.Rows.Count);
        }

        [TestMethod]
        public void BeepDataTableTest_ToXMLString()
        {
            // Arrange
            string tableName = "My_Table";
            var bdt = new BeepDataTable(tableName);

            // Act
            DataRow r = bdt.NewRow();
            r[0] = 1000;
            r[1] = 500;
            r[2] = 200;

            DataRow r2 = bdt.NewRow();
            r2[0] = 5000;
            r2[1] = 100;
            r2[2] = 100;

            bdt.Rows.Add(r);
            bdt.Rows.Add(r2);

            string b = bdt.ToXMLString();

            var doc = new XmlDocument();
            doc.LoadXml(b);

            // Assert
            Assert.AreEqual(1, doc.SelectNodes($"/DocumentElement").Count);
            Assert.AreEqual(2, doc.SelectNodes($"/DocumentElement/{tableName}").Count);
            Assert.AreEqual(2, doc.SelectNodes($"/DocumentElement/{tableName}/MsBeforeRinging").Count);
            Assert.AreEqual(2, doc.SelectNodes($"/DocumentElement/{tableName}/BeepDuration").Count);
            Assert.AreEqual(2, doc.SelectNodes($"/DocumentElement/{tableName}/BeepFrequency").Count);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void BeepDataTableTest_validation()
        {
            var bdt = new BeepDataTable();

            bdt.Rows.Add(new object[] {
                100, 100, 100
            });
            bdt.Rows.Add(new object[] {
                200, "test", 200
            });
        }
    }
}
