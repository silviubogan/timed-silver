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
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace cs_timed_silver
{
    /// <summary>
    /// Interaction logic for ZoomToolBar.xaml
    /// </summary>
    public partial class ZoomToolBar : ToolBar
    {
        public static readonly DependencyProperty LinkedZoomableControlProperty =
            DependencyProperty.Register("LinkedZoomableControl", typeof(IZoomableControl),
                typeof(ZoomToolBar), new FrameworkPropertyMetadata(null, OnLinkedZoomableControlChanged));
        public IZoomableControl LinkedZoomableControl
        {
            get
            {
                return (IZoomableControl)GetValue(LinkedZoomableControlProperty);
            }
            set
            {
                SetValue(LinkedZoomableControlProperty, value);
            }
        }
        private static void OnLinkedZoomableControlChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var o = d as ZoomToolBar;
            var nv = (IZoomableControl)e.NewValue;

            if (e.OldValue == null)
            {
                return;
            }

            if (nv != null)
            {
                o.Value = nv.ZoomFactor * 100M;
            }
            else
            {
                o.Value = 100M;
            }
        }

        public ZoomToolBar()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty ShowZoomToFitProperty =
            DependencyProperty.Register("ShowZoomToFit", typeof(bool),
        typeof(ZoomToolBar), new FrameworkPropertyMetadata(false, OnShowZoomToFitChanged));
        public bool ShowZoomToFit
        {
            get
            {
                return (bool)GetValue(ShowZoomToFitProperty);
            }
            set
            {
                SetValue(ShowZoomToFitProperty, value);
            }
        }
        private static void OnShowZoomToFitChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var o = d as ZoomToolBar;
            o.UpdateZoomToFitVisibility((bool)e.NewValue);
        }

        internal void UpdateZoomToFitVisibility(bool show)
        {
            ApplyTemplate();
            BtnZoomToFit.Visibility = show ? Visibility.Visible : Visibility.Collapsed;
        }

        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(ZoomToolBar),
                new FrameworkPropertyMetadata(""));
        public string Title
        {
            get
            {
                return (string)GetValue(TitleProperty);
            }
            set
            {
                SetValue(TitleProperty, value);
            }
        }

        private void ComboBox_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
        }

        internal void Submit()
        {
            if (MyComboBox == null)
            {
                return;
            }
            string s = MyComboBox.Text;
            if (s.EndsWith("%"))
            {
                s = s.Substring(0, s.Length - 1);
            }
            try
            {
                if (double.TryParse(s, out double p))
                {
                    if (MySlider != null)
                    {
                        MySlider.Value = p;
                        ApplyZoom();
                        ValueChanged?.Invoke(this, EventArgs.Empty);
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        internal void Submit(int p)
        {
            if (MySlider != null)
            {
                Value = p;
                ApplyZoom();
                ValueChanged?.Invoke(this, EventArgs.Empty);
            }

            Dispatcher.BeginInvoke(new Action(() =>
            {
                MyComboBox.Text = $"{p}%";
            }));
        }

        public event EventHandler ValueChanged, ZoomApplicationRequested, ZoomToFitRequested;

        private void BtnZoomIn_Click(object sender, RoutedEventArgs e)
        {
            MySlider.Value += MySlider.SmallChange;
        }

        private void BtnZoomOut_Click(object sender, RoutedEventArgs e)
        {
            MySlider.Value -= MySlider.SmallChange;
        }

        private void BtnZoomReset_Click(object sender, RoutedEventArgs e)
        {
            MySlider.Value = 100d;
        }

        public decimal Value
        {
            get
            {
                return (decimal)MySlider.Value;
            }
            set
            {
                MySlider.Value = (double)value;
            }
        }

        private void MySlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (MyComboBox != null)
            {
                MyComboBox.Text = $"{(int)Math.Round(e.NewValue)}%";
            }
            Dispatcher.BeginInvoke(new Action(() =>
            {
                ApplyZoom();
                ValueChanged?.Invoke(this, EventArgs.Empty);
            }));
        }

        private void MyZoomToolBar_Loaded(object sender, RoutedEventArgs e)
        {
            ApplyZoom();
        }

        internal void ApplyZoom()
        {
            if (LinkedZoomableControl == null)
            {
                ZoomApplicationRequested?.Invoke(this, EventArgs.Empty);
                return;
            }

            LinkedZoomableControl.ZoomFactor = Value / 100M;
        }

        private void MyComboBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter ||
                e.Key == Key.Return)
            {
                Submit();
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    SelectAll();
                }));
                e.Handled = true;
            }
        }

        internal void SelectAll()
        {
            var textBox = (MyComboBox.Template.FindName("PART_EditableTextBox", MyComboBox) as TextBox);
            if (textBox != null)
            {
                textBox.Focus();
                textBox.SelectAll();
                //textBox.SelectionStart = textBox.Text.Length;
            }
        }

        private void BtnZoomToFit_Checked(object sender, RoutedEventArgs e)
        {
            BtnZoomIn.IsEnabled = BtnZoomOut.IsEnabled = BtnZoomReset.IsEnabled = MyComboBox.IsEnabled = MySlider.IsEnabled = false;
            ZoomToFitRequested?.Invoke(this, EventArgs.Empty);
        }

        private void BtnZoomToFit_Unchecked(object sender, RoutedEventArgs e)
        {
            BtnZoomIn.IsEnabled = BtnZoomOut.IsEnabled = BtnZoomReset.IsEnabled = MyComboBox.IsEnabled = MySlider.IsEnabled = true;
            ApplyZoom();
        }

        private void MyComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                Submit((int)e.AddedItems[0]);
            }
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            UpdateZoomToFitVisibility(ShowZoomToFit);
        }
    }
}
