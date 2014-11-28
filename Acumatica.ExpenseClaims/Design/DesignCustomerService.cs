using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Acumatica.ExpenseClaims.Service;
using Acumatica.ExpenseClaims.Model;

namespace Acumatica.ExpenseClaims.Design
{
    public class DesignCustomerService : ICustomerService
    {
        public Task<IList<Customer>> GetCustomers()
        {
            return Task.Factory.StartNew<IList<Customer>>(() =>
            {
                var list = new List<Customer>();
                list.Add(new Customer("ACUMATICA", "Acumatica Inc."));
                list.Add(new Customer("MICROSOFT", "Microsoft Corp."));
                return list;
            });
        }

        public Task<IList<CustomerLocation>> GetCustomerLocations(string customerID)
        {
            return Task.Factory.StartNew<IList<CustomerLocation>>(() =>
            {
                var list = new List<CustomerLocation>();
                list.Add(new CustomerLocation("MAIN", "Main Location"));
                return list;
            });
        }
    }
}
