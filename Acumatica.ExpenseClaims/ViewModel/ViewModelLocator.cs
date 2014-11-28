/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocatorTemplate xmlns:vm="using:ProjectForTemplates.ViewModel"
                                   x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"
*/

using Acumatica.ExpenseClaims.Model;
using Acumatica.ExpenseClaims.Service;
using DevExpress.Core;
using Windows.ApplicationModel;
using System;

namespace Acumatica.ExpenseClaims.ViewModel
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// </summary>
    public class ViewModelLocator
    {
        private static Lazy<MainViewModel> _mainViewModel = new Lazy<MainViewModel>();
        private static Lazy<ExpenseClaimsListViewModel> _expenseClaimsListViewModel = new Lazy<ExpenseClaimsListViewModel>();
        private static Lazy<ExpenseClaimViewModel> _expenseClaimsViewModel = new Lazy<ExpenseClaimViewModel>();
        private static Lazy<ExpenseClaimLineViewModel> _expenseClaimLineViewModel = new Lazy<ExpenseClaimLineViewModel>();

        static ViewModelLocator()
        {
            if (DesignMode.DesignModeEnabled)
            {
                Acumatica.Core.Ioc.Container.Default.Register<IExpenseClaimService, Design.DesignExpenseClaimService>();
                Acumatica.Core.Ioc.Container.Default.Register<IEmployeeService, Design.DesignEmployeeService>();
                Acumatica.Core.Ioc.Container.Default.Register<IExpenseItemsService, Design.DesignExpenseItemsService>();
                Acumatica.Core.Ioc.Container.Default.Register<ICustomerService, Design.DesignCustomerService>();
                Acumatica.Core.Ioc.Container.Default.Register<IProjectService, Design.DesignProjectService>();
            }
            else
            {
                Acumatica.Core.Ioc.Container.Default.Register<IExpenseClaimService, ExpenseClaimService>();
                Acumatica.Core.Ioc.Container.Default.Register<IEmployeeService, EmployeeService>();
                Acumatica.Core.Ioc.Container.Default.Register<IExpenseItemsService, ExpenseItemsService>();
                Acumatica.Core.Ioc.Container.Default.Register<ICustomerService, CustomerService>();
                Acumatica.Core.Ioc.Container.Default.Register<IProjectService, ProjectService>();
            }


            Acumatica.Core.Ioc.Container.Default.Register<Acumatica.Core.Service.IApplicationStateService, Acumatica.Core.Windows.Service.ApplicationStateService>();
            Acumatica.Core.Ioc.Container.Default.Register<IExpenseClaimLineCache, ExpenseClaimLineCache>();
        }

        /// <summary>
        /// Gets the Main property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public MainViewModel Main
        {
            get
            {
                return _mainViewModel.Value;
            }
        }

        /// <summary>
        /// Gets the SignIn property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public SignInViewModel SignIn
        {
            get
            {
                return new ViewModel.SignInViewModel();
            }
        }


        /// <summary>
        /// Gets the ExpenseClaim property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public ExpenseClaimViewModel ExpenseClaim
        {
            get
            {
                return _expenseClaimsViewModel.Value;
            }
        }

        /// <summary>
        /// Gets the ExpenseClaim property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public ExpenseClaimLineViewModel ExpenseClaimLine
        {
            get
            {
                return _expenseClaimLineViewModel.Value;
            }
        }

        /// <summary>
        /// Gets the ExpenseClaimsList property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public ExpenseClaimsListViewModel ExpenseClaimsList
        {
            get
            {
                return _expenseClaimsListViewModel.Value;
            }
        }

        /// <summary>
        /// Cleans up all the resources.
        /// </summary>
        public static void Cleanup()
        {
        }
    }
}