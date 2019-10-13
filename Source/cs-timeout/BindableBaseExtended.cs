using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace cs_timed_silver
{
    /// <summary>
    /// Implementation of <see cref="INotifyPropertyChanged"/> based on <see cref="BindableBase"/>
    /// to simplify models.
    /// </summary>
    public abstract class BindableBaseExtended : INotifyPropertyChanged
    {
        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Checks if a property already matches a desired value. Sets the property and
        /// notifies listeners only when necessary.
        /// </summary>
        /// <typeparam name="T">Type of the property.</typeparam>
        /// <param name="storage">Reference to a property with both getter and setter.</param>
        /// <param name="value">Desired value for the property.</param>
        /// <param name="propertyName">Name of the property used to notify listeners. This
        /// value is optional and can be provided automatically when invoked from compilers that
        /// support CallerMemberName.</param>
        /// <returns>True if the value was changed, false if the existing value matched the
        /// desired value.</returns>
        protected virtual bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            T newValue = value;

            if (EqualityComparer<T>.Default.Equals(storage, newValue)) return false;

            T oldValue = storage;

            storage = newValue;
            RaisePropertyChanged(oldValue, newValue, propertyName);

            return true;
        }

        /// <summary>
        /// Checks if a property already matches a desired value. Sets the property and
        /// notifies listeners only when necessary.
        /// </summary>
        /// <typeparam name="T">Type of the property.</typeparam>
        /// <param name="storage">Reference to a property with both getter and setter.</param>
        /// <param name="value">Desired value for the property.</param>
        /// <param name="propertyName">Name of the property used to notify listeners. This
        /// value is optional and can be provided automatically when invoked from compilers that
        /// support CallerMemberName.</param>
        /// <param name="onChanged">Action that is called after the property value has been changed.</param>
        /// <returns>True if the value was changed, false if the existing value matched the
        /// desired value.</returns>
        protected virtual bool SetProperty<T>(ref T storage, T value, Action onChanged, [CallerMemberName] string propertyName = null)
        {
            T newValue = value;

            if (EqualityComparer<T>.Default.Equals(storage, newValue)) return false;

            T oldValue = storage;

            storage = newValue;
            onChanged?.Invoke();
            RaisePropertyChanged(oldValue, newValue, propertyName);

            return true;
        }

        /// <summary>
        /// Raises this object's PropertyChanged event.
        /// </summary>
        /// <param name="propertyName">Name of the property used to notify listeners. This
        /// value is optional and can be provided automatically when invoked from compilers
        /// that support <see cref="CallerMemberNameAttribute"/>.</param>
        protected void RaisePropertyChanged(object oldValue, object newValue, [CallerMemberName]string propertyName = null)
        {
            //TODO: when we remove the old OnPropertyChanged method we need to uncomment the below line
            //OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
#pragma warning disable CS0618 // Type or member is obsolete
            OnPropertyChanged(oldValue, newValue, propertyName);
#pragma warning restore CS0618 // Type or member is obsolete
        }

        /// <summary>
        /// Notifies listeners that a property value has changed.
        /// </summary>
        /// <param name="propertyName">Name of the property used to notify listeners. This
        /// value is optional and can be provided automatically when invoked from compilers
        /// that support <see cref="CallerMemberNameAttribute"/>.</param>
        [Obsolete("Please use the new RaisePropertyChanged method. This method will be removed to comply wth .NET coding standards. If you are overriding this method, you should overide the OnPropertyChanged(PropertyChangedEventArgs args) signature instead.")]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        protected virtual void OnPropertyChanged(object oldValue, object newValue, [CallerMemberName]string propertyName = null)
        {
            OnPropertyChanged(new PropertyChangedExtendedEventArgs(oldValue, newValue, propertyName));
        }

        /// <summary>
        /// Raises this object's PropertyChanged event.
        /// </summary>
        /// <param name="args">The PropertyChangedEventArgs</param>
        protected virtual void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            PropertyChanged?.Invoke(this, args);
        }

        /// <summary>
        /// Raises this object's PropertyChanged event.
        /// </summary>
        /// <typeparam name="T">The type of the property that has a new value</typeparam>
        /// <param name="propertyExpression">A Lambda expression representing the property that has a new value.</param>
        [Obsolete("Please use RaisePropertyChanged(nameof(PropertyName)) instead. Expressions are slower, and the new nameof feature eliminates the magic strings.")]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        protected virtual void OnPropertyChanged<T>(Expression<Func<T>> propertyExpression)
        {
            var propertyName = PropertySupport.ExtractPropertyName(propertyExpression);
            OnPropertyChanged(null, null, propertyName);
        }
    }

    public class PropertyChangedExtendedEventArgs : PropertyChangedEventArgs
    {
        public virtual object OldValue { get; private set; }
        public virtual object NewValue { get; private set; }
        public PropertyChangedExtendedEventArgs(object oldValue, object newValue, string propertyName) : base(propertyName)
        {
            OldValue = oldValue;
            NewValue = newValue;
        }
    }

    public delegate void PropertyChangedExtendedEventHandler(object sender, PropertyChangedExtendedEventArgs e);

    //public interface INotifyPropertyChangedExtended
    //{
    //    event PropertyChangedExtendedEventHandler PropertyChanged;
    //}
}
