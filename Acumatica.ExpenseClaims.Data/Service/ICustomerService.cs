using Acumatica.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Acumatica.ExpenseClaims.Model;

namespace Acumatica.ExpenseClaims.Service
{
    public interface ICustomerService
    {
        Task<IList<Customer>> GetCustomers();
        Task<IList<CustomerLocation>> GetCustomerLocations(string customerID);
    }
}
