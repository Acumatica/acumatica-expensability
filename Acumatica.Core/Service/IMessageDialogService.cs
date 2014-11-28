using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Acumatica.Core.Service
{
    [Flags]
    public enum MessageDialogButtons
    {
        OK = 1,
        Cancel = 2,
        OKCancel = OK | Cancel,
        Yes = 4,
        No = 8,
        YesNo = Yes | No,
        YesNoCancel = Yes | No | Cancel
    }

    public enum MessageDialogResult
    {
        None = 0,
        OK = 1,
        Cancel = 2,
        Yes = 6,
        No = 7
    }

    public interface IMessageDialogService
    {
        Task<MessageDialogResult> ShowAsync();

        MessageDialogButtons Buttons { get; set; }

        string Content { get; set; }
        string Title { get; set; }
    }
}
