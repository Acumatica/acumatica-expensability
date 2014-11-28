using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Acumatica.Core.Validation
{
    public interface IValidationBehavior
    {
        bool IsValid { get; }
        void DoValidate();
    }
}
