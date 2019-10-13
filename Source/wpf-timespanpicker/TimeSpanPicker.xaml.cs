using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Threading;

namespace wpf_timespanpicker
{
    /// <summary>
    /// Interaction logic for TimeSpanPicker.xaml
    /// </summary>
    public partial class TimeSpanPicker : Canvas
    {
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(TimeSpan), typeof(TimeSpanPicker),
                new UIPropertyMetadata(TimeSpan.Zero, OnValueChanged, ValueCoerceCallback));

        public static readonly DependencyProperty ForegroundProperty =
            TextElement.ForegroundProperty.AddOwner(typeof(TimeSpanPicker), new FrameworkPropertyMetadata());
        public Brush Foreground
        {
            get
            {
                return (Brush)GetValue(ForegroundProperty);
            }
            set
            {
                SetValue(ForegroundProperty, value);
            }
        }

        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as TimeSpanPicker).OnValueChanged();
        }

        private static object ValueCoerceCallback(DependencyObject d, object baseValue)
        {
            return ((TimeSpan)baseValue).Subtract(
                TimeSpan.FromMilliseconds(((TimeSpan)baseValue).Milliseconds));
        }

        public TimeSpan Value
        {
            get
            {
                return (TimeSpan)GetValue(ValueProperty);
            }
            set
            {
                SetValue(ValueProperty, value);
            }
        }

        internal void OnValueChanged()
        {
            ApplyValueToVisual(Value);

            TimeSpanValueChanged?.Invoke(this, EventArgs.Empty);
        }

        private string hh, mm, ss;
        internal void ApplyValueToVisual(TimeSpan ts)
        {
            hh = ts.Hours.ToString().PadLeft(2, '0');
            mm = ts.Minutes.ToString().PadLeft(2, '0');
            ss = ts.Seconds.ToString().PadLeft(2, '0');

            tdd1.tb1.Content = hh[0].ToString();
            tdd1.tb2.Content = hh[1].ToString();
            tdd2.tb1.Content = mm[0].ToString();
            tdd2.tb2.Content = mm[1].ToString();
            tdd3.tb1.Content = ss[0].ToString();
            tdd3.tb2.Content = ss[1].ToString();

            ApplyZeroGrayOut();

            if (ts.Hours >= 23)
            {
                hPlusBtn.IsEnabled = false;
            }
            else
            {
                hPlusBtn.IsEnabled = true;
            }

            if (ts.Hours <= 0)
            {
                hMinusBtn.IsEnabled = false;
            }
            else
            {
                hMinusBtn.IsEnabled = true;
            }


            if (ts.Minutes >= 59)
            {
                mPlusBtn.IsEnabled = false;
            }
            else
            {
                mPlusBtn.IsEnabled = true;
            }

            if (ts.Minutes <= 0)
            {
                mMinusBtn.IsEnabled = false;
            }
            else
            {
                mMinusBtn.IsEnabled = true;
            }

            if (ts.Seconds >= 59)
            {
                sPlusBtn.IsEnabled = false;
            }
            else
            {
                sPlusBtn.IsEnabled = true;
            }

            if (ts.Seconds <= 0)
            {
                sMinusBtn.IsEnabled = false;
            }
            else
            {
                sMinusBtn.IsEnabled = true;
            }
        }

        internal int LongPressElapsedMs = 0;
        private void LongPressTimer_Tick(object sender, EventArgs e)
        {
            LongPressElapsedMs += 60;

            if (LongPressElapsedMs > 300)
            {
                if (hPlusBtn.IsMouseOver)
                {
                    FocusedSectionID = 1;
                    AddHourChecked();
                }
                else if (hMinusBtn.IsMouseOver)
                {
                    FocusedSectionID = 1;
                    SubstractHourChecked();
                }

                else if (mPlusBtn.IsMouseOver)
                {
                    FocusedSectionID = 2;
                    AddMinuteChecked();
                }
                else if (mMinusBtn.IsMouseOver)
                {
                    FocusedSectionID = 2;
                    SubstractMinuteChecked();
                }

                else if (sPlusBtn.IsMouseOver)
                {
                    FocusedSectionID = 3;
                    AddSecondChecked();
                }
                else if (sMinusBtn.IsMouseOver)
                {
                    FocusedSectionID = 3;
                    SubstractSecondChecked();
                }
            }
        }

        private static Dictionary<Key, char> dict = new Dictionary<Key, char>()
        {
            [Key.D0] = '0',
            [Key.D1] = '1',
            [Key.D2] = '2',
            [Key.D3] = '3',
            [Key.D4] = '4',
            [Key.D5] = '5',
            [Key.D6] = '6',
            [Key.D7] = '7',
            [Key.D8] = '8',
            [Key.D9] = '9',
            [Key.NumPad0] = '0',
            [Key.NumPad1] = '1',
            [Key.NumPad2] = '2',
            [Key.NumPad3] = '3',
            [Key.NumPad4] = '4',
            [Key.NumPad5] = '5',
            [Key.NumPad6] = '6',
            [Key.NumPad7] = '7',
            [Key.NumPad8] = '8',
            [Key.NumPad9] = '9'
        };
        private void BeginKeyboardEdit(Key k)
        {
            if (!dict.ContainsKey(k) &&
                k != Key.Return &&
                k != Key.Escape)
            {
                return;
            }

            PreviousValue = KeyboardEditedSectionID == 1 ?
                        Value.Hours :
                        (KeyboardEditedSectionID == 2 ?
                        Value.Minutes :
                        Value.Seconds);

            // un singur caracter introdus
            if (KeyboardEditedSectionID != 0/* &&
                KeyboardEditedValue <= 9 &&
                KeyboardEditedValue >= 0*/)
            {
                // 1 char introdus si se apasa Enter
                if (k == Key.Return)
                {
                    EndKeyboardEdit();
                }
                // se apasa Esc si se revine la valorea anterioara
                else if (k == Key.Escape)
                {
                    KeyboardEditedValue = PreviousValue;
                    
                    ApplyValueToVisual(Value);

                    KeyboardEditedSectionID = 0;
                }
                // 1 char introdus si se apasa un nou char
                else if (dict.ContainsKey(k))
                {
                    KeyboardEditedValue *= 10;
                    KeyboardEditedValue += dict[k] - '0';

                    if (KeyboardEditedSectionID == 1)
                    {
                        // valoarea poate fi doar pana la 24
                        if (KeyboardEditedValue >= 24)
                        {
                            KeyboardEditedValue = 23;
                        }
                    }
                    else // KeyboardEditedSectionID is in [2, 3] (minutes, seconds)
                    {
                        // valoarea poate fi doar pana la 59
                        if (KeyboardEditedValue >= 60)
                        {
                            KeyboardEditedValue = 59;
                        }
                    }

                    EndKeyboardEdit();

                    if (FocusedSectionID < 3)
                    {
                        ++FocusedSectionID;
                    }
                }
            }
            // se introduce primul caracter
            else
            {
                if (dict.ContainsKey(k))
                {
                    KeyboardEditedSectionID = FocusedSectionID;
                    KeyboardEditedValue = dict[k] - '0';

                    TimeSpan ts = Value;
                    if (FocusedSectionID == 1)
                    {
                        ts = ts.Subtract
                            (
                            TimeSpan.FromHours(ts.Hours)
                            ).
                            Add(
                                TimeSpan.FromHours
                                    (KeyboardEditedValue)
                            );
                    }
                    else if (FocusedSectionID == 2)
                    {
                        ts = ts.Subtract
                            (
                            TimeSpan.FromMinutes(ts.Minutes)
                            ).
                            Add(
                                TimeSpan.FromMinutes
                                    (KeyboardEditedValue)
                            );
                    }
                    else if (FocusedSectionID == 3)
                    {
                        ts = ts.Subtract
                            (
                            TimeSpan.FromSeconds(ts.Seconds)
                            ).
                            Add(
                                TimeSpan.FromSeconds
                                    (KeyboardEditedValue)
                            );
                    }

                    ApplyValueToVisual(ts);
                }
            }
        }

        protected int KeyboardEditedSectionID = 0;
        protected int KeyboardEditedValue = 0,
            PreviousValue = 0;

        private void EndKeyboardEdit()
        {
            if (KeyboardEditedSectionID != 0)
            {
                if (KeyboardEditedSectionID == 1)
                {
                    Value = Value.
                        Subtract(
                            TimeSpan.
                                FromHours(Value.Hours))
                        .Add(
                            TimeSpan.
                                FromHours(KeyboardEditedValue));
                }
                else if (KeyboardEditedSectionID == 2)
                {
                    Value = Value.
                        Subtract(
                            TimeSpan.
                                FromMinutes(Value.Minutes))
                        .Add(
                            TimeSpan.
                                FromMinutes(KeyboardEditedValue));
                }
                else if (KeyboardEditedSectionID == 3)
                {
                    Value = Value.
                        Subtract(
                            TimeSpan.
                                FromSeconds(Value.Seconds))
                        .Add(
                            TimeSpan.
                                FromSeconds(KeyboardEditedValue));
                }
                KeyboardEditedSectionID = 0;
                KeyboardEditedValue = 0;

                ApplyValueToVisual(Value);
            }
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if (e.Property == ForegroundProperty)
            {
                ApplyZeroGrayOut();
            }
        }

        internal void ApplyZeroGrayOut()
        {
            Brush b = Foreground;
            Brush s = Brushes.White;

            bool zeros = true;
            if ((string)tdd1.tb1.Content != "0")
            {
                zeros = false;
            }
            tdd1.tb1.Foreground = zeros ? Brushes.Gray :
                (FocusedSectionID == 1 ? s : b);
            if ((string)tdd1.tb2.Content != "0")
            {
                zeros = false;
            }
            tdd1.tb2.Foreground = zeros ? Brushes.Gray :
                (FocusedSectionID == 1 ? s : b);
            if ((string)tdd2.tb1.Content != "0")
            {
                zeros = false;
            }
            tdd2.tb1.Foreground = zeros ? Brushes.Gray :
                (FocusedSectionID == 2 ? s : b);
            if ((string)tdd2.tb2.Content != "0")
            {
                zeros = false;
            }
            tdd2.tb2.Foreground = zeros ? Brushes.Gray :
                (FocusedSectionID == 2 ? s : b);
            if ((string)tdd3.tb1.Content != "0")
            {
                zeros = false;
            }
            tdd3.tb1.Foreground = zeros ? Brushes.Gray :
                (FocusedSectionID == 3 ? s : b);
            if ((string)tdd3.tb2.Content != "0")
            {
                zeros = false;
            }
            tdd3.tb2.Foreground = zeros ? Brushes.Gray :
                (FocusedSectionID == 3 ? s : b);
        }

        internal DispatcherTimer LongPressTimer = new DispatcherTimer(DispatcherPriority.Input)
        {
            Interval = TimeSpan.FromMilliseconds(60)
        };

        public TimeSpanPicker()
        {
            InitializeComponent();

            //IsEnabled = false;

            hPlusBtn.MyButton.PreviewMouseDown += HPlusBtn_Click;
            hMinusBtn.MyButton.PreviewMouseDown += HMinusBtn_Click;

            mPlusBtn.MyButton.PreviewMouseDown += MPlusBtn_Click;
            mMinusBtn.MyButton.PreviewMouseDown += MMinusBtn_Click;

            sPlusBtn.MyButton.PreviewMouseDown += SPlusBtn_Click;
            sMinusBtn.MyButton.PreviewMouseDown += SMinusBtn_Click;

            LongPressTimer.Tick += LongPressTimer_Tick;

            //Value = TimeSpan.FromSeconds(0);
            ApplyValueToVisual(Value);
        }

        const double btnWidth = 2d / 8d;
        const double colonWidth = 1d / 8d;

        // Sum of the following two should be 1:
        const double btnHeight = 1d / 4d; // * 2
        const double digitHeight = 2d / 4d;

        protected override Size MeasureOverride(Size constraint)
        {
            var btnSize = new Size(constraint.Width * btnWidth, constraint.Height * btnHeight);
            var colonSize = new Size(constraint.Width * colonWidth, constraint.Height * digitHeight);
            var twoDigitSize = new Size(constraint.Width * btnWidth, constraint.Height * digitHeight);

            hPlusBtn.Measure(btnSize);
            mPlusBtn.Measure(btnSize);
            sPlusBtn.Measure(btnSize);
            hMinusBtn.Measure(btnSize);
            mMinusBtn.Measure(btnSize);
            sMinusBtn.Measure(btnSize);

            tbc1.Measure(colonSize);
            tbc2.Measure(colonSize);

            tdd1.Measure(twoDigitSize);
            tdd2.Measure(twoDigitSize);
            tdd3.Measure(twoDigitSize);

            double w = constraint.Width;
            double h = constraint.Height;
            if (double.IsPositiveInfinity(w))
            {
                w = hPlusBtn.DesiredSize.Width +
                    mPlusBtn.DesiredSize.Width +
                    sPlusBtn.DesiredSize.Width +
                    tbc1.DesiredSize.Width +
                    tbc2.DesiredSize.Width;
            }
            if (double.IsPositiveInfinity(h))
            {
                h = hPlusBtn.DesiredSize.Height +
                    hMinusBtn.DesiredSize.Height +
                    tdd1.DesiredSize.Height;
            }
            return new Size(w, h);
        }

        protected override Size ArrangeOverride(Size arrangeSize)
        {
            double tdd1W = arrangeSize.Width * btnWidth;
            double tbc1W = arrangeSize.Width * colonWidth;
            double btnH = arrangeSize.Height * btnHeight;
            double dH = arrangeSize.Height * digitHeight;

            double tdd1Left = 0;
            double tbc1Left = tdd1Left + tdd1W;

            double tdd2Left = tbc1Left + tbc1W;
            double tbc2Left = tdd2Left + tdd1W;

            double tdd3Left = tbc2Left + tbc1W;



            hPlusBtn.Arrange(new Rect(
                0,
                0,
                tdd1W,
                btnH));
            mPlusBtn.Arrange(new Rect(
                tbc1Left + tbc1W,
                0,
                tdd1W,
                btnH));
            sPlusBtn.Arrange(new Rect(
                tbc2Left + tbc1W,
                0,
                tdd1W,
                btnH));

            hMinusBtn.Arrange(new Rect(
                0,
                arrangeSize.Height - btnH, // *
                tdd1W,
                btnH)); // *
            mMinusBtn.Arrange(new Rect(
                tbc1Left + tbc1W,
                arrangeSize.Height - btnH, // *
                tdd1W,
                btnH)); // *
            sMinusBtn.Arrange(new Rect(
                tbc2Left + tbc1W,
                arrangeSize.Height - btnH, // *
                tdd1W,
                btnH)); // *

            tdd1.Arrange(new Rect(
                tdd1Left,
                hPlusBtn.DesiredSize.Height,
                tdd1W,
                dH));
            tdd2.Arrange(new Rect(
                tdd2Left,
                mPlusBtn.DesiredSize.Height,
                tdd1W,
                dH));
            tdd3.Arrange(new Rect(
                tdd3Left,
                sPlusBtn.DesiredSize.Height,
                tdd1W,
                dH));

            double tdd1Top, tdd2Top;
            tdd1Top = tdd2Top = mPlusBtn.DesiredSize.Height;

            tbc1.Arrange(new Rect(
                tbc1Left,
                tdd1Top,
                tbc1W,
                tdd1.DesiredSize.Height));
            tbc2.Arrange(new Rect(
                tbc2Left,
                tdd2Top,
                tbc1W,
                tdd2.DesiredSize.Height));
            
            return arrangeSize;
        }

        internal int _FocusedSectionID = 0;
        internal int FocusedSectionID
        {
            get
            {
                return _FocusedSectionID;
            }
            set
            {
                if (value != _FocusedSectionID)
                {
                    _FocusedSectionID = value;

                    EndKeyboardEdit();

                    Brush hotbg = Brushes.Blue;
                    Brush hotfg = Brushes.White;

                    if (value == 0)
                    {
                        tdd1.Background = Brushes.Transparent;
                        tdd1.SetForeground(Foreground);
                        tdd2.Background = Brushes.Transparent;
                        tdd2.SetForeground(Foreground);
                        tdd3.Background = Brushes.Transparent;
                        tdd3.SetForeground(Foreground);

                        hPlusBtn.SetPseudofocused(false);
                        hMinusBtn.SetPseudofocused(false);
                        mPlusBtn.SetPseudofocused(false);
                        mMinusBtn.SetPseudofocused(false);
                        sPlusBtn.SetPseudofocused(false);
                        sMinusBtn.SetPseudofocused(false);
                    }
                    else if (value == 1)
                    {
                        tdd1.Background = hotbg;
                        tdd1.SetForeground(hotfg);
                        tdd2.Background = Brushes.Transparent;
                        tdd2.SetForeground(Foreground);
                        tdd3.Background = Brushes.Transparent;
                        tdd3.SetForeground(Foreground);

                        hPlusBtn.SetPseudofocused(true);
                        hMinusBtn.SetPseudofocused(true);
                        mPlusBtn.SetPseudofocused(false);
                        mMinusBtn.SetPseudofocused(false);
                        sPlusBtn.SetPseudofocused(false);
                        sMinusBtn.SetPseudofocused(false);
                    }
                    else if (value == 2)
                    {
                        tdd1.Background = Brushes.Transparent;
                        tdd1.SetForeground(Foreground);
                        tdd2.Background = hotbg;
                        tdd2.SetForeground(hotfg);
                        tdd3.Background = Brushes.Transparent;
                        tdd3.SetForeground(Foreground);

                        hPlusBtn.SetPseudofocused(false);
                        hMinusBtn.SetPseudofocused(false);
                        mPlusBtn.SetPseudofocused(true);
                        mMinusBtn.SetPseudofocused(true);
                        sPlusBtn.SetPseudofocused(false);
                        sMinusBtn.SetPseudofocused(false);

                    }
                    else if (value == 3)
                    {
                        tdd1.Background = Brushes.Transparent;
                        tdd1.SetForeground(Foreground);
                        tdd2.Background = Brushes.Transparent;
                        tdd2.SetForeground(Foreground);
                        tdd3.Background = hotbg;
                        tdd3.SetForeground(hotfg);

                        hPlusBtn.SetPseudofocused(false);
                        hMinusBtn.SetPseudofocused(false);
                        mPlusBtn.SetPseudofocused(false);
                        mMinusBtn.SetPseudofocused(false);
                        sPlusBtn.SetPseudofocused(true);
                        sMinusBtn.SetPseudofocused(true);
                    }

                    ApplyZeroGrayOut();
                }
            }
        }

        private void UserControl_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (!FocusByClick)
            {
                FocusedSectionID = 1;
                e.Handled = true;
            }
        }

        private void UserControl_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            FocusedSectionID = 0;
            e.Handled = true;
        }

        private void UserControl_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Tab)
            {
                if (Keyboard.IsKeyDown(Key.LeftShift) ||
                    Keyboard.IsKeyDown(Key.RightShift))
                {
                    int x = FocusedSectionID - 1;
                    if (x < 1)
                    {
                        var tr = new TraversalRequest(FocusNavigationDirection.Previous);
                        var kbdFocus = Keyboard.FocusedElement as UIElement;
                        if (kbdFocus != null)
                        {
                            kbdFocus.MoveFocus(tr);
                        }
                    }
                    else
                    {
                        FocusedSectionID = x;
                    }
                }
                else
                {
                    int x = FocusedSectionID + 1;
                    if (x > 3)
                    {
                        var tr = new TraversalRequest(FocusNavigationDirection.Next);
                        var kbdFocus = Keyboard.FocusedElement as UIElement;
                        if (kbdFocus != null)
                        {
                            kbdFocus.MoveFocus(tr);
                        }
                    }
                    else
                    {
                        FocusedSectionID = x;
                    }
                }
                e.Handled = true;
            }

            if (!e.Handled)
            {
                BeginKeyboardEdit(e.Key);
            }
        }

        private void HPlusBtn_Click(object sender, RoutedEventArgs e)
        {
            FocusedSectionID = 1;

            AddHourChecked();

            e.Handled = true;
        }

        private void AddHourChecked()
        {
            if (Value.Hours < 23)
            {
                Value = Value.Add(TimeSpan.FromHours(1));
            }
        }

        private void Tdd1_MouseUp(object sender, MouseButtonEventArgs e)
        {
            FocusedSectionID = 1;
            e.Handled = true;
        }

        private void HMinusBtn_Click(object sender, RoutedEventArgs e)
        {
            FocusedSectionID = 1;

            SubstractHourChecked();

            e.Handled = true;
        }

        private void SubstractHourChecked()
        {
            if (Value.Hours > 0)
            {
                Value = Value.Subtract(TimeSpan.FromHours(1));
            }
        }

        private void MPlusBtn_Click(object sender, RoutedEventArgs e)
        {
            FocusedSectionID = 2;

            AddMinuteChecked();

            e.Handled = true;
        }

        private void AddMinuteChecked()
        {
            if (Value.Minutes < 59)
            {
                Value = Value.Add(TimeSpan.FromMinutes(1));
            }
        }

        private void Tdd2_MouseUp(object sender, MouseButtonEventArgs e)
        {
            FocusedSectionID = 2;
            e.Handled = true;
        }

        private void MMinusBtn_Click(object sender, RoutedEventArgs e)
        {
            FocusedSectionID = 2;

            SubstractMinuteChecked();

            e.Handled = true;
        }

        private void SubstractMinuteChecked()
        {
            if (Value.Minutes > 0)
            {
                Value = Value.Subtract(TimeSpan.FromMinutes(1));
            }
        }

        private void SPlusBtn_Click(object sender, RoutedEventArgs e)
        {
            FocusedSectionID = 3;

            AddSecondChecked();

            e.Handled = true;
        }

        private void AddSecondChecked()
        {
            if (Value.Seconds < 59)
            {
                Value = Value.Add(TimeSpan.FromSeconds(1));
            }
        }

        private void Tdd3_MouseUp(object sender, MouseButtonEventArgs e)
        {
            FocusedSectionID = 3;
            e.Handled = true;
        }

        private void UserControl_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Up)
            {
                if (FocusedSectionID == 1)
                {
                    AddHourChecked();
                }
                else if (FocusedSectionID == 2)
                {
                    AddMinuteChecked();
                }
                else if (FocusedSectionID == 3)
                {
                    AddSecondChecked();
                }
                e.Handled = true;
            }
            else if (e.Key == Key.Down)
            {
                if (FocusedSectionID == 1)
                {
                    SubstractHourChecked();
                }
                else if (FocusedSectionID == 2)
                {
                    SubstractMinuteChecked();
                }
                else if (FocusedSectionID == 3)
                {
                    SubstractSecondChecked();
                }
                e.Handled = true;
            }
            else if (e.Key == Key.Left)
            {
                FocusedSectionID =
                    FocusedSectionID == 0 || FocusedSectionID == 1 ?
                    1 : FocusedSectionID - 1;
                e.Handled = true;
            }
            else if (e.Key == Key.Right)
            {
                FocusedSectionID = FocusedSectionID == 3 ?
                    3 : FocusedSectionID + 1;
                e.Handled = true;
            }
        }

        internal bool FocusByClick = false;

        public event EventHandler TimeSpanValueChanged;

        private void UserControl_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (hPlusBtn.IsMouseOver || hMinusBtn.IsMouseOver ||
                tdd1.IsMouseOver)
            {
                FocusedSectionID = 1;
            }
            else if (mPlusBtn.IsMouseOver || mMinusBtn.IsMouseOver ||
                tdd2.IsMouseOver)
            {
                FocusedSectionID = 2;
            }
            else if (sPlusBtn.IsMouseOver || sMinusBtn.IsMouseOver ||
                tdd3.IsMouseOver)
            {
                FocusedSectionID = 3;
            }
            else
            {
                FocusedSectionID = 0;
            }
            e.Handled = true;
        }

        private void UserControl_GotFocus(object sender, RoutedEventArgs e)
        {
            FocusByClick = false;
        }

        private void UserControl_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            ColonPulse = !(bool)e.NewValue;
            if ((bool)e.NewValue)
            {
                hPlusBtn.Visibility = hMinusBtn.Visibility =
                    mPlusBtn.Visibility = mMinusBtn.Visibility =
                    sPlusBtn.Visibility = sMinusBtn.Visibility =
                    Visibility.Visible;
                //tbc1.Visibility = tbc2.Visibility = Visibility.Visible;
            }
            else
            {
                hPlusBtn.Visibility = hMinusBtn.Visibility =
                    mPlusBtn.Visibility = mMinusBtn.Visibility =
                    sPlusBtn.Visibility = sMinusBtn.Visibility =
                    Visibility.Hidden;
                FocusedSectionID = 0;

                //tbc1.Visibility = tbc2.Visibility = Visibility.Hidden;
            }
        }

        private void UserControl_MouseLeave(object sender, MouseEventArgs e)
        {
            LongPressTimer.Stop();
        }

        private void UserControl_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            FocusByClick = true;
            Keyboard.Focus(this);

            if (hPlusBtn.IsMouseOver || hMinusBtn.IsMouseOver ||
                tdd1.IsMouseOver)
            {
                FocusedSectionID = 1;
            }
            else if (mPlusBtn.IsMouseOver || mMinusBtn.IsMouseOver ||
                tdd2.IsMouseOver)
            {
                FocusedSectionID = 2;
            }
            else if (sPlusBtn.IsMouseOver || sMinusBtn.IsMouseOver ||
                tdd3.IsMouseOver)
            {
                FocusedSectionID = 3;
            }
            else
            {
                EndKeyboardEdit();
            }

            if (!hPlusBtn.IsMouseOver && !hMinusBtn.IsMouseOver &&
                !tdd1.IsMouseOver &&
                !mPlusBtn.IsMouseOver && !mMinusBtn.IsMouseOver &&
                !tdd2.IsMouseOver &&
                !sPlusBtn.IsMouseOver && !sMinusBtn.IsMouseOver &&
                !tdd3.IsMouseOver)
            {
                EndKeyboardEdit();
            }

            LongPressElapsedMs = 0;
            LongPressTimer.Start();

            //e.Handled = true;
        }

        private void UserControl_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            LongPressTimer.Stop();

            if (hPlusBtn.IsMouseOver || hMinusBtn.IsMouseOver ||
                tdd1.IsMouseOver)
            {
            }
            else if (mPlusBtn.IsMouseOver || mMinusBtn.IsMouseOver ||
                tdd2.IsMouseOver)
            {
            }
            else if (sPlusBtn.IsMouseOver || sMinusBtn.IsMouseOver ||
                tdd3.IsMouseOver)
            {
            }
            else
            {
                FocusedSectionID = 0;
            }
            e.Handled = true;
        }

        private void UserControl_LostFocus(object sender, RoutedEventArgs e)
        {
            FocusedSectionID = 0;
            e.Handled = true;
        }

        private void UserControl_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (hPlusBtn.IsMouseOver ||
                hMinusBtn.IsMouseOver ||
                tdd1.IsMouseOver)
            {
                FocusedSectionID = 1;
                if (e.Delta < 0) SubstractHourChecked();
                else AddHourChecked();
                e.Handled = true;
            }
            else if (mPlusBtn.IsMouseOver ||
                mMinusBtn.IsMouseOver ||
                tdd2.IsMouseOver)
            {
                FocusedSectionID = 2;
                if (e.Delta < 0) SubstractMinuteChecked();
                else AddMinuteChecked();
                e.Handled = true;
            }
            else if (sPlusBtn.IsMouseOver ||
                sMinusBtn.IsMouseOver ||
                tdd3.IsMouseOver)
            {
                FocusedSectionID = 3;
                if (e.Delta < 0) SubstractSecondChecked();
                else AddSecondChecked();
                e.Handled = true;
            }
        }

        private void SMinusBtn_Click(object sender, RoutedEventArgs e)
        {
            FocusedSectionID = 3;

            SubstractSecondChecked();

            e.Handled = true;
        }

        private void SubstractSecondChecked()
        {
            if (Value.Seconds > 0)
            {
                Value = Value.Subtract(TimeSpan.FromSeconds(1));
            }
        }

        public static readonly DependencyProperty ColonPulseProperty =
            DependencyProperty.Register("ColonPulse", typeof(bool), typeof(TimeSpanPicker),
                new FrameworkPropertyMetadata(false, OnColonPulseChanged));

        public bool ColonPulse
        {
            get
            {
                return (bool)GetValue(ColonPulseProperty);
            }
            set
            {
                SetValue(ColonPulseProperty, value);
            }
        }

        private static void OnColonPulseChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //var o = d as TimeSpanPicker;

            //o.tbc1.Pulse = o.tbc2.Pulse = (bool)e.NewValue;
        }
    }
}
