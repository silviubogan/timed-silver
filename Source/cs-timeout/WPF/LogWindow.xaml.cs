using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace cs_timed_silver
{
    /// <summary>
    /// Interaction logic for LogWindow.xaml
    /// </summary>
    public partial class LogWindow : Window
    {
        internal string LogFilePath = System.IO.Path.Combine(
                    Environment.GetFolderPath(
                        Environment.SpecialFolder.LocalApplicationData,
                        Environment.SpecialFolderOption.Create),
                    "timed-silver.log.txt");

        protected string MyFilterString = "";

        public LogWindow()
        {
            InitializeComponent();

            Messages = new ObservableCollection<LogMessageVM>();
        }

        internal ObservableCollection<LogMessageVM> _Messages;
        public ObservableCollection<LogMessageVM> Messages
        {
            get
            {
                return _Messages;
            }
            set
            {
                if (_Messages != value)
                {
                    _Messages = value;
                    OnMessagesChanged();
                }
            }
        }

        internal FileStream MyLogFileStream;
        internal StreamWriter MyLogFileStreamWriter;

        internal void OpenLogFile()
        {
            if (Program.LogToFile && LoggingEnabled)
            {
                using (MyLogFileStream = File.Open(LogFilePath, FileMode.OpenOrCreate, FileAccess.Read, FileShare.Read))
                using (var reader = new StreamReader(MyLogFileStream))
                {
                    while (!reader.EndOfStream)
                    {
                        string l = reader.ReadLine();

                        if (string.IsNullOrWhiteSpace(l))
                        {
                            continue;
                        }

                        string[] p = l.Split('|');

                        if (p.Length != 3)
                        {
                            continue;
                        }

                        var m = new LogMessageVM();
                        for (int i = 0; i <= 2; ++i)
                        {
                            string x = p[i].Trim();
                            if (i == 0)
                            {
                                m.DateTime = DateTime.Parse(x);
                            }
                            else if (i == 1)
                            {
                                m.Category = Utils.StrToEnum<LogCategory>(x);
                            }
                            else if (i == 2)
                            {
                                m.Text = x;
                            }
                        }
                        Messages.Add(m);
                    }

                    reader.Close();
                }

                MyLogFileStream = File.Open(LogFilePath, FileMode.Append, FileAccess.Write, FileShare.Read);
                MyLogFileStreamWriter = new StreamWriter(MyLogFileStream);
            }
        }

        internal void WriteToLog(LogMessageM m)
        {
            if (Program.LogToFile && LoggingEnabled)
            {
                string s = $"{m.DateTime} | {m.Category} | {m.Text}";
                MyLogFileStreamWriter.WriteLine(s);
                MyLogFileStreamWriter.Flush();
            }
        }

        private void OnMessagesChanged()
        {
            MyListView.ItemsSource = Messages;

            var view = (CollectionView)CollectionViewSource.GetDefaultView(MyListView.ItemsSource);
            var groupDescription = new PropertyGroupDescription("DateString");
            view.GroupDescriptions.Add(groupDescription);
            view.Filter = UserFilter;

            if (MyListView.Items.Count > 0)
            {
                MyListView.ScrollIntoView(MyListView.Items[MyListView.Items.Count - 1]);
            }
        }

        private bool UserFilter(object item)
        {
            if (string.IsNullOrEmpty(MyFilterString))
            {
                return true;
            }
            var l = item as LogMessageVM;
            return (l.Text + " " +
                l.Category.ToString() + " " +
                l.DateTime.ToLongDateString()).
                IndexOf(MyFilterString, StringComparison.InvariantCultureIgnoreCase) >= 0;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Hide();
            e.Cancel = true;
        }

        private void MySaveToFileButton_Click(object sender, RoutedEventArgs e)
        {
            var fd = new Microsoft.Win32.SaveFileDialog();
            fd.Filter = "Plain Text (*.txt)|*.txt";
            fd.FileName = DateTime.Now.ToString("dd-MM-yyyy HH-mm-ss z") + ".txt";
            fd.InitialDirectory = Environment.GetFolderPath(
                Environment.SpecialFolder.Personal);
            fd.RestoreDirectory = true;

            bool? r = fd.ShowDialog(this);
            if (r.HasValue && r.Value)
            {
                var b = new StringBuilder();
                foreach (LogMessageM m in (MyListView.ItemsSource as ObservableCollection<LogMessageVM>))
                {
                    string s = $"{m.DateTime} | {m.Category} | {m.Text}";
                    b.AppendLine(s);
                }
                File.WriteAllText(fd.FileName, b.ToString(), Encoding.Unicode);
                MessageBox.Show(this, $"File saved: {fd.FileName}", "Confirmation",
                    MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK);
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            MyFilterString = MyFilterTextBox.Text;

            CollectionViewSource.GetDefaultView(MyListView.ItemsSource).Refresh();
        }

        internal void CloseLogFile()
        {
            if (Program.LogToFile)
            {
                MyLogFileStreamWriter.Flush();
                MyLogFileStreamWriter.Close();
                MyLogFileStreamWriter.Dispose();

                MyLogFileStream.Close();
                MyLogFileStream.Dispose();
            }
        }

        private void MyClearLogButton_Click(object sender, RoutedEventArgs e)
        {
            ClearLog();
        }

        internal void ClearLog(bool reopenAfter = true)
        {
            (Application.Current.MainWindow as MainWindow).MyStatusBar.ClearStatus();

            Messages.Clear();

            if (Program.LogToFile)
            {
                if (LoggingEnabled)
                {
                    CloseLogFile();
                }

                File.WriteAllText(LogFilePath, "");

                if (reopenAfter)
                {
                    OpenLogFile();
                }
            }
        }

        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            if (Program.LogToFile)
            {
                CloseLogFile();
            }
        }

        private void MyOpenLogsFolderButton_Click(object sender, RoutedEventArgs e)
        {
            Utils.SelectFileInItsFolderInExplorer(LogFilePath);
        }

        public bool LoggingEnabled
        {
            get
            {
                return Properties.Settings.Default.EnableLogging;
            }
            set
            {
                if (value != Properties.Settings.Default.EnableLogging)
                {
                    Properties.Settings.Default.EnableLogging = value;
                    Properties.Settings.Default.Save();

                    CbEnableLogging.IsChecked = value;
                }
            }
        }

        private void CbEnableLogging_Checked(object sender, RoutedEventArgs e)
        {
            DoEnableLogging();
        }

        internal void DoEnableLogging()
        {
            if (LoggingEnabled)
            {
                return;
            }

            LoggingEnabled = true;
            OpenLogFile();
        }

        private void CbEnableLogging_Unchecked(object sender, RoutedEventArgs e)
        {
            if (!LoggingEnabled)
            {
                return;
            }

            MessageBoxResult r = MessageBox.Show(this, "Do you also wish to remove the existing log data?",
                "Confirmation request", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
            if (r == MessageBoxResult.Yes)
            {
                ClearLog(false);
            }
            else if (r == MessageBoxResult.No)
            {
                Messages.Clear();
                CloseLogFile();
            }
            else // Cancel || None
            {
                CbEnableLogging.IsChecked = true;
                return;
            }

            LoggingEnabled = false;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            CbEnableLogging.IsChecked = LoggingEnabled;
        }
    }
}
