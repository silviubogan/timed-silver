using System;
using System.Collections.Generic;
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
    /// Interaction logic for ClockMenuItem.xaml
    /// </summary>
    public partial class AudioFileMenuItem : MenuItem
    {
        public static RoutedUICommand ToggleCommand = new RoutedUICommand();
        public static RoutedUICommand OpenCommand = new RoutedUICommand();

        public static readonly DependencyProperty PlayingProperty =
            DependencyProperty.Register("Playing", typeof(bool), typeof(AudioFileMenuItem),
                new FrameworkPropertyMetadata(false, OnPlayingChanged));
        public bool Playing
        {
            get { return (bool)GetValue(PlayingProperty); }
            set { SetValue(PlayingProperty, value); }
        }
        internal NAudioPlayer MyPlayer;
        private static void OnPlayingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var o = d as AudioFileMenuItem;
            if ((bool)e.NewValue)
            {
                o.MyPlayer.PlaySound();
            }
            else
            {
                o.MyPlayer.StopSound();
            }
        }

        public AudioFileMenuItem()
        {
            InitializeComponent();

            DataContext = this;
        }

        private void MenuItem_Loaded(object sender, RoutedEventArgs e)
        {
            MyPlayer = new NAudioPlayer();
            MyPlayer.AudioFilePath = ToolTip.ToString();
        }

        internal bool Validate()
        {
            if (!File.Exists(ToolTip.ToString()))
            {
                MessageBox.Show(Application.Current.MainWindow,
                    $"The file \"{ToolTip.ToString()}\" does not exist currently in the file system.",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            return true;
        }

        private void Executed_Toggle(object sender, ExecutedRoutedEventArgs e)
        {
            if (!Validate())
            {
                return;
            }
            Playing = !Playing;
        }

        private void Executed_Open(object sender, ExecutedRoutedEventArgs e)
        {
            if (!Validate())
            {
                return;
            }
            Utils.OpenUrlInDefaultApp(ToolTip.ToString());
        }

        private void MenuItem_Unloaded(object sender, RoutedEventArgs e)
        {

        }
    }
}
