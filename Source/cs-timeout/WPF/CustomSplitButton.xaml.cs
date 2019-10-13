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
using Xceed.Wpf.Toolkit;

namespace cs_timed_silver
{
    /// <summary>
    /// Interaction logic for CustomSplitButton.xaml
    /// </summary>
    public partial class CustomSplitButton : SplitButton
    {
        static CustomSplitButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CustomSplitButton),
                new FrameworkPropertyMetadata(typeof(CustomSplitButton)));
        }

        public CustomSplitButton()
        {
            InitializeComponent();
        }

        internal Popup PartPopup;
        internal Button PartButtonWith1, PartButtonWith5, PartButtonWith10, PartButtonWithCustom;
        internal ToggleButton PartToggleButton;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
        }

        public event EventHandler<IntEventArgs> CountRequested;

        private void SplitButton_Loaded(object sender, RoutedEventArgs e)
        {
            PartPopup = (Popup)GetTemplateChild("PART_Popup");
            PartButtonWith1 = (Button)GetTemplateChild("PART_ButtonWith1");
            PartButtonWith5 = (Button)GetTemplateChild("PART_ButtonWith5");
            PartButtonWith10 = (Button)GetTemplateChild("PART_ButtonWith10");
            PartButtonWithCustom = (Button)GetTemplateChild("PART_ButtonWithCustom");

            if (this.TryFindVisualChildElementByName("PART_ToggleButton", out FrameworkElement tb))
            {
                PartToggleButton = (ToggleButton)tb;
            }

            if (PartPopup != null)
            {
                PartPopup.ApplyTemplateRecursively();
            }

            if (PartButtonWith1 != null)
            {
                PartButtonWith1.Click += Btns_NewTimer_Click;
            }
            if (PartButtonWith5 != null)
            {
                PartButtonWith5.Click += Btns_NewTimer_Click;
            }
            if (PartButtonWith10 != null)
            {
                PartButtonWith10.Click += Btns_NewTimer_Click;
            }
            if (PartButtonWithCustom != null)
            {
                PartButtonWithCustom.Click += BtnCustom_Click;
            }

            Mouse.AddPreviewMouseDownHandler(Application.Current.MainWindow, KeepPopupOpen);
        }

        private void KeepPopupOpen(object sender, MouseButtonEventArgs e)
        {
            // click hit testing
            var mouseClickSourceElement = e.OriginalSource as DependencyObject;

            // this split button was not clicked
            if (!ReferenceEquals(e.Source, this))
            {
                TogglePopup(false);
                e.Handled = false;
                return;
            }

            // the clicked element has a (grand)parent button
            // the (grand)parent button is PartActionButton
            var isPartActionButtonClicked =
                mouseClickSourceElement.TryFindVisualParentElementByName("PART_ActionButton", out FrameworkElement button);

            // the clicked element has a (grand)parent button
            // the (grand)parent button is PartToggleButton
            var isPartToggleButtonClicked =
                mouseClickSourceElement.TryFindVisualParentElementByName(PartToggleButton.Name, out FrameworkElement button2);

            bool isPopupContentClicked = false;
            if (!isPartToggleButtonClicked && !isPartActionButtonClicked)
            {
                isPopupContentClicked =
                  // this split button was clicked
                  ReferenceEquals(e.Source, this)
                  // the element from the hit test shows that the PART_ContentPresenter was clicked
                  && mouseClickSourceElement.TryFindVisualParentElementByName("PART_ContentPresenter",
                    out FrameworkElement popupContentPresenter);
            }

            if (isPartActionButtonClicked)
            {
                TogglePopup(false);
                e.Handled = false;
            }
            else if (isPartToggleButtonClicked)
            {
                TogglePopup(!PartPopup.IsOpen);
                e.Handled = true;
            }
            else
            {
                TogglePopup(/*isPartToggleButtonClicked || */isPopupContentClicked);
                e.Handled = !isPopupContentClicked;
            }
        }

        internal void TogglePopup(bool open)
        {
            PartPopup.IsOpen = IsOpen = open;
        }

        private void Btns_NewTimer_Click(object sender, RoutedEventArgs e)
        {
            var b = e.Source as Button;
            if (b?.Content == null)
            {
                CountRequested?.Invoke(this, new IntEventArgs()
                {
                    Value = 1
                });
                TogglePopup(false);
                return;
            }
            if (int.TryParse(b.Content.ToString(), out int x))
            {
                CountRequested?.Invoke(this, new IntEventArgs()
                {
                    Value = x
                });
                TogglePopup(false);
            }
            else
            {
                CountRequested?.Invoke(this, new IntEventArgs()
                {
                    Value = 1
                });
                TogglePopup(false);
            }
        }

        private void BtnCustom_Click(object sender, RoutedEventArgs e)
        {
            ApplyTemplate();
            var o = (IntegerUpDown)Template.FindName("MyCustomIntegerUpDown", this);
            CountRequested?.Invoke(this, new IntEventArgs()
            {
                Value = o.Value.HasValue ? o.Value.Value : 1
            });
            TogglePopup(false);
        }

        private void SplitButton_Click(object sender, RoutedEventArgs e)
        {
            CountRequested?.Invoke(this, new IntEventArgs()
            {
                Value = 1
            });
            TogglePopup(false);
        }
    }
}
