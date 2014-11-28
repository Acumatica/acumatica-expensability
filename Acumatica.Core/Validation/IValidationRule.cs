using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Acumatica.Core.Validation
{
    public enum ErrorType
    {
        None = 0,
        Default = 1,
        Information = 2,
        Warning = 3,
        Critical = 4,
        User = 5,
    }

    public interface IValidationRule
    {
        object Value { get; set; }
        bool IsValid { get; }
        ErrorType ErrorType { get; }
        object ErrorContent { get; }
    }
}
