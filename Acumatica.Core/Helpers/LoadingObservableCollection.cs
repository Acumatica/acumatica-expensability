using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Acumatica.Core.Helpers
{
    public class LoadingObservableCollection<T> : ObservableCollection<T>
    {
        public bool _loading;
        public bool _loaded;

        protected override void OnCollectionChanged(System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            base.OnCollectionChanged(e);
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Reset)
            {
                Loaded = false;
            }
        }

        public bool Loading
        {
            get { return _loading; }
            set { 
                _loading = value; 
                this.OnPropertyChanged(new PropertyChangedEventArgs("Loading"));
            }
        }

        public bool Loaded
        {
            get { return _loaded; }
            set
            {
                _loaded = value;
                this.OnPropertyChanged(new PropertyChangedEventArgs("Loaded"));
            }
        }
    }
}
