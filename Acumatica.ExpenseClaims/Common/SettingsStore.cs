using Acumatica.Core.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Acumatica.ExpenseClaims.Common
{
    public static class SettingsStore
    {
        public static string SiteUrl
        {
            get
            {
                var settings = Acumatica.Core.Ioc.Container.Default.GetInstance<ISettingsService>();
                return settings.GetValue("SiteUrl") as string;
            }
        }

        public static string Username
        {
            get
            {
                var settings = Acumatica.Core.Ioc.Container.Default.GetInstance<ISettingsService>();
                return settings.GetValue("Username") as string;
            }
        }

        public static string Password
        {
            get
            {
                var settings = Acumatica.Core.Ioc.Container.Default.GetInstance<ISettingsService>();
                return settings.GetValue("Password") as string;
            }
        }
    }
}
