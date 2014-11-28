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
    public class ExpenseClaimsListViewModel : ViewModelBase
    {
        private IExpenseClaimService _dataService;

        private string _currentStatus;
        private ObservableCollection<ExpenseClaimBase> _items = new ObservableCollection<ExpenseClaimBase>();
        private string _pageTitle = "Expense Claims for Status";
        private bool _isSearch;
        private bool _isEmpty;
        private string _searchText;

        public RelayCommand AddCommand { get; private set; }
        public RelayCommand RefreshCommand { get; private set; }
        public RelayCommand<ExpenseClaimBase> ShowExpenseClaimCommand { get; private set; }

        /// <summary>
        /// Initializes a new instance of the ExpenseClaimsListViewModel class.
        /// </summary>
        ///
        public ExpenseClaimsListViewModel()
        {
            _dataService = Acumatica.Core.Ioc.Container.Default.GetInstance<IExpenseClaimService>();
            InitializeCommands();
            Messenger.Default.Register<ExpenseClaimUpdated>(this, OnExpenseClaimUpdated);
        }

        private void OnExpenseClaimUpdated(ExpenseClaimUpdated message)
        {
            // Check if item is already in the list
            for (int i = 0; i <_items.Count;i++)
            {
                if (_items[i].RefNbr == message.ExpenseClaim.RefNbr)
                {
                    if (_items[i].Status == message.ExpenseClaim.Status)
                    {
                        _items[i] = message.ExpenseClaim;
                    }
                    else
                    {
                        // Status changed; we can't list the item anymore.
                        _items.RemoveAt(i);
                    }

                    return;
                }
            }

            // This is a new item, add it.
            if (message.ExpenseClaim.Status == _currentStatus)
            {
                _items.Insert(0, message.ExpenseClaim);
                OnPropertyChanged("IsEmpty");
            }
        }

        private void InitializeCommands()
        {
            AddCommand = new RelayCommand(ExecuteAddCommand);
            RefreshCommand = new RelayCommand(ExecuteRefreshCommand);
            ShowExpenseClaimCommand = new RelayCommand<ExpenseClaimBase>(ExecuteShowExpenseClaimCommand);
        }

        public bool IsEmpty
        {
            get
            {
                return _isEmpty;
            }
            set
            {
                SetProperty(ref _isEmpty, value);
            }
        }


        public bool IsSearch
        {
            get
            {
                return _isSearch;
            }
        }

        public ObservableCollection<ExpenseClaimBase> Items 
        { 
            get 
            { 
                return _items; 
            } 
        }

        public string SearchText
        {
            get
            {
                return _searchText;
            }
            set
            {
                SetProperty(ref _searchText, value);
            }
        }

        public string PageTitle
        {
            get
            {
                return _pageTitle;
            }
            set
            {
                SetProperty(ref _pageTitle, value);
            }
        }

        public override void LoadState(object navigationParameter, Dictionary<String, Object> viewData)
        {
            string param = navigationParameter as string;
            if (param.StartsWith("search:"))
            {
                _currentStatus = "";
                _searchText = param.Substring(param.IndexOf(":") + 1);
                PageTitle = String.Format("Search Results: {0}", _searchText);
                _isSearch = true;
            }
            else
            {
                _currentStatus = (string)navigationParameter;
                PageTitle = String.Format("{0} Expense Claims", param);
                _isSearch = false;
            }

            ExecuteRefreshCommand();
        }

        private void ExecuteAddCommand()
        {
            Acumatica.Core.Ioc.Container.Default.GetInstance<INavigationService>().NavigateTo("ExpenseClaimPage", "<NEW>");
        }

        #pragma warning disable 4014
        private async void ExecuteRefreshCommand()
        {
            Items.Clear();

            try
            {
                Loading = true;
                IsEmpty = false;

                IList<ExpenseClaimBase> list = null;
                if (_isSearch)
                {
                    list = await _dataService.SearchExpenseClaims(_searchText);
                }
                else
                {
                    list = await _dataService.GetExpenseClaimsForStatus(_currentStatus);
                }

                foreach (var item in list.OrderByDescending(i => i.Date).ThenByDescending(i => i.RefNbr))
                {
                    _items.Add(item);
                }

                IsEmpty = _items.Count == 0;
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

        private void ExecuteShowExpenseClaimCommand(ExpenseClaimBase expenseClaim)
        {
            Acumatica.Core.Ioc.Container.Default.GetInstance<INavigationService>().NavigateTo("ExpenseClaimPage", expenseClaim.RefNbr);
        }

    }
}