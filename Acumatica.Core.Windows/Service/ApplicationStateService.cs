using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Acumatica.Core.Service;
using Windows.ApplicationModel;

namespace Acumatica.Core.Windows.Service
{
    public class ApplicationStateService : IApplicationStateService
    {
        public bool IsInDesignMode
        {
            get
            {
                return DesignMode.DesignModeEnabled;
            }
        }
    }
}
