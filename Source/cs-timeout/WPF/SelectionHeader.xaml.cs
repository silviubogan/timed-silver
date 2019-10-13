using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace cs_timed_silver
{
    /// <summary>
    /// Interaction logic for SelectionHeader.xaml
    /// </summary>
    public partial class SelectionHeader : UserControl
    {
        public static readonly DependencyProperty ClocksProperty =
            DependencyProperty.Register("Clocks", typeof(ClockVMCollection),
                typeof(SelectionHeader), new PropertyMetadata(null));
        public ClockVMCollection Clocks
        {
            get
            {
                return (ClockVMCollection)GetValue(ClocksProperty);
            }
            set
            {
                SetValue(ClocksProperty, value);
            }
        }

        internal ClockFlowLayoutPanel MyClockListView { get; set; } = null;

        public SelectionHeader()
        {
            InitializeComponent();
        }

        private void BtnSelectAll_Click(object sender, RoutedEventArgs e)
        {
            Clocks.Model.CheckAll();
        }

        private void BtnDeselectAll_Click(object sender, RoutedEventArgs e)
        {
            Clocks.Model.UncheckAll();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            CustomCommands.MultipleSelectionInListView.Execute(false, Application.Current.MainWindow as MainWindow);
        }

        private void BtnMenu_Click(object sender, RoutedEventArgs e)
        {
            Point p = BtnMenu.PointToScreen(
                new Point(BtnMenu.ActualWidth, BtnMenu.ActualHeight)
            );

            MyClockListView.ShowContextMenu(
                new HashSet<ClockVM>(Clocks.SelectedClocks),
                new System.Drawing.Point(
                    (int)Math.Round(p.X),
                    (int)Math.Round(p.Y)),
                System.Windows.Forms.ToolStripDropDownDirection.BelowLeft);
        }

        internal void ShowAnimated()
        {
            var s = FindResource("MyShowStoryboard") as Storyboard;
            BeginStoryboard(s);
        }

        internal void HideAnimated()
        {
            var s = FindResource("MyHideStoryboard") as Storyboard;
            BeginStoryboard(s);
        }
    }
}
