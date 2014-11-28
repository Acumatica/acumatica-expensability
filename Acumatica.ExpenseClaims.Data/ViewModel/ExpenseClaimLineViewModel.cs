using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Acumatica.Core;
using Acumatica.Core.Helpers;
using Acumatica.Core.Command;
using Acumatica.Core.Messaging;
using Acumatica.Core.Service;
using Acumatica.ExpenseClaims.Model;
using Acumatica.ExpenseClaims.Service;
using Acumatica.ExpenseClaims.Messages;
using Acumatica.Core.Validation;

namespace Acumatica.ExpenseClaims.ViewModel
{
    public class ExpenseClaimLineViewModel : ViewModelBase
    {
        private string _pageTitle = "Expense Claim 000000000 Line";
        private ExpenseClaimLine _model;

        private static LoadingObservableCollection<ExpenseItem> _expenseItems = new LoadingObservableCollection<ExpenseItem>();
        private static LoadingObservableCollection<Customer> _customers = new LoadingObservableCollection<Customer>();

        // We do not cache this one, since it depends on the currently selected customer.
        private LoadingObservableCollection<CustomerLocation> _customerLocations = new LoadingObservableCollection<CustomerLocation>();

        private static LoadingObservableCollection<Project> _projects = new LoadingObservableCollection<Project>();

        // We do not cache this one, since it depends on the currently selected project.
        private LoadingObservableCollection<ProjectTask> _projectTasks = new LoadingObservableCollection<ProjectTask>();

        public RelayCommand AddAttachmentCommand { get; private set; }
        public RelayCommand RefreshExpenseItemDescriptionCommand { get; private set; }
        public RelayCommand SaveLineCommand { get; private set; }
        public RelayCommand DeleteLineCommand { get; private set; }

        public RelayCommand LoadExpenseItemsCommand { get; private set; }
        public RelayCommand LoadCustomersCommand { get; private set; }
        public RelayCommand LoadCustomerLocationsCommand { get; private set; }
        public RelayCommand LoadProjectsCommand { get; private set; }
        public RelayCommand LoadProjectTasksCommand { get; private set; }

        public ValidationRule<string> LineDescriptionValidation { get; private set; }
        public ValidationRule<string> ExpenseIdValidation { get; private set; }

        /// <summary>
        /// Initializes a new instance of the ExpenseClaimLineViewModel class.
        /// </summary>
        public ExpenseClaimLineViewModel()
        {
            InitializeValidationRules();
            InitializeCommands();

            if (IsInDesignMode)
            {
                LoadDesignData();
            }
        }

        private async void LoadDesignData()
        {
            var service = Acumatica.Core.Ioc.Container.Default.GetInstance<IExpenseClaimService>();
            var claim = await service.GetExpenseClaim("12345");
            _model = claim.Lines[0];
            OnPropertyChanged("Model");
        }

        private void InitializeValidationRules()
        {
            // Validation messages has to be short enough to fit when in snapped view since they don't wrap.
            LineDescriptionValidation = new ValidationRule<string>(ErrorType.Critical, "Required. Maximum 256 characters.", () => !(string.IsNullOrEmpty(LineDescriptionValidation.Value) || LineDescriptionValidation.Value.Length > 256));
            ExpenseIdValidation = new ValidationRule<string>(ErrorType.Critical, "Required.", () => !(string.IsNullOrEmpty(ExpenseIdValidation.Value)));
        }

