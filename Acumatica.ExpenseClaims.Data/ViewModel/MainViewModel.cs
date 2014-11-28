using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Acumatica.Core;
using Acumatica.Core.Command;
using Acumatica.Core.Messaging;
using Acumatica.Core.Service;
using Acumatica.ExpenseClaims.Model;
using Acumatica.ExpenseClaims.Service;
using Acumatica.ExpenseClaims.Messages;

namespace Acumatica.ExpenseClaims.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private readonly IExpenseClaimService _dataService;

        private ObservableCollection<ExpenseClaimGroup> _groups = new ObservableCollection<ExpenseClaimGroup>();

        public RelayCommand SignOutCommand { get; private set; }
        public RelayCommand AddCommand { get; private set; }
        public RelayCommand RefreshCommand { get; private set; }
        public RelayCommand<string> ShowExpenseClaimsForStatusCommand { get; private set; }
        public RelayCommand<ExpenseClaimBase> ShowExpenseClaimCommand { get; private set; }

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            _dataService = Acumatica.Core.Ioc.Container.Default.GetInstance<IExpenseClaimService>();
            InitializeCommands();
            Messenger.Default.Register<ExpenseClaimUpdated>(this, OnExpenseClaimUpdated);
            RefreshCommand.Execute(null);
        }

        private void OnExpenseClaimUpdated(ExpenseClaimUpdated message)
        {
            foreach (var group in _groups)
            {
                for (int i = 0; i < group.Items.Count; i++)
                {
                    if (group.Items[i].RefNbr == message.ExpenseClaim.RefNbr)
                    {
                        if (message.ExpenseClaim.Status == group.GroupTitle)
                        {
                            // Same status, just swap item
                            group.Items[i] = message.ExpenseClaim;
                        }
                        else
                        {
                            // Different status, need to re-insert in another group
                            group.Items.RemoveAt(i);
                            AddExpenseClaimToGroup(message.ExpenseClaim.Status, message.ExpenseClaim);
                        }

                        return;
                    }
                }
            }

            AddExpenseClaimToGroup(message.ExpenseClaim.Status, message.ExpenseClaim);
        }

        private void AddExpenseClaimToGroup(string groupTitle, ExpenseClaim claim)
        {
            // Check if group exists, if so add item to it
            foreach (var group in _groups)
            {
                if (group.GroupTitle == groupTitle)
                {
                    group.Items.Insert(0, claim);
                    return;
                }
            }

            // Group does not exist, create it.
            var newGroup = new ExpenseClaimGroup(groupTitle);
            newGroup.Items.Add(claim);
            _groups.Add(newGroup);
        }

        private void InitializeCommands()
        {
            SignOutCommand = new RelayCommand(ExecuteSignOutCommand);
            ShowExpenseClaimsForStatusCommand = new RelayCommand<string>(ExecuteShowExpenseClaimsForStatusCommand);
            AddCommand = new RelayCommand(ExecuteAddCommand);
            RefreshCommand = new RelayCommand(ExecuteRefreshCommand);
            ShowExpenseClaimCommand = new RelayCommand<ExpenseClaimBase>(ExecuteShowExpenseClaimCommand);
        }

        public ObservableCollection<ExpenseClaimGroup> Groups
        {
            get
            {
                return _groups;
            }
        }

        public string CurrentUserName
        {
            get
            {
                return _dataService.CurrentLoginName;
            }
        }
       
        private void ExecuteSignOutCommand()
        {
            _dataService.Logout();
            Acumatica.Core.Ioc.Container.Default.GetInstance<INavigationService>().NavigateTo("SignInPage", null);
        }

        private void ExecuteAddCommand()
        {
            Acumatica.Core.Ioc.Container.Default.GetInstance<INavigationService>().NavigateTo("ExpenseClaimPage", "<NEW>");
        }

        #pragma warning disable 4014
        private async void ExecuteRefreshCommand()
        {
            Groups.Clear();

            try
            {
                Loading = true;
                var list = await _dataService.GetMostRecentExpenseClaimsByStatus();

                foreach (var item in list)
                {
                    Groups.Add(item);
                }
            }
            catch (Exception e)
            {
                Acumatica.Core.Ioc.Container.Default.GetInstance<IExceptionHandlerService>().HandleExceptionAsync(e);
            }
            finally
            {
                Loading = false;
            }
        }

        private void ExecuteShowExpenseClaimsForStatusCommand(string status)
        {
            Acumatica.Core.Ioc.Container.Default.GetInstance<INavigationService>().NavigateTo("ExpenseClaimsListPage", status);
        }

        private void ExecuteShowExpenseClaimCommand(ExpenseClaimBase expenseClaim)
        {
            Acumatica.Core.Ioc.Container.Default.GetInstance<INavigationService>().NavigateTo("ExpenseClaimPage", expenseClaim.RefNbr);
        }
    }
}