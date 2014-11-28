using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Acumatica.ExpenseClaims.Model;

namespace Acumatica.ExpenseClaims.Service
{
    public interface IEmployeeService
    {
        Task<IList<Employee>> GetEmployees();
    }
}