        private void InitializeCommands()
        {
            AddAttachmentCommand = new RelayCommand(ExecuteAddAttachmentCommand);
            RefreshExpenseItemDescriptionCommand = new RelayCommand(ExecuteRefreshExpenseItemDescriptionCommand);
            SaveLineCommand = new RelayCommand(ExecuteSaveLineCommand);
            DeleteLineCommand = new RelayCommand(ExecuteDeleteLineCommand);

            LoadExpenseItemsCommand = new RelayCommand(ExecuteLoadExpenseItemsCommand);
            LoadCustomersCommand = new RelayCommand(ExecuteLoadCustomersCommand);
            LoadProjectsCommand = new RelayCommand(ExecuteLoadProjectsCommand);
            LoadProjectTasksCommand = new RelayCommand(ExecuteLoadProjectTasksCommand); 
            LoadCustomerLocationsCommand = new RelayCommand(ExecuteLoadCustomerLocationsCommand); 
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

        private void ExecuteSaveLineCommand()
        {
            if (SaveLine())
            {
                Acumatica.Core.Ioc.Container.Default.GetInstance<Acumatica.Core.Service.INavigationService>().GoBack();
            }
        }


        private async void ExecuteDeleteLineCommand()
        {
            if (Model != null)
            {
                var dialog = Acumatica.Core.Ioc.Container.Default.GetInstance<IMessageDialogService>();
                dialog.Title = "Delete Current Line";
                dialog.Content = "Are you sure you want to delete the current line?";
                dialog.Buttons = MessageDialogButtons.YesNo;
                var result = await dialog.ShowAsync();

                if (result == MessageDialogResult.Yes)
                {
                    Model.Deleted = true;
                    var cache = Acumatica.Core.Ioc.Container.Default.GetInstance<IExpenseClaimLineCache>();
                    cache.CurrentLine = Model;
                    Acumatica.Core.Ioc.Container.Default.GetInstance<Acumatica.Core.Service.INavigationService>().GoBack();
                }
            }
        }


        #pragma warning disable 4014
        private async void ExecuteRefreshExpenseItemDescriptionCommand()
        {
            var itemService = Acumatica.Core.Ioc.Container.Default.GetInstance<IExpenseItemsService>();
            if (!String.IsNullOrEmpty(_model.ExpenseId))
            {
                try
                {
                    _model.Description = await itemService.GetExpenseItemDescription(_model.ExpenseId);
                }
                catch (Exception e)
                {

                    Acumatica.Core.Ioc.Container.Default.GetInstance<IExceptionHandlerService>().HandleExceptionAsync(e);
                }
            }
        }

        private async void ExecuteAddAttachmentCommand()
        {
            var pickerService = Acumatica.Core.Ioc.Container.Default.GetInstance<Acumatica.Core.Service.IFileService>();
            var file = await pickerService.PickSingleFile();
            if (file != null)
            {
                _model.Attachments.Add(new Attachment(file.Name, file.Contents));
            }
        }

        #pragma warning disable 4014
        private async void ExecuteLoadExpenseItemsCommand()
        {
            if (_expenseItems.Loaded || _expenseItems.Loading) return;
            _expenseItems.Loading = true;
            var expenseItemsService = Acumatica.Core.Ioc.Container.Default.GetInstance<IExpenseItemsService>();

            try
            {
                foreach (var expenseItem in await expenseItemsService.GetExpenseItems())
                {
                    _expenseItems.Add(expenseItem);
                }
                _expenseItems.Loaded = true;
            }
            catch (Exception e)
            {
                Acumatica.Core.Ioc.Container.Default.GetInstance<IExceptionHandlerService>().HandleExceptionAsync(e);
            }
            finally
            {
                _expenseItems.Loading = false;
            }
        }

        #pragma warning disable 4014
        private async void ExecuteLoadCustomersCommand()
        {
            if (_customers.Loaded || _customers.Loading) return;
            _customers.Loading = true;
            var customersService = Acumatica.Core.Ioc.Container.Default.GetInstance<ICustomerService>();

            try
            {
                foreach (var customer in await customersService.GetCustomers())
                {
                    _customers.Add(customer);
                }
                _customers.Loaded = true;
            }
            catch (Exception e)
            {
                Acumatica.Core.Ioc.Container.Default.GetInstance<IExceptionHandlerService>().HandleExceptionAsync(e);
            }
            finally
            {
                _customers.Loading = false;
            }
        }

        #pragma warning disable 4014
        private async void ExecuteLoadProjectsCommand()
        {
            if (_projects.Loaded || _projects.Loading) return;
            _projects.Loading = true;
            var projectsService = Acumatica.Core.Ioc.Container.Default.GetInstance<IProjectService>();

            try
            {
                foreach (var project in await projectsService.GetProjects())
                {
                    _projects.Add(project);
                }
                _projects.Loaded = true;
            }
            catch (Exception e)
            {
                Acumatica.Core.Ioc.Container.Default.GetInstance<IExceptionHandlerService>().HandleExceptionAsync(e);
            }
            finally
            {
                _projects.Loading = false;
            }
        }

        private async void ExecuteLoadProjectTasksCommand()
        {
            if (_projectTasks.Loading) return;
            _projectTasks.Clear();
            _projectTasks.Loading = true;
            var projectsService = Acumatica.Core.Ioc.Container.Default.GetInstance<IProjectService>();

            try
            {
                foreach (var projectTask in await projectsService.GetProjectTasks(_model.Project))
                {
                    _projectTasks.Add(projectTask);
                }
                _projectTasks.Loaded = true;
            }
            catch (Exception e)
            {
                Acumatica.Core.Ioc.Container.Default.GetInstance<IExceptionHandlerService>().HandleExceptionAsync(e);
            }
            finally
            {
                _projectTasks.Loading = false;
            }
        }

        private async void ExecuteLoadCustomerLocationsCommand()
        {
            if (_customerLocations.Loading) return;

            _customerLocations.Clear();
            _customerLocations.Loading = true;
            var customersService = Acumatica.Core.Ioc.Container.Default.GetInstance<ICustomerService>();

            try
            {
                foreach (var location in await customersService.GetCustomerLocations(_model.Customer))
                {
                    _customerLocations.Add(location);
                }
            }
            catch (Exception e)
            {
                Acumatica.Core.Ioc.Container.Default.GetInstance<IExceptionHandlerService>().HandleExceptionAsync(e);
            }
            finally
            {
                _customerLocations.Loading = false;
            }
        }

        private bool SaveLine()
        {
            if (DoValidate())
            {
                var cache = Acumatica.Core.Ioc.Container.Default.GetInstance<IExpenseClaimLineCache>();
                cache.CurrentLine = _model;
                return true;
            }
            else
            {
                return false;
            }
        }


        public ExpenseClaimLine Model
        {
            get
            {
                return _model;
            }
        }

        public LoadingObservableCollection<Customer> Customers
        {
            get
            {
                return _customers;
            }

        }

        public LoadingObservableCollection<CustomerLocation> CustomerLocations
        {
            get
            {
                return _customerLocations;
            }

        }


        public LoadingObservableCollection<Project> Projects
        {
            get
            {
                return _projects;
            }

        }

        public LoadingObservableCollection<ProjectTask> ProjectTasks
        {
            get
            {
                return _projectTasks;
            }
        }

        public LoadingObservableCollection<ExpenseItem> ExpenseItems
        {
            get
            {
                return _expenseItems;
            }
        }

        public override void LoadState(object navigationParameter, Dictionary<String, Object> viewData)
        {
            if (viewData != null && viewData.ContainsKey("CurrentLine"))
            {
                _model = (ExpenseClaimLine)viewData["CurrentLine"];
                _model.TrackChanges();
            }
            else
            {
                var cache = Acumatica.Core.Ioc.Container.Default.GetInstance<IExpenseClaimLineCache>();
                System.Diagnostics.Debug.Assert(cache.CurrentLine != null, "An expense claim line should be stored in the cache before we are loaded");
                if (cache.CurrentLine != null)
                {
                    _model = Acumatica.Core.Helpers.ObjectCloner.Clone<ExpenseClaimLine>(cache.CurrentLine);
                    _model.TrackChanges();
                }
            }

            if(_model != null)
            {
                PageTitle = string.Format("Expense Claim {0} Line", _model.ParentRefNbr);
            }
        }

        public override void SaveState(Dictionary<string, object> viewData)
        {
            base.SaveState(viewData);
            viewData.Add("CurrentLine", _model);
        }


        protected async override Task<bool> ConfirmGoBack()
        {
            var cache = Acumatica.Core.Ioc.Container.Default.GetInstance<IExpenseClaimLineCache>();
            if (_model.HasUnsavedChanges)
            {
                var dialog = Acumatica.Core.Ioc.Container.Default.GetInstance<IMessageDialogService>();
                dialog.Title = "Unsaved Changes";
                dialog.Content = "Do you want to save the changes made to this line before continuing?";
                dialog.Buttons = MessageDialogButtons.YesNoCancel;
                var result = await dialog.ShowAsync();

                if (result == MessageDialogResult.Yes)
                {
                    return SaveLine();
                }
                else if (result == MessageDialogResult.Cancel)
                {
                    return false;
                }
                else
                {
                    // Clear line from cache so that it is not merged by parent.
                    cache.CurrentLine = null;
                    return true;
                }
            }
            else
            {
                return true;
            }
        }
    }
}
