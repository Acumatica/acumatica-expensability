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
    public class TrackableModelBase : ModelBase
    {
        bool _hasUnsavedChanges;

        public TrackableModelBase()
        {
            TrackChanges();
        }
    
        [DataMember, TrackChanges(false)]
        public bool HasUnsavedChanges
        {
            get { return _hasUnsavedChanges; }
            set { SetProperty(ref _hasUnsavedChanges, value, OnHasUnsavedChangesChanged); }
        }
        public event EventHandler HasUnsavedChangesChanged;

        protected virtual void OnHasUnsavedChangesChanged(bool oldValue, bool newValue)
        {
            if (HasUnsavedChangesChanged != null)
                HasUnsavedChangesChanged(this, EventArgs.Empty);
        }
        public void TrackChanges()
        {
            PropertyChanged += (s, e) =>
            {
                PropertyInfo property = GetType().GetRuntimeProperty(e.PropertyName);
                if (!TrackPropertyChanges(property)) return;
                HasUnsavedChanges = true;
            };
            foreach (PropertyInfo currentProperty in GetType().GetRuntimeProperties())
            {
                PropertyInfo property = currentProperty;
                if (!typeof(TrackableModelBase).GetTypeInfo().IsAssignableFrom(property.PropertyType.GetTypeInfo())) continue;
                if (!TrackPropertyChanges(property)) continue;
                TrackableModelBase oldValue = null;
                PropertyChangedEventHandler onAggregatedPropertyChanged = (s, e) =>
                {
                    if (e.PropertyName != property.Name) return;
                    TrackableModelBase newValue = (TrackableModelBase)property.GetValue(this);
                    if (oldValue != null)
                        oldValue.HasUnsavedChangesChanged -= OnAggregatedObjectHasUnsavedChangesChanged;
                    if (newValue != null)
                        newValue.HasUnsavedChangesChanged += OnAggregatedObjectHasUnsavedChangesChanged;
                    oldValue = newValue;
                };
                PropertyChanged += onAggregatedPropertyChanged;
                onAggregatedPropertyChanged(this, new PropertyChangedEventArgs(property.Name));
            }
        }
        protected virtual bool TrackPropertyChanges(PropertyInfo property)
        {
            TrackChangesAttribute trackChangesAttribute = property.GetCustomAttribute<TrackChangesAttribute>();
            if (trackChangesAttribute != null) return trackChangesAttribute.TrackChanges;
            return property.GetCustomAttribute<DataMemberAttribute>() != null;
        }

        void OnAggregatedObjectHasUnsavedChangesChanged(object sender, EventArgs e)
        {
            TrackableModelBase obj = (TrackableModelBase)sender;
            if (obj.HasUnsavedChanges)
                HasUnsavedChanges = true;
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class TrackChangesAttribute : Attribute
    {
        public TrackChangesAttribute(bool trackChanges)
        {
            TrackChanges = trackChanges;
        }
        public bool TrackChanges { get; set; }
    }
}
