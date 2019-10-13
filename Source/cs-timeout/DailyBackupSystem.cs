using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace cs_timed_silver
{
    internal class DailyBackupSystem
    {
        internal DataFile MyDataFile { get; set; }

        internal bool Enabled
        {
            get
            {
                return (bool)MyDataFile.Settings.GetValue("AutomaticBackup");
            }
        }

        internal string BackupFilePath
        {
            get
            {
                if (string.IsNullOrWhiteSpace(MyDataFile.FilePath))
                {
                    return null;
                }
                return MyDataFile.FilePath + ".bak"; // TODO: use Path class' methods to add this extension
            }
        }

        internal bool BackupExists
        {
            get
            {
                if (string.IsNullOrWhiteSpace(MyDataFile.FilePath))
                {
                    return false;
                }
                return File.Exists(BackupFilePath);
            }
        }

        internal bool CanDoBackup
        {
            get
            {
                return !BackupExists ||
                    DataFileOlderThanToday; // at most yesterday
            }
        }

        internal bool DataFileOlderThanToday
        {
            get
            {
                if (string.IsNullOrWhiteSpace(MyDataFile.FilePath))
                {
                    return false;
                }
                return File.GetLastWriteTime(MyDataFile.FilePath) < MyDateTimeProvider.TodayDateTime;
            }
        }

        private IDateTimeProvider MyDateTimeProvider;

        internal DailyBackupSystem(DataFile df, IDateTimeProvider dtp)
        {
            MyDataFile = df;
            MyDateTimeProvider = dtp;

            df.BeforeSave += Df_BeforeSave;
        }

        private void Df_BeforeSave(object sender, EventArgs e)
        {
            if (!(bool)MyDataFile.Settings.GetValue("AutomaticBackup"))
            {
                return;
            }

            if (MyDataFile.FilePath != "") // the file is on the disk
            {
                if (CanDoBackup) // it is the first save in the current day
                {
                    DoBackup();
                }
            }
            else
            {
                DoBackup();
            }
        }

        /// <summary>
        /// The `forced` parameter is not used currently.
        /// </summary>
        /// <param name="forced"></param>
        /// <returns></returns>
        internal bool DoBackup(bool forced = false)
        {
            try
            {
                File.Copy(MyDataFile.FilePath, BackupFilePath, true);
                return true;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(MyDataFile.MainWindow, $"The backup file could not be saved. Error: {ex.Message}");
            }

            return false;
        }

        internal void DeleteBackup()
        {
            if (BackupExists)
            {
                File.Delete(BackupFilePath);
            }
        }

        internal void RestoreBackup()
        {
            if (BackupExists)
            {
                string fp = MyDataFile.FilePath;

                File.Copy(BackupFilePath, MyDataFile.FilePath, true);

                MyDataFile.Close(true);

                MyDataFile.MainWindow.IsFileBeingClosed = false;
                if (!MyDataFile.LoadFromFileWPF(fp))
                {
                    // TODO: improve design of this message box:
                    System.Windows.MessageBox.Show(MyDataFile.MainWindow, "Error on loading the backup.");
                }
            }
        }
    }
}
