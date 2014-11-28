using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Acumatica.Core.Validation;
using Windows.UI.Xaml;

namespace Acumatica.ExpenseClaims.Common
{
    public class ValidationBehavior : DevExpress.Core.Behavior<DevExpress.UI.Xaml.Editors.BaseEdit>, IValidationBehavior
    {
        bool doValidate = false;

        public static readonly DependencyProperty RuleProperty =
            DependencyProperty.Register("Rule", typeof(object), typeof(ValidationBehavior), new PropertyMetadata(null, (d, e) => ((ValidationBehavior)d).OnRuleChanged(e)));
        public static readonly DependencyProperty ServiceProperty =
            DependencyProperty.Register("Service", typeof(object), typeof(ValidationBehavior), new PropertyMetadata(null, (d, e) => ((ValidationBehavior)d).OnServiceChanged(e)));

        public IValidationRule Rule
        {
            get { return (IValidationRule)GetValue(RuleProperty); }
            set { SetValue(RuleProperty, value); }
        }
        public IValidationService Service
        {
            get { return (IValidationService)GetValue(ServiceProperty); }
            set { SetValue(ServiceProperty, value); }
        }
        public bool IsValid { get { return !AssociatedObject.HasValidationError; } }

        public void DoValidate()
        {
            doValidate = true;
            AssociatedObject.DoValidate();
            doValidate = false;
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.Validate += OnAssociatedObjectValidate;
            if (Service != null)
                Service.AddBehavior(this);
        }
        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.Validate -= OnAssociatedObjectValidate;
            if (Service != null)
                Service.RemoveBehavior(this);
        }

        void OnRuleChanged(DependencyPropertyChangedEventArgs e)
        {
            //Disabled, when on the system doesn't show us the red boxes around invalid text on form open
            //DoValidate();
        }

        void OnServiceChanged(DependencyPropertyChangedEventArgs e)
        {
            IValidationService oldValue = (IValidationService)e.OldValue;
            IValidationService newValue = (IValidationService)e.NewValue;
            if (AssociatedObject != null && oldValue != null)
                oldValue.RemoveBehavior(this);
            if (AssociatedObject != null && newValue != null)
                newValue.AddBehavior(this);
        }
        void OnAssociatedObjectValidate(object sender, DevExpress.UI.Xaml.Editors.Native.ValidationEventArgs e)
        {
            if (Rule == null || Service == null) return;
            Rule.Value = e.Value;
            if (!doValidate)
                Service.DoValidate(this);
            e.Handled = true;
            e.IsValid = Rule.IsValid;
            e.ErrorType = (DevExpress.UI.Xaml.Editors.Native.ErrorType)(e.IsValid ? ErrorType.None : Rule.ErrorType);
            e.ErrorContent = e.IsValid ? null : Rule.ErrorContent;
        }
    }

}
