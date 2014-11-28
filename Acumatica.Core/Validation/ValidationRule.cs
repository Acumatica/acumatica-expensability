using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Acumatica.Core.Validation
{
    public class ValidationRule<TValue> : IValidationRule
    {
        Func<bool> validate;
        bool incorrectType;
        ErrorType errorType;
        object errorContent;

        public ValidationRule(ErrorType errorType, object errorContent, Func<bool> validate)
        {
            this.errorType = errorType;
            this.errorContent = errorContent;
            this.validate = validate;
        }
        public TValue Value { get; private set; }

        bool IValidationRule.IsValid
        {
            get
            {
                return !incorrectType && (validate == null || validate());
            }
        }
        object IValidationRule.Value
        {
            get { return Value; }
            set
            {
                try
                {
                    Value = (TValue)value;
                    incorrectType = false;
                }
                catch
                {
                    Value = default(TValue);
                    incorrectType = true;
                }
            }
        }
        object IValidationRule.ErrorContent { get { return errorContent; } }
        ErrorType IValidationRule.ErrorType { get { return errorType; } }
    }
}
