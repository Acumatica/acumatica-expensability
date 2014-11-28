using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Acumatica.Core.Ioc
{
    public class ActivationException : Exception
    {
        public ActivationException(string message)
            : base(message)
        {
        }
    }
}
