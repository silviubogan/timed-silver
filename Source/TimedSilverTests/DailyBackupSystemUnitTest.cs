using System;
using System.IO;
using cs_timed_silver;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TimedSilverTests
{
    [TestClass]
    public class DailyBackupSystemUnitTest
    {
        [TestMethod]
        public void DailyBackupSystemTest_BackupFilePath()
        {
            cs_timed_silver.Properties.Settings.Default.AutoOpenLastFile = "No";
            var mf = new MainForm();
            mf.Show();

            Assert.IsNull(mf.MyDataFile.MyDailyBackupSystem.BackupFilePath);

            mf.MyDataFile.LoadFromFile("test.xml");
            
            mf.MyDataFile.MyDailyBackupSystem.DeleteBackup();

            Assert.AreEqual(mf.MyDataFile.MyDailyBackupSystem.BackupFilePath,
                mf.MyDataFile.FilePath + ".bak");

            mf.MyDataFile.MyDailyBackupSystem.DoBackup();

            Assert.IsTrue(mf.MyDataFile.MyDailyBackupSystem.BackupFilePath.EndsWith(".bak"));
        }

        [TestMethod]
        public void DailyBackupSystemTest_BackupExists()
        {
            cs_timed_silver.Properties.Settings.Default.AutoOpenLastFile = "No";
            var mf = new MainForm();
            mf.Show();

            Assert.IsFalse(mf.MyDataFile.MyDailyBackupSystem.BackupExists);

            mf.MyDataFile.LoadFromFile("test.xml");

            mf.MyDataFile.MyDailyBackupSystem.DeleteBackup();

            Assert.IsFalse(mf.MyDataFile.MyDailyBackupSystem.BackupExists);

            mf.MyDataFile.MyDailyBackupSystem.DoBackup();

            Assert.IsTrue(mf.MyDataFile.MyDailyBackupSystem.BackupExists);
        }

        [TestMethod]
        public void DailyBackupSystemTest_DataFileOlderThanToday()
        {
            cs_timed_silver.Properties.Settings.Default.AutoOpenLastFile = "No";
            var mf = new MainForm();
            mf.Show();

            Assert.IsFalse(mf.MyDataFile.MyDailyBackupSystem.DataFileOlderThanToday);

            mf.MyDataFile.LoadFromFile("test.xml");
            mf.MyDataFile.SetValueWithoutApply("DarkMode",
                !(bool)mf.MyDataFile.Settings.GetValue("DarkMode"));
            mf.MyDataFile.Save();

            // unless I run the test at midnight
            Assert.IsFalse(mf.MyDataFile.MyDailyBackupSystem.DataFileOlderThanToday);
        }

        [TestMethod]
        public void DailyBackupSystemTest_DoBackup()
        {
            cs_timed_silver.Properties.Settings.Default.AutoOpenLastFile = "No";
            var mf = new MainForm();
            mf.Show();

            mf.MyDataFile.MyDailyBackupSystem.DeleteBackup();

            mf.MyDataFile.LoadFromFile("test.xml");

            mf.MyDataFile.MyDailyBackupSystem.DoBackup();
            
            Assert.IsTrue(File.Exists(mf.MyDataFile.MyDailyBackupSystem.BackupFilePath));
        }

        [TestMethod]
        public void DailyBackupSystemTest_DeleteBackup()
        {
            cs_timed_silver.Properties.Settings.Default.AutoOpenLastFile = "No";
            var mf = new MainForm();
            mf.Show();

            mf.MyDataFile.LoadFromFile("test.xml");

            mf.MyDataFile.MyDailyBackupSystem.DoBackup();

            mf.MyDataFile.MyDailyBackupSystem.DeleteBackup();

            Assert.IsFalse(File.Exists(mf.MyDataFile.MyDailyBackupSystem.BackupFilePath));
        }

        [TestMethod]
        public void DailyBackupSystemTest_RestoreBackup()
        {
            cs_timed_silver.Properties.Settings.Default.AutoOpenLastFile = "No";
            var mf = new MainForm();
            mf.Show();

            Assert.IsFalse(mf.MyDataFile.MyDailyBackupSystem.DataFileOlderThanToday);
            
            mf.MyDataFile.LoadFromFile("test.xml");

            mf.MyDataFile.MyDailyBackupSystem.DoBackup();

            bool lastSet = !(bool)mf.MyDataFile.Settings.GetValue("DarkMode");

            mf.MyDataFile.SetValueWithoutApply("DarkMode",
                lastSet);
            mf.MyDataFile.Save();
            
            mf.MyDataFile.MyDailyBackupSystem.RestoreBackup();

            Assert.AreEqual(!lastSet, (bool)mf.MyDataFile.Settings.GetValue("DarkMode"));
        }
    }
}
