using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace wpf_timespanpicker
{
    /// <summary>
    /// Interaction logic for ArrowButton.xaml
    /// </summary>
    public partial class ArrowButton : Button
    {
        internal Path MyTemplateSecondPath,
            MyTemplateFirstPath;

        public ArrowButton()
        {
            InitializeComponent();

            // TODO: use smth like MyButton.PropertyChanged to set this:
            MyButton.Margin = new Thickness(
                -MyButton.BorderThickness.Left,
                -MyButton.BorderThickness.Top,
                -MyButton.BorderThickness.Right,
                -MyButton.BorderThickness.Bottom);
        }

        public bool State
        {
            get
            {
                return (bool)GetValue(StateProperty);
            }
            set
            {
                SetValue(StateProperty, value);
            }
        }
        public static readonly DependencyProperty StateProperty =
            DependencyProperty.Register("State", typeof(bool),
                typeof(ArrowButton), new PropertyMetadata(true, new PropertyChangedCallback(OnStateChanged)));

        private static void OnStateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var b = d as ArrowButton;

            b.MyButton.ApplyTemplate();

            b.MyTemplateFirstPath = (Path)b.MyButton.Template.FindName("MyFirstPath", b.MyButton);

            if (b.State)
            {
                // plus
                b.MyTemplateFirstPath.Style = b.MyButton.FindResource("styleWithPlusSign") as Style;
            }
            else
            {
                // minus
                b.MyTemplateFirstPath.Style = b.MyButton.FindResource("styleWithMinusSign") as Style;
            }
        }

        internal void SetPseudofocused(bool p)
        {
            if (p)
            {
                BorderBrush = Brushes.Blue;
            }
            else
            {
                BorderBrush = Brushes.Transparent;
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            ApplyTemplate();

            MyTemplateSecondPath = (Path)MyButton.Template.FindName("MySecondPath", MyButton);

            UpdateMyLayout();
        }

        private void CommonStates_CurrentStateChanged(object sender, VisualStateChangedEventArgs e)
        {
            // TODO: for easier refactoring, move the strings below to class
            // level string fields (or directly Control fields)
            var btn = e.Control as Button;

            if (e.NewState.Name == "MouseOver")
            {
                btn.Background = Brushes.White;
                
                Path mfp = btn.Template.FindName("MyFirstPath", btn) as Path;
                mfp.Fill = Brushes.White;
            }
            else if (e.NewState.Name == "Pressed")
            {
                btn.Background = Brushes.LightBlue;

                Path mfp = btn.Template.FindName("MyFirstPath", btn) as Path;
                mfp.Fill = Brushes.LightBlue;
            }
            else if (e.NewState.Name == "Disabled")
            {
                btn.Background = (Brush)MyButton.FindResource(SystemColors.ControlBrushKey);

                Path mfp = btn.Template.FindName("MyFirstPath", btn) as Path;
                mfp.Fill = Brushes.Gray;
            }
            else
            {
                btn.Background = (Brush)MyButton.FindResource(SystemColors.ControlBrushKey);

                Path mfp = btn.Template.FindName("MyFirstPath", btn) as Path;
                mfp.Fill = (Brush)MyButton.FindResource(SystemColors.ControlBrushKey);
            }
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if (e.Property == WidthProperty ||
                e.Property == HeightProperty ||
                e.Property == BorderThicknessProperty)
            {
                UpdateMyLayout();
            }
        }

        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateMyLayout();
        }

        internal void UpdateMyLayout()
        {
            if (MyTemplateSecondPath == null)
            {
                return;
            }

            // update focus border size:
            double v = ActualHeight / 5d;
            BorderThickness = new Thickness(v, v, v, v);

            //MyTemplateFirstPath.Margin = new Thickness(v, v, v, v);

            double min = Math.Min(ActualWidth, ActualHeight);

            if (State)
            {
                // plus sign
                //MyTemplateSecondPath.Margin = new Thickness(
                //    min / 5d,
                //    min / 5d,
                //    min / 5d,
                //    min / 5d);
                MyTemplateSecondPath.Width =
                    MyTemplateSecondPath.Height =
                    ActualHeight / 2d;
            }
            else
            {
                // minus sign
                MyTemplateSecondPath.Width =
                    MyTemplateSecondPath.Height =
                    ActualHeight / 2d;
            }
        }
    }
}
