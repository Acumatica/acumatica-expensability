using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Acumatica.ExpenseClaims.Model;

namespace Acumatica.ExpenseClaims.Service
{
    public interface IExpenseClaimService
    {
        Task Login(string siteUrl, string username, string password);
        void Logout();

        Task<IList<ExpenseClaimGroup>> GetMostRecentExpenseClaimsByStatus();
        Task<IList<ExpenseClaimBase>> GetExpenseClaimsForStatus(string status);
        Task<IList<ExpenseClaimBase>> SearchExpenseClaims(string searchText);
        Task<ExpenseClaim> GetExpenseClaim(string refNbr);
        Task SaveExpenseClaim(ExpenseClaim expenseClaim);
        string CurrentLoginName { get; }
    }
}
