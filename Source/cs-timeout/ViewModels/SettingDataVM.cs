using System;
using System.Collections.ObjectModel;
using System.Windows;

namespace cs_timed_silver
{
    public class SettingDataVM : DependencyObject
    {
        internal SettingDataM M;



        public ObservableCollection<NamedCommandVM> Actions
        {
            get { return (ObservableCollection<NamedCommandVM>)GetValue(ActionsProperty); }
            set { SetValue(ActionsProperty, value); }
        }

        public static readonly DependencyProperty ActionsProperty =
            DependencyProperty.Register("Actions", typeof(ObservableCollection<NamedCommandVM>), typeof(SettingDataVM), new PropertyMetadata(null));





        public SettingDataVM ParentSetting
        {
            get { return (SettingDataVM)GetValue(ParentSettingProperty); }
            set { SetValue(ParentSettingProperty, value); }
        }

        public static readonly DependencyProperty ParentSettingProperty =
            DependencyProperty.Register("ParentSetting", typeof(SettingDataVM), typeof(SettingDataVM), new PropertyMetadata(null, OnParentSettingChanged));

        private static void OnParentSettingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ps = (SettingDataVM)e.OldValue;

            if (ps != null)
            {
                ps.EditedValueChanged -= (d as SettingDataVM).ParentSetting_PropertyChanged;
            }

            var psn = (SettingDataVM)e.NewValue;

            if (psn != null)
            {
                psn.EditedValueChanged += (d as SettingDataVM).ParentSetting_PropertyChanged;
            }
        }




        public object EditedValue
        {
            get { return (object)GetValue(EditedValueProperty); }
            set { SetValue(EditedValueProperty, value); }
        }

        public static readonly DependencyProperty EditedValueProperty =
            DependencyProperty.Register("EditedValue", typeof(object), typeof(SettingDataVM), new PropertyMetadata(null, OnEditedValueChanged));

        public event EventHandler EditedValueChanged;

        private static void OnEditedValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as SettingDataVM).EditedValueChanged?.Invoke(d, EventArgs.Empty);
        }

        public bool IsReadOnly
        {
            get { return (bool)GetValue(IsReadOnlyProperty); }
            set { SetValue(IsReadOnlyProperty, value); }
        }

        public static readonly DependencyProperty IsReadOnlyProperty =
            DependencyProperty.Register("IsReadOnly", typeof(bool), typeof(SettingDataVM), new PropertyMetadata(false));







        public bool IsChecked
        {
            get { return (bool)GetValue(IsCheckedProperty); }
            set { SetValue(IsCheckedProperty, value); }
        }

        public static readonly DependencyProperty IsCheckedProperty =
            DependencyProperty.Register("IsChecked", typeof(bool), typeof(SettingDataVM), new PropertyMetadata(false));






        private void ParentSetting_PropertyChanged(object sender, EventArgs e)
        {
            UpdateIsReadOnly();
        }

        public SettingDataVM(SettingDataM model)
        {
            Actions = new ObservableCollection<NamedCommandVM>();
            HeaderWithoutAccelerator = "";

            M = model;
            M.PropertyChanged += M_PropertyChanged;

            SyncToViewModel();
        }
        
        internal void SyncToViewModel(string propertyName = "")
        {
            if (propertyName == "")
            {
                SyncToViewModel("Header");
                SyncToViewModel("Value");
                SyncToViewModel("Category");
                SyncToViewModel("ParentSetting");
                return;
            }

            // TODO: use switch-case TODO: comments to each
            if (propertyName == "Header" || propertyName == "Value")
            {
                DisplayString = $"{M.Header} : {M.Value}"; // one way only

                if (propertyName == "Header")
                {
                    Header = M.Header;
                    HeaderWithoutAccelerator = M.Header.Replace("_", ""); // one way only
                }
                else
                {
                    Value = M.Value;
                }
            }
            else if (propertyName == "Category")
            {
                Category = M.Category;
            }
            else if (propertyName == "ParentSetting")
            {
                UpdateIsReadOnly(); // manual, one way only
            }
        }

        internal void SyncToModel(string propertyName)
        {
            switch (propertyName)
            {
                case "Header":
                    M.Header = Header;
                    break;

                case "Value":
                    M.Value = Value;
                    break;

                case "Category":
                    M.Category = Category;
                    break;

                default:
                    break;
            }
        }

        private void M_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            SyncToViewModel(e.PropertyName);
        }

        internal void UpdateIsReadOnly()
        {
            IsReadOnly = ParentSetting == null ? false : !(bool)ParentSetting.EditedValue;
        }




        public string Header
        {
            get { return (string)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register("Header", typeof(string), typeof(SettingDataVM), new PropertyMetadata(""));







        public string HeaderWithoutAccelerator
        {
            get { return (string)GetValue(HeaderWithoutAcceleratorProperty); }
            set { SetValue(HeaderWithoutAcceleratorProperty, value); }
        }

        public static readonly DependencyProperty HeaderWithoutAcceleratorProperty =
            DependencyProperty.Register("HeaderWithoutAccelerator", typeof(string), typeof(SettingDataVM), new PropertyMetadata(null));





        public string Category
        {
            get { return (string)GetValue(CategoryProperty); }
            set { SetValue(CategoryProperty, value); }
        }

        public static readonly DependencyProperty CategoryProperty =
            DependencyProperty.Register("Category", typeof(string), typeof(SettingDataVM), new PropertyMetadata(""));






        public string DisplayString
        {
            get { return (string)GetValue(DisplayStringProperty); }
            set { SetValue(DisplayStringProperty, value); }
        }

        public static readonly DependencyProperty DisplayStringProperty =
            DependencyProperty.Register("DisplayString", typeof(string), typeof(SettingDataVM), new PropertyMetadata(""));






        public object Value
        {
            get { return (object)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(object), typeof(SettingDataVM), new PropertyMetadata(null));






        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            SyncToModel(e.Property.Name);
        }
    }
}
