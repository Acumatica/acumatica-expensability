using Acumatica.ExpenseClaims.Model;

namespace Acumatica.ExpenseClaims.Service
{
    public class ExpenseClaimLineCache : IExpenseClaimLineCache
    {
        public ExpenseClaimLine CurrentLine { get; set; }
    }
}
