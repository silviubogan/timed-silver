using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Media;

namespace cs_timed_silver
{
    public static class Extensions
    {
        /// <summary>
        /// TODO: remake for WPF, and use it to not show drop placeholder in MainWindow when SettingsWindow is open.
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        [Obsolete]
        public static bool HasOpenModalChildWindows(this Form f)
        {
            return f.Visible && !f.CanFocus;
        }

        public static int Remove<T>(
            this ObservableCollection<T> coll, Func<T, bool> condition)
        {
            var itemsToRemove = coll.Where(condition).ToList();

            foreach (var itemToRemove in itemsToRemove)
            {
                coll.Remove(itemToRemove);
            }

            return itemsToRemove.Count;
        }

        public static int IndexOf<T>(
            this ObservableCollection<T> coll, Func<T, bool> condition)
        {
            for (int i = 0; i < coll.Count; ++i)
            {
                if (condition(coll[i]))
                {
                    return i;
                }
            }
            return -1;
        }

        public static object GetObjectAtPoint<ItemContainer>(this ItemsControl control, Point p)
                where ItemContainer : FrameworkElement
        {
            // ItemContainer - can be ListViewItem, or TreeViewItem and so on(depends on control)
            ItemContainer obj = GetContainerAtPoint<ItemContainer>(control, p);
            if (obj == null)
                return null;

            return obj.DataContext;//control.ItemContainerGenerator.ItemFromContainer(obj);
        }

        public static ItemContainer GetContainerAtPoint<ItemContainer>(this ItemsControl control, Point p)
            where ItemContainer : DependencyObject
        {
            HitTestResult result = VisualTreeHelper.HitTest(control, p);

            if (result == null)
            {
                return null;
            }

            DependencyObject obj = result.VisualHit;

            while (VisualTreeHelper.GetParent(obj) != null && !(obj is ItemContainer))
            {
                obj = VisualTreeHelper.GetParent(obj);
            }

            // Will return null if not found
            return obj as ItemContainer;
        }

        public static T GetParentOfType<T>(this System.Windows.DependencyObject element) where T : System.Windows.DependencyObject
        {
            Type type = typeof(T);

            if (element == null)
            {
                return null;
            }

            System.Windows.DependencyObject parent = System.Windows.Media.VisualTreeHelper.GetParent(element);

            if (parent == null &&
                ((System.Windows.FrameworkElement)element).Parent is System.Windows.DependencyObject)
            {
                parent = ((System.Windows.FrameworkElement)element).Parent;
            }

            if (parent == null)
            {
                return null;
            }
            else if (parent.GetType() == type ||
                parent.GetType().IsSubclassOf(type))
            {
                return parent as T;
            }

            return GetParentOfType<T>(parent);
        }

        public static IEnumerable<System.Windows.Controls.DataGridRow> GetDataGridRows(
            this System.Windows.Controls.DataGrid grid)
        {
            var itemsSource = grid.ItemsSource as IEnumerable<object>;
            if (null == itemsSource) yield return null;
            foreach (var item in itemsSource)
            {
                var row = grid.ItemContainerGenerator.ContainerFromItem(item) as System.Windows.Controls.DataGridRow;
                if (null != row) yield return row;
            }
        }

        public static async Task MoveClocksFromIndicesToIndex<T>(
            this ObservableCollection<T> oc,
            List<int> oldIndices, int targetIndex, int maxCount)
        {
            int delta = 0;
            foreach (int idx in oldIndices)
            {
                int oldIndex = Math.Max(idx - delta, 0);
                int newIndex = Math.Min(targetIndex, maxCount - 1);

                if (oldIndex == newIndex)
                {
                    continue;
                }

                // when moving from oldIndex, the new oldIndex should be -1
                // if the move is done after the first oldIndex
                oc.Move(oldIndex, newIndex);

                await System.Windows.Application.Current.MainWindow.Dispatcher.InvokeAsync(new Action(() =>
                {
                }), System.Windows.Threading.DispatcherPriority.Loaded);

                if (oldIndex < newIndex)
                {
                    ++delta;
                }
                else
                {
                    ++targetIndex;
                }
            }
        }

