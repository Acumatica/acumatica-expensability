﻿using System;
using System.Windows;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Acumatica.Core.Windows.Helpers
{
    //Source: http://weblogs.asp.net/dwahlin/archive/2009/08/20/creating-a-silverlight-datacontext-proxy-to-simplify-data-binding-in-nested-controls.aspx
    public class DataContextProxy : FrameworkElement
    {
        public DataContextProxy()
        {
            this.Loaded += new RoutedEventHandler(DataContextProxy_Loaded);
        }

        void DataContextProxy_Loaded(object sender, RoutedEventArgs e)
        {
            Binding binding = new Binding();
            if (!String.IsNullOrEmpty(BindingPropertyName))
            {
                binding.Path = new PropertyPath(BindingPropertyName);
            }
            binding.Source = this.DataContext;
            binding.Mode = BindingMode;
            this.SetBinding(DataContextProxy.DataSourceProperty, binding);             
        }

        public Object DataSource
        {
            get { return (Object)GetValue(DataSourceProperty); }
            set { SetValue(DataSourceProperty, value); }
        }

        public static readonly DependencyProperty DataSourceProperty =
            DependencyProperty.Register("DataSource", typeof(Object), typeof(DataContextProxy), null);

        public string BindingPropertyName { get; set; }
        public BindingMode BindingMode { get; set; }
    }
}