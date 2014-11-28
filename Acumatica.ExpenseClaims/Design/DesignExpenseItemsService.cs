using Acumatica.ExpenseClaims.Model;
using Acumatica.ExpenseClaims.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Acumatica.ExpenseClaims.Design
{
    public class DesignExpenseItemsService : IExpenseItemsService
    {
        public Task<IList<ExpenseItem>> GetExpenseItems()
        {
            return Task.Factory.StartNew<IList<ExpenseItem>>(() =>
            {
                var list = new List<ExpenseItem>();
                list.Add(new ExpenseItem("CARRENTAL", "Car Rental"));
                list.Add(new ExpenseItem("ACCOMODATION", "Accomodation"));
                return list;
            });
        }

        public Task<string> GetExpenseItemDescription(string inventoryID)
        {
            return Task.Factory.StartNew<string>(() => { return inventoryID + " description"; });
        }

    }
}
