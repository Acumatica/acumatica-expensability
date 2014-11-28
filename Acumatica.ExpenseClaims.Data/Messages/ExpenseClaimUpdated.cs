using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Acumatica.ExpenseClaims.Messages
{
    public class ExpenseClaimUpdated
    {
        public ExpenseClaimUpdated(Model.ExpenseClaim expenseClaim)
        {
            ExpenseClaim = expenseClaim;
        }

        public Model.ExpenseClaim ExpenseClaim { get; private set; }
    }
}
