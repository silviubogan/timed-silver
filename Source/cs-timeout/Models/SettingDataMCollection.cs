using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace cs_timed_silver
{
    public class SettingDataMCollection : IUnsavedStatusCapable, IEquatable<SettingDataMCollection>
    {
        internal bool _IsUnsaved = false;
        public bool IsUnsaved
        {
            get
            {
                return _IsUnsaved;
            }
            set
            {
                if (_IsUnsaved != value && !IsUnsavedLocked)
                {
                    _IsUnsaved = value;

                    IsUnsavedChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        public bool IsUnsavedLocked { get; set; } = false;

        public event EventHandler IsUnsavedChanged;

        internal Dictionary<string, SettingDataM> SettingsData = null;

        public Dictionary<string, SettingDataM>.Enumerator GetEnumerator()
        {
            // TODO: is this enough for foreach?
            return SettingsData.GetEnumerator();
        }

        internal DataFile MyDataFile = null;

        internal int Count
        {
            get
            {
                return SettingsData.Count;
            }
        }

        internal void ExportAsAttributes(XmlElement to)
        {
            foreach (KeyValuePair<string, SettingDataM> p in this)
            {
                SettingDataM ds = p.Value;
                if (p.Key == "MainFormRectangle") // TODO: generalize method
                {
                    var r = (Rectangle)ds.Value;
                    to.SetAttribute(p.Key, $"{r.Left}|{r.Top}|{r.Right}|{r.Bottom}");
                    continue;
                }
                if (ds.DataType == typeof(float))
                {
                    to.SetAttribute(p.Key,
                        ((float)ds.Value).ToString(CultureInfo.InvariantCulture));
                    continue;
                }
                if (ds.DataType == typeof(double))
                {
                    to.SetAttribute(p.Key,
                        ((double)ds.Value).ToString(CultureInfo.InvariantCulture));
                    continue;
                }
                if (ds.DataType == typeof(decimal))
                {
                    to.SetAttribute(p.Key,
                        ((decimal)ds.Value).ToString(CultureInfo.InvariantCulture));
                    continue;
                }
                to.SetAttribute(p.Key, ds.Value.ToString());
            }
        }

        internal event EventHandler
            SettingValueSave,
            SettingValueResetToDefault,
            SettingValueResetToLastSaved,
            ResetAllValuesDone;

        internal event EventHandler<SettingValueChangedEventArgs> SettingValueChange;

        internal SettingDataM this[string key]
        {
            get
            {
                if (!Contains(key))
                {
                    return null;
                }
                return SettingsData[key];
            }
            set
            {
                SettingsData[key] = value;
            }
        }

        internal bool Add(SettingDataM ds)
        {
            if (Contains(ds.Name))
            {
                UnsubscribeFromSettingData(SettingsData[ds.Name]);

                SettingsData.Remove(ds.Name);

                SettingsData.Add(ds.Name, ds);

                SubscribeToSettingData(ds);

                return true;
            }
            else
            {
                SettingsData.Add(ds.Name, ds);

                SubscribeToSettingData(ds);

                return true;
            }
        }

        internal bool Add(params SettingDataM[] p)
        {
            bool changed = false;

            foreach (SettingDataM ds in p)
            {
                changed |= Add(ds);
            }

            return changed;
        }

        internal bool Contains(string key)
        {
            return SettingsData.ContainsKey(key);
        }

        internal bool Contains(SettingDataM ds)
        {
            return SettingsData.ContainsValue(ds);
        }

        private void SubscribeToSettingData(SettingDataM ds)
        {
            ds.Changed += Ds_Changed;
            ds.Saved += Ds_Saved;
            ds.ResetToDefault += Ds_ResetToDefault;
            ds.ResetToLastSaved += Ds_ResetToLastSaved;
        }

        private void Ds_ResetToLastSaved(object sender, EventArgs e)
        {
            SettingValueResetToLastSaved?.Invoke(sender, e);
        }

        private void Ds_ResetToDefault(object sender, EventArgs e)
        {
            SettingValueResetToDefault?.Invoke(sender, e);
        }

        private void Ds_Saved(object sender, EventArgs e)
        {
            var ds = sender as SettingDataM;
            SettingValueSave?.Invoke(sender, e);

            IsUnsaved = IsUnsaved &&
                ds.Value.Equals(ds.LastSavedValue);
        }

        private void Ds_Changed(object sender, SettingValueChangedEventArgs e)
        {
            var ds = sender as SettingDataM;

            if (MyDataFile.SetWithoutApply)
            {

            }
            else
            {
                SettingValueChange?.Invoke(sender, e);
            }

            // document is marked as modified
            if (IsUnsaved)
            {
                // do nothing
                if (ds.Value.Equals(ds.DefaultValue))
                {

                }
            }
            // document is marked as unmodified
            else
            {
                // if the value of the changed setting is equal to its default value
                // NOTE: check instead the difference between the saved value and the current value (this check is done already in SettingDataM class in the setter of its value, so everything's OK!)
                if (ds.Value.Equals(ds.DefaultValue))
                {
                    IsUnsaved = true;
                }
                else
                {
                    if (e.IsInitialization)
                    {

                    }
                    else
                    {
                        IsUnsaved = true;
                    }
                }
            }
        }

        private void UnsubscribeFromSettingData(SettingDataM ds)
        {
            ds.Changed -= Ds_Changed;
            ds.Saved -= Ds_Saved;
            ds.ResetToDefault -= Ds_ResetToDefault;
            ds.ResetToLastSaved -= Ds_ResetToLastSaved;
        }

        internal void ResetValue(string s)
        {
            this[s].Reset(SettingDataM.ResetType.ToDefault);
        }

        internal void ResetAllValues()
        {
            foreach (SettingDataM ds in SettingsData.Values)
            {
                ds.Reset(SettingDataM.ResetType.ToDefault);
            }

            ResetAllValuesDone?.Invoke(this, EventArgs.Empty);
        }

        internal static bool SettingsCollectionsHaveEqualValues(
            Dictionary<string, SettingDataM> d1,
            Dictionary<string, SettingDataM> d2)
        {
            foreach (KeyValuePair<string, SettingDataM> p in d1)
            {
                SettingDataM ds = p.Value;
                SettingDataM ds2 = d2[p.Key];

                // settings without a default value (which means
                // they can reset to different values on different moments)
                // should not be compared by values
                if (ds.DefaultValueComputation != null &&
                    ds2.DefaultValueComputation != null)
                {
                    continue;
                }

                if (p.Key == "MainFormRectangle") // TODO: generalize
                {
                    if ((ds.Value == null && ds2.Value != null) ||
                        (ds.Value != null && ds2.Value == null))
                    {
                        // TODO: remove comment:
                        // For the DataFile to be saved on first load,
                        // I must ignore this setting (when it has
                        // a null value)
                        // that changes its value
                        // on first load to an unknown rectangle.
                        return false;
                    }
                    else if (ds.Value != null &&
                        !ds.Value.Equals(ds2.Value))
                    {
                        return false;
                    }
                }
                else if (ds.Value != null && ds2.Value != null)
                {
                    if (!ds.Value.ToString().Equals(ds2.Value.ToString()))
                    {
                        return false;
                    }
                }
                else if (ds.Value == null && ds2.Value != null ||
                         ds.Value != null && ds2.Value == null)
                {
                    return false;
                }
            }

            return true;
        }

        internal bool CheckIfDirtySettingExists()
        {
            foreach (SettingDataM ds in SettingsData.Values)
            {
                if (!ds.Value.ToString().Equals(ds.DefaultValue.ToString()))
                {
                    return true;
                }
            }
            return false;
        }

        public object GetValue(string key)
        {
            if (Contains(key))
            {
                return this[key].Value;
            }
            return null;
        }

        internal void SetValue(string v, object value)
        {
            if (!Contains(v))
            {
                return;
            }

            if (!Equals(GetValue(v), value))
            {
                SettingsData[v].Value = value;
                //if (IsUnsavedLocked)
                //{

                //}
                //else
                //{
                //    IsUnsaved = true;
                //}
                //SettingValueChange?.Invoke(this, new PropertyChangedEventArgs(v));
            }
        }

        internal void LoadStringAttribute(XmlDocument d, string s)
        {
            string at1 = d.SelectSingleNode($"/*/@{s}")?.Value;
            SetValue(s, at1);
        }

        internal void LoadBoolAttribute(XmlDocument d, string s)
        {
            string at1 = d.SelectSingleNode($"/*/@{s}")?.Value;
            if (!string.IsNullOrEmpty(at1))
            {
                SetValue(s, at1 == "True");
            }
        }

        internal void LoadFloatAttribute(XmlDocument d, string s)
        {
            string at351 = d.SelectSingleNode($"/*/@{s}")?.Value;
            if (!string.IsNullOrEmpty(at351))
            {
                SetValue(s, float.Parse(at351, CultureInfo.InvariantCulture));
            }
        }

        internal void LoadDoubleAttribute(XmlDocument d, string s)
        {
            string at351 = d.SelectSingleNode($"/*/@{s}")?.Value;
            if (!string.IsNullOrEmpty(at351))
            {
                SetValue(s, double.Parse(at351, CultureInfo.InvariantCulture));
            }
        }

        internal void LoadIntAttribute(XmlDocument d, string s)
        {
            string at351 = d.SelectSingleNode($"/*/@{s}")?.Value;
            if (!string.IsNullOrEmpty(at351))
            {
                SetValue(s, int.Parse(at351, CultureInfo.InvariantCulture));
            }
        }

        internal void LoadTimeSpanAttribute(XmlDocument d, string s)
        {
            string at3 = d.SelectSingleNode($"/*/@{s}")?.Value;
            if (!string.IsNullOrEmpty(at3))
            {
                SetValue(s, TimeSpan.Parse(at3, CultureInfo.InvariantCulture));
            }
        }

        internal void LoadDecimalAttribute(XmlDocument d, string s)
        {
            string at3 = d.SelectSingleNode($"/*/@{s}")?.Value;
            if (!string.IsNullOrEmpty(at3))
            {
                SetValue(s, decimal.Parse(at3, CultureInfo.InvariantCulture));
            }
        }

        internal void LoadRectangleAttribute(XmlDocument d, string s, Rectangle df)
        {
            string at356 = d.SelectSingleNode($"/*/@{s}")?.Value;
            if (!string.IsNullOrEmpty(at356))
            {
                string[] p = at356.Split('|');
                int l = int.Parse(p[0]);
                int t = int.Parse(p[1]);
                int r = int.Parse(p[2]);
                int b = int.Parse(p[3]);
                SetValue(s, Rectangle.FromLTRB(l, t, r, b));
            }
            else
            {
                SetValue(s, df);
            }
        }

        // TODO: also implement the static Equals
        // TODO: also implement GetHashCode
        public bool Equals(SettingDataMCollection other)
        {
            // TODO: also compare other properties of the SettingData-s such as
            // the default value and the header
            return SettingsCollectionsHaveEqualValues(SettingsData, other.SettingsData);
        }

        public static bool operator ==(SettingDataMCollection c1, SettingDataMCollection c2)
        {
            return c1.Equals(c2);
        }

        public static bool operator !=(SettingDataMCollection c1, SettingDataMCollection c2)
        {
            return !(c1 == c2);
        }

        internal SettingDataMCollection(DataFile df)
        {
            MyDataFile = df;
            SettingsData = new Dictionary<string, SettingDataM>();
        }

        internal void ImportFromAttributes(XmlDocument doc)
        {
            IEnumerable<XmlAttribute> attrs = from s in SettingsData
                                              orderby s.Value.Priority ascending
                                              where doc.DocumentElement.Attributes[s.Key] != null
                                              select doc.DocumentElement.Attributes[s.Key];

            foreach (XmlAttribute attr in attrs)
            {
                Type t = Contains(attr.Name) ? this[attr.Name].DataType : null;

                if (t == null)
                {
                    continue;
                }

                if (t == typeof(string))
                {
                    LoadStringAttribute(doc, attr.Name);
                }
                else if (t == typeof(bool))
                {
                    LoadBoolAttribute(doc, attr.Name);
                }
                else if (t == typeof(float))
                {
                    LoadFloatAttribute(doc, attr.Name);
                }
                else if (t == typeof(double))
                {
                    LoadDoubleAttribute(doc, attr.Name);
                }
                else if (t == typeof(int))
                {
                    LoadIntAttribute(doc, attr.Name);
                }
                else if (t == typeof(TimeSpan))
                {
                    LoadTimeSpanAttribute(doc, attr.Name);
                }
                else if (t == typeof(decimal))
                {
                    LoadDecimalAttribute(doc, attr.Name);
                }
                else if (t == typeof(Rectangle))
                {
                    LoadRectangleAttribute(doc, attr.Name, Rectangle.Empty);
                }
                else
                {
                    throw new NotImplementedException();
                }
            }
        }
    }
}
