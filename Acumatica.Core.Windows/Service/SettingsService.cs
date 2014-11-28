using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Acumatica.Core.Service;
using Windows.Storage;

namespace Acumatica.Core.Windows.Service
{
    public class SettingsService : ISettingsService
    {
        public object GetValue(string key)
        {
            return ApplicationData.Current.LocalSettings.Values[key];
        }

        public void SetValue(string key, object value)
        {
            ApplicationData.Current.LocalSettings.Values[key] = value;
        }
    }
}
