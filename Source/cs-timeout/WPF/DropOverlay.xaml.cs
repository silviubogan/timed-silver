using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace cs_timed_silver
{
    /// <summary>
    /// Interaction logic for DropOverlay.xaml
    /// </summary>
    public partial class DropOverlay : UserControl
    {
        public DropOverlay()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty ShowWarningProperty =
            DependencyProperty.Register("ShowWarning", typeof(bool), typeof(DropOverlay),
                new FrameworkPropertyMetadata(false, OnShowWarningChanged));
        public bool ShowWarning
        {
            get
            {
                return (bool)GetValue(ShowWarningProperty);
            }
            set
            {
                SetValue(ShowWarningProperty, value);
            }
        }
        private static void OnShowWarningChanged(object s, DependencyPropertyChangedEventArgs e)
        {
            var o = (DropOverlay)s;
            if ((bool)e.NewValue) // show-warning mode enabled
            {
                o.MyImage.Source = Imaging.CreateBitmapSourceFromHIcon(
                    System.Drawing.SystemIcons.Warning.Handle,
                    Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions());
                o.MyTextBlock.Text = "Unknown File Extension";
            }
            else
            {
                o.MyImage.Source = new BitmapImage(new Uri("/Resources/rect833.ico", UriKind.Relative));
                o.MyTextBlock.Text = "Drop Here...";
            }
        }

        public void FadeIn()
        {
            Selector.SetIsSelected(this, true);
            //var a = new DoubleAnimation();
            //a.From = 0;
            //a.To = 1;
            //a.Duration = TimeSpan.FromSeconds(0.5);
            //this.BeginAnimation(OpacityProperty, a);
        }

        public void FadeOut()
        {
            Selector.SetIsSelected(this, false);
            //var a = new DoubleAnimation();
            //a.To = 0;
            //a.Duration = TimeSpan.FromSeconds(0.5);
            //this.BeginAnimation(OpacityProperty, a);
        }
    }
}
