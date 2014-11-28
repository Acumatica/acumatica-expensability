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
    public class ExpenseClaimViewModel : ViewModelBase
    {
        private string _pageTitle = "Expense Claim 000000000000"; // For an unknown reason, initial title must be wide enough to accomodate the title that we will set in LoadTitle
        private ExpenseClaim _model;

        private static LoadingObservableCollection<Employee> _employees = new LoadingObservableCollection<Employee>();

        public RelayCommand AddExpenseCommand { get; private set; }
        public RelayCommand<ExpenseClaimLine> ShowExpenseClaimLine { get; private set; }
        public RelayCommand SaveExpenseClaimCommand { get; private set; }
        public RelayCommand AddAttachmentCommand { get; private set; }

        public RelayCommand LoadEmployeesCommand { get; private set; }

        public ValidationRule<string> HeaderDescriptionValidation { get; private set; }
        public ValidationRule<string> ClaimedByValidation { get; private set; }
        
        /// <summary>
        /// Initializes a new instance of the ExpenseClaimViewModel class.
        /// </summary>
        public ExpenseClaimViewModel()
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
            _model = await service.GetExpenseClaim("12345");
            OnPropertyChanged("Model");
        }

        private void MergeLineFromCache(ExpenseClaimLine line)
        {
            bool isFound = false;
            for (int i = 0; i < _model.Lines.Count; i++)
            {
                if (_model.Lines[i].LineNbr == line.LineNbr)
                {
                    if (line.Deleted == true)
                    {
                        _model.Lines.RemoveAt(i);
                        _model.DeletedLines.Add(line);
                    }
                    else
                    {
                        _model.Lines[i] = line;
                    }
                    isFound = true;
                    break;
                }
            }

            if (!isFound && !line.Deleted)
            {
                //This is a newly added line
                _model.Lines.Add(line);
            }
        }

        private void InitializeValidationRules()
        {
            // Validation messages has to be short enough to fit when in snapped view since they don't wrap.
            HeaderDescriptionValidation = new ValidationRule<string>(ErrorType.Critical, "Required. Maximum 60 characters.", () => !(string.IsNullOrEmpty(HeaderDescriptionValidation.Value) || HeaderDescriptionValidation.Value.Length > 60));
            ClaimedByValidation = new ValidationRule<string>(ErrorType.Critical, "Required.", () => !(string.IsNullOrEmpty(ClaimedByValidation.Value)));
        }

        private void InitializeCommands()
        {
            AddExpenseCommand = new RelayCommand(ExecuteAddExpenseCommand);
            ShowExpenseClaimLine = new RelayCommand<ExpenseClaimLine>(ExecuteShowExpenseClaimLine);
            SaveExpenseClaimCommand = new RelayCommand(ExecuteSaveExpenseClaimCommand);
            AddAttachmentCommand = new RelayCommand(ExecuteAddAttachmentCommand);

            LoadEmployeesCommand = new RelayCommand(ExecuteLoadEmployeesCommand);  
        }

        protected async override Task<bool> ConfirmGoBack()
        {
            if (_model.HasUnsavedChanges)
            {
                var dialog = Acumatica.Core.Ioc.Container.Default.GetInstance<IMessageDialogService>();
                dialog.Title = "Unsaved Changes";
                dialog.Content = "Do you want to save the changes made to this expense claim before continuing?";
                dialog.Buttons = MessageDialogButtons.YesNoCancel;
                var result = await dialog.ShowAsync();

                if (result == MessageDialogResult.Yes)
                {
                    bool saved = await SaveExpenseClaim();
                    return saved;
                }
                else if (result == MessageDialogResult.Cancel)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return true;
            }
        }

        private void ExecuteAddExpenseCommand()
        {
            var newLine = new ExpenseClaimLine();
            newLine.AllowEdit = true;
            newLine.ParentRefNbr = Model.RefNbr;
            newLine.LineNbr = Model.Lines.Max(l => l.LineNbr).GetValueOrDefault(0) + 1;
            newLine.Date = System.DateTime.Today;

            var cache = Acumatica.Core.Ioc.Container.Default.GetInstance<IExpenseClaimLineCache>();
            cache.CurrentLine = newLine;

            Acumatica.Core.Ioc.Container.Default.GetInstance<Acumatica.Core.Service.INavigationService>().NavigateTo("ExpenseClaimLinePage", null);
        }

        private void ExecuteShowExpenseClaimLine(ExpenseClaimLine line)
        {
            var cache = Acumatica.Core.Ioc.Container.Default.GetInstance<IExpenseClaimLineCache>();
            cache.CurrentLine = line;
            cache.CurrentLine.HasUnsavedChanges = false;

            Acumatica.Core.Ioc.Container.Default.GetInstance<Acumatica.Core.Service.INavigationService>().NavigateTo("ExpenseClaimLinePage", null);
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
        private async void ExecuteLoadEmployeesCommand()
        {
            if (_employees.Loaded || _employees.Loading) return;
            _employees.Loading = true;
            var employeeService = Acumatica.Core.Ioc.Container.Default.GetInstance<IEmployeeService>();

            try
            {
                foreach (var employee in await employeeService.GetEmployees())
                {
                    _employees.Add(employee);
                }
                _employees.Loaded = true;
            }
            catch (Exception e)
            {
                Acumatica.Core.Ioc.Container.Default.GetInstance<IExceptionHandlerService>().HandleExceptionAsync(e);
            }
            finally
            {
                _employees.Loading = false;
            }
        }

        #pragma warning disable 4014
        private async void ExecuteSaveExpenseClaimCommand()
        {
            if (await SaveExpenseClaim())
            {
                Acumatica.Core.Ioc.Container.Default.GetInstance<Acumatica.Core.Service.INavigationService>().GoBack();
            }
        }

        private async Task<bool> SaveExpenseClaim()
        {
            if (DoValidate())
            {
                try
                {
                    Loading = true;
                    var service = Acumatica.Core.Ioc.Container.Default.GetInstance<IExpenseClaimService>();
                    await service.SaveExpenseClaim(Model);
                    Messenger.Default.Send<ExpenseClaimUpdated>(new ExpenseClaimUpdated(Model));
                    return true;
                }
                catch (System.Exception e)
                {
                    Acumatica.Core.Ioc.Container.Default.GetInstance<IExceptionHandlerService>().HandleExceptionAsync(e);
                    return false;
                }
                finally
                {
                    Loading = false;
                }
            }
            else
            {
                return false;
            }
        }

        public string LinesTotal
        {
            get
            {
                decimal total = 0;
                if(Model != null) total = Model.Lines.Sum(i=>i.Total);
                return String.Format("Total Claim: {0:c}", total);
            }
        }

        public LoadingObservableCollection<Employee> Employees
        {
            get
            {
                return _employees;
            }

        }

        public ExpenseClaim Model        
        {
            get
            {
                return _model;
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

        public async override void LoadState(object navigationParameter, Dictionary<String, Object> viewData)
        {
            Exception error = null;

            try
            {
                Loading = true;
                PageTitle = string.Format("Expense Claim {0}", (string) navigationParameter);

                if (viewData != null && viewData.ContainsKey("CurrentExpenseClaim"))
                {
                    _model = (ExpenseClaim)viewData["CurrentExpenseClaim"];
                    _model.TrackChanges();

                    var cache = Acumatica.Core.Ioc.Container.Default.GetInstance<IExpenseClaimLineCache>();
                    if (cache.CurrentLine != null)
                    {
                        if (cache.CurrentLine.HasUnsavedChanges)
                        {
                            this.Model.HasUnsavedChanges = true;
                        }
                        MergeLineFromCache(cache.CurrentLine);
                        cache.CurrentLine = null;
                    }
                }
                else
                {
                    var service = Acumatica.Core.Ioc.Container.Default.GetInstance<IExpenseClaimService>();
                    _model = await service.GetExpenseClaim(navigationParameter as string);
                }

                OnPropertyChanged("Model");
                OnPropertyChanged("LinesTotal");
            }
            catch (Exception e)
            {
                error = e;
            }
            finally
            {
                Loading = false;
            }

            if (error != null)
            {
                await Acumatica.Core.Ioc.Container.Default.GetInstance<IExceptionHandlerService>().HandleExceptionAsync(error);
                Acumatica.Core.Ioc.Container.Default.GetInstance<Acumatica.Core.Service.INavigationService>().GoBack();
            }
        }

        public override void SaveState(Dictionary<string,object> viewData)
        {
            base.SaveState(viewData);
            viewData.Add("CurrentExpenseClaim", _model); 
        }
    }
}