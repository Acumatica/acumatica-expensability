using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Acumatica.Core
{
    public interface IStateAware
    {
        void LoadState(object navigationParameter, Dictionary<String, Object> viewData);
        void SaveState(Dictionary<String, Object> viewData);
    }
}
