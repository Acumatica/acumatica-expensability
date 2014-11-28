using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Reflection;

namespace Acumatica.Core
{
    [DataContract]
    public class ModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            return this.SetProperty<T>(ref storage, value, null, propertyName);
        }

        protected bool SetProperty<T>(ref T storage, T value, Action<T, T> changedCallback, [CallerMemberName] string propertyName = null)
        {
            if (object.Equals((T)storage, value))
            {
                return false;
            }
            T oldValue = storage;
            storage = value;
            if (changedCallback != null)
            {
                changedCallback(oldValue, value);
            }
            this.OnPropertyChanged(propertyName);
            return true;
        }
    }
}
