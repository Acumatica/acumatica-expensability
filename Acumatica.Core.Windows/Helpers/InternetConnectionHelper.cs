using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking.Connectivity;

namespace Acumatica.Core.Windows.Helpers
{
    public class InternetConnectionStatusChangedEventArgs : EventArgs
    {
        public bool IsConnected { get; private set; }
        public InternetConnectionStatusChangedEventArgs(bool isConnected)
        {
            IsConnected = isConnected;
        }
    }
    public delegate void InternetConnectionStatusChangedEventHandler(object sender, InternetConnectionStatusChangedEventArgs e);

    public static class InternetConnectionHelper
    {
        static InternetConnectionHelper()
        {
            NetworkInformation.NetworkStatusChanged += OnNetworkStatusChanged;
        }
        public static event InternetConnectionStatusChangedEventHandler OnStatusChanged;
        private static void OnNetworkStatusChanged(object sender)
        {
            if (OnStatusChanged != null)
            {
                OnStatusChanged(sender, new InternetConnectionStatusChangedEventArgs(IsConnected));
            }
        }
        public static bool IsConnected
        {
            get { return NetworkInformation.GetInternetConnectionProfile() != null; }
        }

    }
}
