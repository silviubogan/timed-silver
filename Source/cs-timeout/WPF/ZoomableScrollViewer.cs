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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace cs_timed_silver
{
    public class ZoomableScrollViewer : ScrollViewer, IZoomableControl
    {
        public ZoomableScrollViewer()
        {
        }

        public static readonly DependencyProperty ZoomFactorProperty =
            DependencyProperty.Register("ZoomFactor", typeof(decimal), typeof(ZoomableScrollViewer),
                new FrameworkPropertyMetadata(1M));
        public decimal ZoomFactor
        {
            get
            {
                return (decimal)GetValue(ZoomFactorProperty);
            }
            set
            {
                SetValue(ZoomFactorProperty, value);
            }
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            var scaleTransform = new ScaleTransform();
            BindingOperations.SetBinding(scaleTransform, ScaleTransform.ScaleXProperty, new Binding("ZoomFactor")
            {
                RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor, typeof(ScrollViewer), 1)
            });
            BindingOperations.SetBinding(scaleTransform, ScaleTransform.ScaleYProperty, new Binding("ZoomFactor")
            {
                RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor, typeof(ScrollViewer), 1)
            });

            LayoutTransform = scaleTransform;
        }
    }
}
