using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Acumatica.Core.Validation
{
    public interface IValidationService
    {
        bool DoValidate(params IValidationBehavior[] excludeBehaviors);
        bool IsValid { get; }
        void AddBehavior(IValidationBehavior behavior);
        void RemoveBehavior(IValidationBehavior behavior);
    }
}
