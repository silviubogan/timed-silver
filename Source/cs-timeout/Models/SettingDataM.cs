using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cs_timed_silver
{
    public class SettingDataM : BindableBase, IEquatable<SettingDataM>
    {
        public virtual bool Equals(SettingDataM other)
        {
            return Name == other.Name &&
                Value == other.Value &&
                DataType == other.DataType &&
                DefaultValue == other.DefaultValue &&
                Header == other.Header &&
                VMType == other.VMType;

            // intended to not compare priorities
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as SettingDataM);
        }

        public new static bool Equals(object a, object b)
        {
            if (a == b) return true;
            if (a == null) return false;
            if (b == null) return false;
            return a.Equals(b);
        }

        internal SettingDataM(Type t, Type vmt, string name, object defaultVal, Func<object, object> coerceFunc = null)
        {
            DataType = t;
            Name = name;
            Coerce = coerceFunc;
            DefaultValue = defaultVal;
            Value = defaultVal;
            VMType = vmt;
        }

        internal SettingDataM(Type t, Type vmt, string name, Computation<SettingDataM> defaultValComputation, Func<object, object> coerceFunc = null)
        {
            DataType = t;
            Name = name;
            Coerce = coerceFunc;
            DefaultValueComputation = defaultValComputation;
            Value = defaultValComputation.Compute(this);
            VMType = vmt;
        }

        internal string _Name;
        public string Name
        {
            get { return _Name; }
            set { SetProperty(ref _Name, value); }
        }

        internal string _Category = "General";
        public string Category
        {
            get { return _Category; }
            set { SetProperty(ref _Category, value); }
        }

        internal string _Header = "";
        public string Header
        {
            get { return _Header; }
            set { SetProperty(ref _Header, value); }
        }

        internal object _DefaultValue = null;
        public object DefaultValue
        {
            get { return _DefaultValue; }
            set { SetProperty(ref _DefaultValue, value); }
        }

        internal Computation<SettingDataM> _DefaultValueComputation = null;
        public Computation<SettingDataM> DefaultValueComputation
        {
            get { return _DefaultValueComputation; }
            set { SetProperty(ref _DefaultValueComputation, value); }
        }

        internal float _Priority = 1f;
        /// <summary>
        /// The smaller values indicate earlier-loaded settings.
        /// </summary>
        public float Priority
        {
            get { return _Priority; }
            set { SetProperty(ref _Priority, value); }
        }

        internal object _Value = null;
        public object Value
        {
            get { return _Value; }
            set
            {
                var newValue = value;

                if (Coerce != null)
                {
                    newValue = Coerce(newValue);
                }

                SetProperty(ref _Value, newValue,
                    new Action(() =>
                    {
                        Changed?.Invoke(this, new SettingValueChangedEventArgs()
                        {
                            IsInitialization = FirstChange && FirstChangeIsInitialization
                        });

                        FirstChange = false;
                    }
                ));
            }
        }

        internal Type _DataType;
        public Type DataType
        {
            get { return _DataType; }
            set { SetProperty(ref _DataType, value); }
        }

        internal Type _VMType;
        public Type VMType
        {
            get { return _VMType; }
            set { SetProperty(ref _VMType, value); }
        }

        internal object _LastSavedValue = null;
        public object LastSavedValue
        {
            get { return _LastSavedValue; }
            set { SetProperty(ref _LastSavedValue, value); }
        }

        internal Func<object, object> _Coerce = null;
        public Func<object, object> Coerce
        {
            get { return _Coerce; }
            set { SetProperty(ref _Coerce, value); }
        }

        internal bool _FirstChangeIsInitialization = false;
        public bool FirstChangeIsInitialization
        {
            get { return _FirstChangeIsInitialization; }
            set { SetProperty(ref _FirstChangeIsInitialization, value); }
        }

        internal bool _FirstChange = true;
        public bool FirstChange
        {
            get { return _FirstChange; }
            set { SetProperty(ref _FirstChange, value); }
        }

        internal event EventHandler<SettingValueChangedEventArgs> Changed;
        internal event EventHandler Saved;
        internal event EventHandler ResetToDefault;
        internal event EventHandler ResetToLastSaved;

        public static bool operator ==(SettingDataM d1, SettingDataM d2)
        {
            return d1.Equals(d2);
        }

        public static bool operator !=(SettingDataM d1, SettingDataM d2)
        {
            return !(d1 == d2);
        }

        internal bool Save()
        {
            if (LastSavedValue != Value)
            {
                LastSavedValue = Value;
                Saved?.Invoke(this, EventArgs.Empty);
                return true;
            }
            return false;
        }

        internal enum ResetType
        {
            ToDefault,
            ToLastSaved
        }

        internal bool Reset(ResetType how)
        {
            if (how == ResetType.ToDefault)
            {
                // should I also check if the crt value is the default value (default value being
                // recomputed twice in this Reset)?
                if (DefaultValueComputation == null)
                {
                    if (Value != DefaultValue)
                    {
                        Value = DefaultValue;
                        ResetToDefault?.Invoke(this, EventArgs.Empty);
                        return true;
                    }
                }
                else
                {
                    Value = DefaultValueComputation.Compute(this);
                    ResetToDefault?.Invoke(this, EventArgs.Empty);
                    return true;
                }
                return false;
            }
            else if (how == ResetType.ToLastSaved)
            {
                if (Value != LastSavedValue)
                {
                    Value = LastSavedValue;
                    ResetToLastSaved?.Invoke(this, EventArgs.Empty);
                    return true;
                }
                return false;
            }
            else
            {
                throw new NotImplementedException();
                return false;
            }
        }

        public override string ToString()
        {
            return $"Setting {Name} = {Value}";
        }
    }
}
