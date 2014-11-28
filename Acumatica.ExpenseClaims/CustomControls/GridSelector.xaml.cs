using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using DevExpress.UI.Xaml.Editors;
using System.ComponentModel;
using DevExpress.UI.Xaml.Editors.Native;

namespace Acumatica.ExpenseClaims.CustomControls
{
    public class ItemSelectedEventArgs : EventArgsBase
    {
        //TODO: this could be updated to directly pass the selected item in the event args; this way we wouldn't need to refresh it from the web service
    }

    public partial class GridSelector : TextEdit
    {
        public event PopupOpenedEventHandler PopupOpened;
        public delegate void PopupOpenedEventHandler(object sender, OpenPopupEventArgs e);

        public event ItemSelectedEventHandler ItemSelected;
        public delegate void ItemSelectedEventHandler(object sender, ItemSelectedEventArgs e);

        public GridSelector()
        {
            this.InitializeComponent();

            var popupbutton = (PopupButtonInfo)this.Buttons[0];
            popupbutton.PopupSettings.PopupOpened += new PopupOpenedWeakEventHandler<PopupButtonInfo>(popupbutton, OnPopupOpened).Handler;
            popupbutton.PopupSettings.PopupClosed += new PopupClosedWeakEventHandler<PopupButtonInfo>(popupbutton, OnPopupClosed).Handler;

            var content = (FrameworkElement)popupbutton.PopupSettings.PopupContent;
            content.DataContext = this;
        }

        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register("ItemsSource", typeof(object),
          typeof(GridSelector), null);

        public object ItemsSource
        {
            get { return this.GetValue(ItemsSourceProperty); }
            set { this.SetValue(ItemsSourceProperty, value); }
        }

        public static readonly DependencyProperty ItemTemplateProperty = DependencyProperty.Register("ItemTemplate", typeof(DataTemplate),
          typeof(GridSelector), null);

        public DataTemplate ItemTemplate
        {
            get { return (DataTemplate) this.GetValue(ItemTemplateProperty); }
            set { this.SetValue(ItemTemplateProperty, value); }
        }

        public static readonly DependencyProperty SelectedValuePathProperty = DependencyProperty.Register("SelectedValuePath", typeof(string),
          typeof(GridSelector), null);

        public string SelectedValuePath
        {
            get { return (string)this.GetValue(SelectedValuePathProperty); }
            set { this.SetValue(SelectedValuePathProperty, value); }
        }

        public static readonly DependencyProperty SelectedValueProperty = DependencyProperty.Register("SelectedValue", typeof(object),
          typeof(GridSelector), new PropertyMetadata(null, OnSelectedValueChanged));
        public object SelectedValue
        {
            get { return (string)this.GetValue(SelectedValueProperty); }
            set { this.SetValue(SelectedValueProperty, value); }
        }

        public static readonly DependencyProperty LoadingProperty = DependencyProperty.Register("Loading", typeof(bool),
          typeof(GridSelector), null);

        public bool Loading
        {
            get { return (bool)this.GetValue(LoadingProperty); }
            set { this.SetValue(LoadingProperty, value); }
        }

        private static void OnSelectedValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var selector = (GridSelector)d;
            selector.Text = e.NewValue.ToString() ?? String.Empty;
            selector.OnItemSelected();
        }

        private void OnPopupClosed(PopupButtonInfo button, object sender, DevExpress.UI.Xaml.Editors.Native.EventArgsBase e)
        {
            this.SelectAll();
        }

        protected void OnPopupOpened(PopupButtonInfo button, object sender, DevExpress.UI.Xaml.Editors.Native.EventArgsBase e)
        {
            if (PopupOpened != null) { PopupOpened(this, new OpenPopupEventArgs() ); }
        }

        protected void OnItemSelected()
        {
            if (ItemSelected != null) { ItemSelected(this, new ItemSelectedEventArgs()); }
        }

        private void ListViewControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var popup = (PopupButtonInfo)base.Buttons[0];
            if (popup.PopupSettings.IsPopupOpen)
            {
                popup.PopupSettings.ClosePopup();
            }
        }
    }
}
