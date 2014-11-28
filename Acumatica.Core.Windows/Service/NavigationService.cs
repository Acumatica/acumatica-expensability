using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Acumatica.Core;
using Acumatica.Core.Service;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Acumatica.Core.Windows.Service
{
    public class NavigationService : INavigationService
    {
        private Assembly _mainAssembly;
        private string _rootNamespace;
        private Frame _rootframe;
        
        public NavigationService(Assembly mainAssembly, string rootNamespace, Frame rootFrame)
        {
            _mainAssembly = mainAssembly;
            _rootNamespace = rootNamespace;
            _rootframe = rootFrame;
        }

        public void GoBack()
        {
            _rootframe.GoBack();
        }

        public void GoForward()
        {
            _rootframe.GoForward();
        }
        
        public void NavigateTo(string targetName, object parameter)
        {
            System.Type pageType = GetPageType(targetName);
            if (pageType == null)
            {
                throw new Exception(String.Format("Page {0} not found.", targetName));
            }
            else
            {
                _rootframe.Navigate(pageType, parameter);
            }
        }

        public bool CanGoBack
        {
            get
            {
                return _rootframe.CanGoBack;
            }
        }
        
        public bool CanGoForward
        {
            get
            {
                return _rootframe.CanGoForward;
            }
        }

        private System.Type GetPageType(string targetName)
        {
            return _mainAssembly.GetType(_rootNamespace + "." + targetName);
        }
    }
}