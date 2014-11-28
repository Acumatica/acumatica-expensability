    using Acumatica.ExpenseClaims.Model;

namespace Acumatica.ExpenseClaims.Service
{
    public interface IExpenseClaimLineCache
    {
        ExpenseClaimLine CurrentLine { get; set; }
    }
}
