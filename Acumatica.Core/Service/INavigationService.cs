using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Acumatica.Core.Service
{
    public interface INavigationService
    {
        void GoBack();
        void GoForward();
        void NavigateTo(string targetName, object parameter);

        bool CanGoBack { get; }
        bool CanGoForward { get; }
    }
}
