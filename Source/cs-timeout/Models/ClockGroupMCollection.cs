using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Drawing;
using System.Linq;

namespace cs_timed_silver
{
    public class ClockGroupMCollection : BindableBase, IEquatable<ClockGroupMCollection>
    {
        internal ClockMCollection Clocks;

        public ObservableCollection<ClockGroupM> Ms { get; set; }

        public bool Add(string item)
        {
            if (Contains(item) || string.IsNullOrEmpty(item))
            {
                return false;
            }

            var m = new ClockGroupM()
            {
                Name = item
            };

            //Ms.Insert(0, m);
            Ms.Add(m);

            return true;
        }

        public bool Add(ClockGroupM item)
        {
            if (item == null)
            {
                return false;
            }

            if (Contains(item.Name) || string.IsNullOrEmpty(item.Name))
            {
                return false;
            }

            //Ms.Insert(0, item);
            Ms.Add(item);

            return true;
        }

        public ClockGroupMCollection(ClockMCollection c)
        {
            Clocks = c;

            Ms = new ObservableCollection<ClockGroupM>();
            Ms.CollectionChanged += Ms_CollectionChanged;
        }

        private void Ms_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            Clocks.IsUnsaved = true;
        }

        public bool Clear()
        {
            bool changed = Ms.Count > 0;

            if (changed)
            {
                Ms.Clear();

                foreach (ClockM td in Clocks.Ms)
                {
                    td.GroupName = string.Empty;
                }
            }

            return changed;
        }

        public bool ClearWithoutChangingClocks()
        {
            bool changed = Ms.Count > 0;

            if (changed)
            {
                Ms.Clear();
            }

            return changed;
        }

        public bool Contains(string item)
        {
            foreach (ClockGroupM model in Ms)
            {
                if (model.Name == item)
                {
                    return true;
                }
            }
            return false;
        }

        public bool Remove(string g)
        {
            if (g == "" || !Contains(g))
            {
                return false;
            }

            Ms.Remove(x => x.Name == g);

            foreach (ClockM td in Clocks.Ms)
            {
                if (td.GroupName == g)
                {
                    td.GroupName = "";
                }
            }

            return true;
        }

        internal bool Rename(string g, string gg)
        {
            if (string.IsNullOrEmpty(g) ||
                string.IsNullOrEmpty(gg) ||
                Contains(gg))
            {
                return false;
            }

            bool changed = false;
            for (int i = 0; i < Ms.Count; ++i)
            {
                if (Ms[i].Name == g)
                {
                    Ms[i].Name = gg;

                    changed = true;
                    break;
                }
            }

            if (changed)
            {
                foreach (ClockM td in Clocks.Ms)
                {
                    if (td.GroupName == g)
                    {
                        td.GroupName = gg;
                    }
                }

                return true;
            }

            return false;
        }

        internal int IndexOf(string g)
        {
            for (int i = 0; i < Ms.Count; ++i)
            {
                if (Ms[i].Name == g)
                {
                    return i;
                }
            }
            return -1;
        }

        internal void Move(string g, int toIndex)
        {
            if (toIndex < 0)
            {
                throw new NotImplementedException();
            }

            int oldIndex = IndexOf(g);

            if (oldIndex < 0)
            {
                throw new NotImplementedException();
            }

            Ms.Move(oldIndex, toIndex);
        }

        internal bool ClearGroup(string g)
        {
            if (g == "" || !Contains(g))
            {
                return false;
            }

            foreach (ClockM td in Clocks.Ms)
            {
                if (td.GroupName == g)
                {
                    td.GroupName = "";
                }
            }

            return true;
        }

        public bool Equals(ClockGroupMCollection other)
        {
            return SequenceEqual(other);
        }

        public bool SequenceEqual(ClockGroupMCollection other)
        {
            // NOTE: does not compare Bitmaps by pixels but by references

            if (other == null)
            {
                return false;
            }

            if (Ms.Count != other.Ms.Count)
            {
                return false;
            }

            bool eq = true;
            for (int i = 0; i < Ms.Count; ++i)
            {
                if (Ms[i] != other.Ms[i])
                {
                    eq = false;
                    break;
                }
            }
            return eq;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            var cgc = obj as ClockGroupMCollection;
            if (cgc == null)
                return false;
            else
                return Equals(cgc);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 19;
                foreach (ClockGroupM g in Ms)
                {
                    hash = hash * 31 + g.Name.GetHashCode();
                }
                return hash;
            }
        }

        public static bool operator ==(ClockGroupMCollection cgc1, ClockGroupMCollection cgc2)
        {
            if (((object)cgc1) == null || ((object)cgc2) == null)
                return object.Equals(cgc1, cgc2);

            return cgc1.Equals(cgc2);
        }

        public static bool operator !=(ClockGroupMCollection cgc1, ClockGroupMCollection cgc2)
        {
            return !(cgc1 == cgc2);
        }

        internal bool HasIcon(string g)
        {
            int idx = IndexOf(g);
            if (idx < 0)
            {
                return false;
            }

            return Ms[idx].Icon != null;
        }
    }
}
