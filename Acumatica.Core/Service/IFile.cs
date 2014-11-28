using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Acumatica.Core.Service
{
    public interface IFile
    {
        string Name { get; set; }
        byte[] Contents { get; set; }
    }
}
