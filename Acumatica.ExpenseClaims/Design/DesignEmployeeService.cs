using Acumatica.ExpenseClaims.Model;
using Acumatica.ExpenseClaims.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Acumatica.ExpenseClaims.Design
{
    class DesignEmployeeService : IEmployeeService
    {
        public Task<IList<Employee>> GetEmployees()
        {
            return Task.Factory.StartNew<IList<Employee>>(() =>
            {
                var list = new List<Employee>();
                list.Add(new Employee("MICG", "Gabriel Michaud"));
                list.Add(new Employee("MORA", "Alexandre Morin"));
                return list;
            });
        }
    }
}
