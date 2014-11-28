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
    public class SignInViewModel : ViewModelBase
    {
        private readonly IExpenseClaimService _dataService;

        public RelayCommand SignInCommand { get; private set; }
        public RelayCommand ForgotPasswordCommand { get; private set; }

        private string _siteUrl;
        private string _username;
        private string _password;

        /// <summary>
        /// Initializes a new instance of the SignInViewModel class.
        /// </summary>
        public SignInViewModel()
        {
            _dataService = Acumatica.Core.Ioc.Container.Default.GetInstance<IExpenseClaimService>();

            var settings = Acumatica.Core.Ioc.Container.Default.GetInstance<ISettingsService>();
            SiteUrl = settings.GetValue("SiteUrl") as string;
            Username = settings.GetValue("Username") as string;
            Password = settings.GetValue("Password") as string;

            InitializeCommands();
        }

        private void InitializeCommands()
        {
            SignInCommand = new RelayCommand(ExecuteSignInCommand);
            ForgotPasswordCommand = new RelayCommand(ExecuteForgotPasswordCommand);
        }

        public string SiteUrl
        {
            get
            {
                return _siteUrl;
            }
            set
            {
                SetProperty(ref _siteUrl, value);
            }
        }

        public string Username
        {
            get
            {
                return _username;
            }
            set
            {
                SetProperty(ref _username, value);
            }
        }


        public string Password
        {
            get
            {
                return _password;
            }
            set
            {
                SetProperty(ref _password, value);
            }
        }

        private async void ExecuteSignInCommand()
        {
            Exception error = null;
            
            try
            {
                if (await ValidateSiteAddress())
                {
                    Loading = true;

                    await _dataService.Login(SiteUrl, Username, Password);
                    var settings = Acumatica.Core.Ioc.Container.Default.GetInstance<ISettingsService>();
                    settings.SetValue("SiteUrl", SiteUrl);
                    settings.SetValue("Username", Username);
                    settings.SetValue("Password", Password);
                }
                else
                {
                    return;
                }
            }
            catch (Exception ex)
            {
                error = ex;
            }
            finally
            {
                Loading = false;
            }

            if (error == null)
            {
                Acumatica.Core.Ioc.Container.Default.GetInstance<Acumatica.Core.Service.INavigationService>().NavigateTo("MainPage", null);
            }
            else
            {
                await Acumatica.Core.Ioc.Container.Default.GetInstance<IExceptionHandlerService>().HandleExceptionAsync(error);
            }
        }

        private async void ExecuteForgotPasswordCommand()
        {
            if (await ValidateSiteAddress())
            {
                if (!SiteUrl.EndsWith("/"))
                {
                    SiteUrl = SiteUrl + "/";
                }

                var baseUri = new Uri(SiteUrl);
                var forgotPasswordUri = new Uri(String.Format("{0}Frames/PasswordRemind.aspx?ReturnUrl={1}Main.aspx&Target={1}Frames%2fLogin.aspx", SiteUrl, baseUri.AbsolutePath));
                await Acumatica.Core.Ioc.Container.Default.GetInstance<IUriLauncherService>().LaunchUriAsync(forgotPasswordUri);
            }
        }

        private async Task<bool> ValidateSiteAddress()
        {
            if (String.IsNullOrEmpty(SiteUrl))
            {
                var dialog = Acumatica.Core.Ioc.Container.Default.GetInstance<IMessageDialogService>();
                dialog.Title = "Address Missing";
                dialog.Content = "Please enter your Acumatica site address before your continue. This information is provided by your system administrator.";
                dialog.Buttons = MessageDialogButtons.OK;
                await dialog.ShowAsync();                
                return false;
            }
            else if (!Uri.IsWellFormedUriString(SiteUrl, UriKind.Absolute) || !(SiteUrl.StartsWith("http://", StringComparison.CurrentCultureIgnoreCase) || SiteUrl.StartsWith("https://", StringComparison.CurrentCultureIgnoreCase)))
            {
                var dialog = Acumatica.Core.Ioc.Container.Default.GetInstance<IMessageDialogService>();
                dialog.Title = "Invalid Address";
                dialog.Content = "Please enter a valid address (ex: http://erp.acumatica.com)";
                dialog.Buttons = MessageDialogButtons.OK;
                await dialog.ShowAsync();                
                return false;
            }
            {
                return true;
            }
        }
    }
}