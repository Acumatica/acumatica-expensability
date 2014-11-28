using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using WindowsUI = Windows.UI; 

namespace Acumatica.Core.Windows
{
    /// <summary>
    /// Provides a wrapper for showing settings with events to track any settings flyout coming or going.
    /// </summary>
    public class SettingsFlyout
    {
        private const int FlyoutWidth = 346;
        private Popup popup;

        /// <summary>
        /// Shows the given control in the flyout.
        /// </summary>
        public void ShowFlyout(UserControl control)
        {
            this.popup = new Popup();
            this.popup.Opened += OnPopupOpened;
            this.popup.Closed += OnPopupClosed;
            Window.Current.Activated += OnWindowActivated;
            this.popup.IsLightDismissEnabled = true;
            this.popup.Width = FlyoutWidth;
            this.popup.Height = Window.Current.Bounds.Height;

            control.Width = FlyoutWidth;
            control.Height = Window.Current.Bounds.Height;

            this.popup.Child = control;
            this.popup.SetValue(Canvas.LeftProperty, Window.Current.Bounds.Width - FlyoutWidth);
            this.popup.SetValue(Canvas.TopProperty, 0);
            this.popup.IsOpen = true;
        }


        private void OnWindowActivated(object sender, WindowsUI.Core.WindowActivatedEventArgs e)
        {
            if (e.WindowActivationState == WindowsUI.Core.CoreWindowActivationState.Deactivated)
            {
                this.popup.IsOpen = false;
            }
        }


        private void OnPopupClosed(object sender, object e)
        {
            Window.Current.Activated -= OnWindowActivated;
            OnSettingsClosed(EventArgs.Empty);
        }


        private void OnPopupOpened(object sender, object e)
        {
            OnSettingsOpened(EventArgs.Empty);
        }

        /// <summary>
        /// Raised to indicate settings flyout has been opened.
        /// </summary>
        public static event EventHandler SettingsOpened;
        private static void OnSettingsOpened(EventArgs args)
        {
            var handler = SettingsFlyout.SettingsOpened;
            if (handler != null)
            {
                handler(null, args);
            }
        }

        /// <summary>
        /// Raised to indicate settings flyout has been closed.
        /// </summary>
        public static event EventHandler SettingsClosed;
        private static void OnSettingsClosed(EventArgs args)
        {
            var handler = SettingsFlyout.SettingsClosed;
            if (handler != null)
            {
                handler(null, args);
            }
        }
    }
}
