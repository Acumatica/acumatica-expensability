using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Acumatica.Core.Service
{
    public interface IExceptionHandlerService
    {
        Task HandleExceptionAsync(Exception ex);
    }
}
