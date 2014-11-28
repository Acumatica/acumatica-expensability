using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Acumatica.Core.Service;
using Windows.System;

namespace Acumatica.Core.Windows.Service
{
    public class UriLauncherService : IUriLauncherService
    {
        public async Task<bool> LaunchUriAsync(Uri uri)
        {
            return await Launcher.LaunchUriAsync(uri);
        }
    }
}
