using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Acumatica.Core.Service;
using Windows.UI.Popups;

namespace Acumatica.Core.Windows.Service
{
    public class MessageDialogService : IMessageDialogService
    {
        public async Task<MessageDialogResult> ShowAsync()
        {
            MessageDialog md = new MessageDialog(Content, Title);
            MessageDialogResult result = MessageDialogResult.None;

            if (Buttons.HasFlag(MessageDialogButtons.OK))
            {
                md.Commands.Add(new UICommand("OK",
                    new UICommandInvokedHandler((cmd) => result = MessageDialogResult.OK)));
            }
            if (Buttons.HasFlag(MessageDialogButtons.Yes))
            {
                md.Commands.Add(new UICommand("Yes",
                    new UICommandInvokedHandler((cmd) => result = MessageDialogResult.Yes)));
            }
            if (Buttons.HasFlag(MessageDialogButtons.No))
            {
                md.Commands.Add(new UICommand("No",
                    new UICommandInvokedHandler((cmd) => result = MessageDialogResult.No)));
            }
            if (Buttons.HasFlag(MessageDialogButtons.Cancel))
            {
                md.Commands.Add(new UICommand("Cancel",
                    new UICommandInvokedHandler((cmd) => result = MessageDialogResult.Cancel)));
                md.CancelCommandIndex = (uint)md.Commands.Count - 1;
            }

            var r = await md.ShowAsync();
            return result;
        }

        public MessageDialogButtons Buttons { get; set; }

        public string Content { get; set; }
        public string Title { get; set; }
    }
}
