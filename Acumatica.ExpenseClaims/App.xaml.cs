using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Acumatica.ExpenseClaims.Common;
using Acumatica.Core.Windows;
using Windows.UI.ApplicationSettings;
using Windows.System;
using System.Reflection;
using Windows.UI.Popups;
using Windows.UI.Xaml.Media.Animation;

namespace Acumatica.ExpenseClaims
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            InitializeComponent();
            Suspending += OnSuspending;

            Acumatica.Core.Windows.SuspensionManager.KnownTypes.Add(typeof(Model.ExpenseClaim));
            Acumatica.Core.Windows.SuspensionManager.KnownTypes.Add(typeof(Model.ExpenseClaimLine));
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used when the application is launched to open a specific file, to display
        /// search results, and so forth.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            ActivateApplication(args);
        }

        /// <summary>
        /// Invoked when the application is activated to display search results.
        /// </summary>
        /// <param name="args">Details about the activation request.</param>
        /// 
        protected override void OnSearchActivated(Windows.ApplicationModel.Activation.SearchActivatedEventArgs args)
        {
            ActivateApplication(args);
        }

        private async void ActivateApplication(IActivatedEventArgs args)
        {
            var frame = Window.Current.Content as Frame;            
            if (frame == null)
            {
                // Initial launch of the app
                frame = new Frame();
                RegisterServices(frame);

                SettingsPane.GetForCurrentView().CommandsRequested += App_CommandsRequested;

                //Associate the frame with a SuspensionManager key                                
                Acumatica.Core.Windows.SuspensionManager.RegisterFrame(frame, "AppFrame");
                if (args.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    // Restore the saved session state only when appropriate
                    try
                    {
                        await SuspensionManager.RestoreAsync();
                    }
                    catch (SuspensionManagerException)
                    {
                        //Something went wrong restoring state.
                        //Assume there is no state and continue
                    }
                }
            }

            Window.Current.Content = frame;

            if (Acumatica.Core.Windows.Helpers.InternetConnectionHelper.IsConnected)
            {
                if (!String.IsNullOrEmpty(Common.SettingsStore.SiteUrl) &&
                     !String.IsNullOrEmpty(Common.SettingsStore.Username) &&
                     !String.IsNullOrEmpty(Common.SettingsStore.Password))
                {
                    if (args is SearchActivatedEventArgs)
                    {
                        if (!frame.Navigate(typeof(ExpenseClaimsListPage), "search:" + ((SearchActivatedEventArgs)args).QueryText.Trim())) throw new Exception("Failed to create initial page (MainPage)");
                    }
                    else
                    {
                        // When resuming from suspended state, we'll already have a frame so don't navigate at this time.
                        if (frame.Content == null)
                        {
                            if (!frame.Navigate(typeof(MainPage), null)) throw new Exception("Failed to create initial page (MainPage)");
                        }
                    }
                }
                else
                {
                    if (!frame.Navigate(typeof(SignInPage), null)) throw new Exception("Failed to create initial page (SignIn)");
                }
            }
            else
            {
                if (!frame.Navigate(typeof(SignInPage), null)) throw new Exception("Failed to create initial page (SignIn)");
            }

            // Ensure the current window is active
            Window.Current.Activate();
        }

        private void RegisterServices(Frame frame)
        {
            Acumatica.Core.Ioc.Container.Default.Register<Acumatica.Core.Service.IExceptionHandlerService, Acumatica.Core.Windows.Service.ExceptionHandlerService>();
            Acumatica.Core.Ioc.Container.Default.Register<Acumatica.Core.Service.INavigationService>(() => new Acumatica.Core.Windows.Service.NavigationService(typeof(App).GetTypeInfo().Assembly, "Acumatica.ExpenseClaims", frame), true);
            Acumatica.Core.Ioc.Container.Default.Register<Acumatica.Core.Service.ISettingsService, Acumatica.Core.Windows.Service.SettingsService>();
            Acumatica.Core.Ioc.Container.Default.Register<Acumatica.Core.Service.IUriLauncherService, Acumatica.Core.Windows.Service.UriLauncherService>();
            Acumatica.Core.Ioc.Container.Default.Register<Acumatica.Core.Service.IMessageDialogService, Acumatica.Core.Windows.Service.MessageDialogService>();
            Acumatica.Core.Ioc.Container.Default.Register<Acumatica.Core.Service.IFileService, Acumatica.Core.Windows.Service.FileService>();
            Acumatica.Core.Ioc.Container.Default.Register<Acumatica.Core.Validation.IValidationService, Acumatica.Core.Validation.ValidationService>();
        }

        #pragma warning disable 4014
        private void App_CommandsRequested(SettingsPane sender, SettingsPaneCommandsRequestedEventArgs args)
        {
            args.Request.ApplicationCommands.Add(
                        new SettingsCommand("privacypolicy", "Privacy Policy",
                            a =>
                            {
                                Launcher.LaunchUriAsync(new Uri("http://www.acumatica.com/company/policies/privacypolicy"));
                            }));

            args.Request.ApplicationCommands.Add(
                        new SettingsCommand("signout", "Sign Out",
                            a =>
                            {
                                Acumatica.Core.Ioc.Container.Default.GetInstance<Service.IExpenseClaimService>().Logout();
                                Acumatica.Core.Ioc.Container.Default.GetInstance<Acumatica.Core.Service.INavigationService>().NavigateTo("SignInPage", null);
                            }));

            args.Request.ApplicationCommands.Add(new SettingsCommand("support", "Support", (handler) =>
            {
                var settings = new Acumatica.Core.Windows.SettingsFlyout();
                settings.ShowFlyout(new SupportFlyout());
            }));
        }

       
        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private async void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            await Acumatica.Core.Windows.SuspensionManager.SaveAsync();
            deferral.Complete();
        }
    }
}
