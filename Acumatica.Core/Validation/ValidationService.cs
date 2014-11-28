using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Acumatica.Core.Validation
{
    public class ValidationService : IValidationService
    {
        Dictionary<IValidationBehavior, bool> behaviors = new Dictionary<IValidationBehavior, bool>();

        public void AddBehavior(IValidationBehavior behavior)
        {
            behaviors.Add(behavior, true);
            DoValidate();
        }
        public void RemoveBehavior(IValidationBehavior behavior)
        {
            behaviors.Remove(behavior);
        }
        public bool DoValidate(params IValidationBehavior[] excludeBehaviors)
        {
            foreach (IValidationBehavior behavior in behaviors.Keys)
            {
                if (excludeBehaviors.Contains(behavior)) continue;
                behavior.DoValidate();
            }
            return IsValid;
        }
        public bool IsValid
        {
            get
            {
                foreach (IValidationBehavior behavior in behaviors.Keys)
                {
                    if (!behavior.IsValid) return false;
                }
                return true;
            }
        }
    }
}