        public static bool TryFindVisualChildElementByName(
            this System.Windows.DependencyObject parent,
            string childElementName,
            out System.Windows.FrameworkElement resultElement)
        {
            resultElement = null;

            if (parent is System.Windows.Controls.Primitives.Popup popup)
            {
                parent = popup.Child;
                if (parent == null)
                {
                    return false;
                }
            }

            for (var childIndex = 0; childIndex < System.Windows.Media.VisualTreeHelper.GetChildrenCount(parent); childIndex++)
            {
                System.Windows.DependencyObject childElement = System.Windows.Media.VisualTreeHelper.GetChild(parent, childIndex);

                if (childElement is System.Windows.FrameworkElement uiElement && uiElement.Name.Equals(
                      childElementName,
                      StringComparison.OrdinalIgnoreCase))
                {
                    resultElement = uiElement;
                    return true;
                }

                if (childElement.TryFindVisualChildElementByName(childElementName, out resultElement))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Not working because the ApplyTemplate affects the VisualTree and when applying
        /// templates recursively it does not see the correct updated visual tree to be able
        /// to continue.
        /// </summary>
        /// <param name="root"></param>
        //internal static void ApplyTemplateRecursively(this System.Windows.DependencyObject root)
        //{
        //    if (root is System.Windows.Controls.Primitives.Popup p)
        //    {
        //        p.Child.ApplyTemplateRecursively();
        //        return;
        //    }

        //    if (root is FrameworkElement r)
        //    {
        //        r.ApplyTemplate();
        //    }

        //    foreach (object element in System.Windows.LogicalTreeHelper.GetChildren(root))
        //    {
        //        if (element is System.Windows.DependencyObject el)
        //        {
        //            ApplyTemplateRecursively(el);
        //        }
        //    }
        //}

        /// <summary>
        /// I am not sure if this is sufficiently efficient, because it goes through the entire visual tree.
        /// </summary>
        /// <param name="root"></param>
        internal static void ApplyTemplateRecursively(this System.Windows.DependencyObject root)
        {
            if (root is System.Windows.Controls.Primitives.Popup p)
            {
                p.Child.ApplyTemplateRecursively();
                return;
            }

            if (root is FrameworkElement r)
            {
                r.ApplyTemplate();
            }

            // TODO: do this only if r.ApplyTemplate() call above returns true?
            for (int i = 0; i < System.Windows.Media.VisualTreeHelper.GetChildrenCount(root); ++i)
            {
                DependencyObject d = VisualTreeHelper.GetChild(root, i);
                ApplyTemplateRecursively(d);
            }
        }

        public static bool TryFindVisualParentElement<TParent>(this DependencyObject child, out TParent resultElement)
             where TParent : DependencyObject
        {
            resultElement = null;

            if (child == null)
            {
                return false;
            }

            DependencyObject parentElement = VisualTreeHelper.GetParent(child);

            if (parentElement is TParent parent)
            {
                resultElement = parent;
                return true;
            }

            return parentElement.TryFindVisualParentElement(out resultElement);
        }

        public static bool TryFindVisualParentElementByName(
            this DependencyObject child,
            string elementName,
            out FrameworkElement resultElement)
        {
            resultElement = null;

            if (child == null)
            {
                return false;
            }

            DependencyObject parentElement = VisualTreeHelper.GetParent(child);

            if (parentElement is FrameworkElement frameworkElement &&
                frameworkElement.Name.Equals(elementName, StringComparison.OrdinalIgnoreCase))
            {
                resultElement = frameworkElement;
                return true;
            }

            // NOTE: a change is here:
            return parentElement.TryFindVisualParentElementByName(elementName, out resultElement);
        }

        public static IEnumerable<T> FindVisualChildren<T>(this DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                depObj.ApplyTemplateRecursively();

                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);

                    child.ApplyTemplateRecursively();

                    if (child != null && child is T)
                        yield return (T)child;

                    foreach (T childOfChild in FindVisualChildren<T>(child))
                        yield return childOfChild;
                }
            }
        }

        public static FlowDocument Clone(this FlowDocument f)
        {
            if (f == null)
            {
                throw new ArgumentNullException(nameof(f));
            }

            var d = new FlowDocument();
            Utils.CopyDocument(f, d);
            return d;
        }

        public static string ToPlainText(this FlowDocument f)
        {
            var r = new TextRange(f.ContentStart, f.ContentEnd);
            return r.Text.Trim();
        }
    }
}