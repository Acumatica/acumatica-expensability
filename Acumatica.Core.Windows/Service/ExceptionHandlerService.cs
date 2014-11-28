using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Acumatica.Core.Service;

namespace Acumatica.Core.Windows.Service
{
    public class ExceptionHandlerService : IExceptionHandlerService
    {
        public async Task HandleExceptionAsync(Exception ex)
        {
            string message = String.Empty;
            string title = "Oops!";

            if (ex is System.ServiceModel.FaultException)
            {
                if (ex.Message.Contains("PXUndefinedCompanyException"))
                {
                    message = "Unable to determine the company. Please specify the company name after your Username, for example John@Northwind";
                }
                else if (ex.Message.Contains("--->"))
                {
                    message = ExtractPXExceptionMessage(ex.Message);
                }
                else
                {
                    message = ex.Message;
                }
            }
            else if (ex is System.ServiceModel.EndpointNotFoundException || ex is System.ServiceModel.CommunicationException)
            {
                message = "Unable to connect to Acumatica. Please verify the address, check your network connection and try again.";
            }
            else
            {
                message = ex.Message;
            }

            var dialog = Acumatica.Core.Ioc.Container.Default.GetInstance<IMessageDialogService>();
            dialog.Buttons = MessageDialogButtons.OK;
            dialog.Title = title;
            dialog.Content = message;
            await dialog.ShowAsync();
        }

        private static string ExtractPXExceptionMessage(string message)
        {
            var regex = new System.Text.RegularExpressions.Regex(@"PX\w*Exception:\sError(?:\:|\s#[0-9]*:)\s((?<ErrorMessage>.*?)(?:-->\s(?<InnerException>.*))|(?<ErrorMessage>.*))");
            var matches = regex.Match(message);

            var match = matches.Groups["ErrorMessage"].Value;
            if (String.IsNullOrEmpty(match))
                return message;
            else
            {
                if (match.EndsWith(".."))
                {
                    return match.Substring(0, match.Length - 1);
                }
                else
                {
                    return match;
                }
            }
        }
    }
}
