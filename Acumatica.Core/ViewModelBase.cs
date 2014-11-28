using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Acumatica.Core.Command;
using Acumatica.Core.Service;
using Acumatica.Core.Validation;

namespace Acumatica.Core
{
    public abstract class ViewModelBase : ModelBase, IStateAware
    {
        bool _loading;
        public ICommand GoBackCommand { get; private set; }

        public ViewModelBase()
        {
            InitializeCommands();
        }

        public bool IsInDesignMode
        {
            get
            {
                var service = Acumatica.Core.Ioc.Container.Default.GetInstance<IApplicationStateService>();
                return service.IsInDesignMode;
            }
        }

        private void InitializeCommands()
        {
            GoBackCommand = new RelayCommand(ExecuteGoBackCommand);
        }

        private async void ExecuteGoBackCommand()
        {
            bool confirmed = await ConfirmGoBack();
            if (confirmed)
            {
                Acumatica.Core.Ioc.Container.Default.GetInstance<INavigationService>().GoBack();
            }
        }

        protected virtual Task<bool> ConfirmGoBack()
        {
            return Task.Factory.StartNew<bool>(() => { return true; } );
        }

        public bool Loading
        {
            get { return _loading; }
            set { SetProperty(ref _loading, value); }
        }

        #pragma warning disable 4014
        public bool DoValidate()
        {
            if (!ValidationService.DoValidate())
            {
                var dialog = Acumatica.Core.Ioc.Container.Default.GetInstance<IMessageDialogService>();
                dialog.Title = "Validation Failed";
                dialog.Content = "One or more fields are incorrect. Please check the fields that are highlighted in red.";
                dialog.Buttons = MessageDialogButtons.OK;
                dialog.ShowAsync();
                return false;
            }
            else
            {
                return true;
            }
        }

        public IValidationService ValidationService
        {
            get
            {
                return Acumatica.Core.Ioc.Container.Default.GetInstance<IValidationService>();
            }
        }

        public virtual void LoadState(object navigationParameter, Dictionary<string, object> viewData)
        {
        }

        public virtual void SaveState(Dictionary<string, object> viewData)
        {
        }
    }
}
