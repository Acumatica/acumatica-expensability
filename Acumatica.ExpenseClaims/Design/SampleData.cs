using Acumatica.ExpenseClaims.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Acumatica.ExpenseClaims.Design
{
    public class SampleData
    {
        public SampleData()
        {
            var designService = new DesignExpenseClaimService();
            foreach (var item in designService.GetExpenseClaimsForStatus("Open").Result)
            {
                ExpenseClaims.Add(item);
            }
            foreach (var item in designService.GetMostRecentExpenseClaimsByStatus().Result)
            {
                GroupedExpenseClaims.Add(item);
            }
        }

        private ObservableCollection<ExpenseClaimBase> _expenseClaims = new ObservableCollection<ExpenseClaimBase>();
        public ObservableCollection<ExpenseClaimBase> ExpenseClaims
        {
            get { return this._expenseClaims; }
        }

        private ObservableCollection<ExpenseClaimGroup> _groupedExpenseClaims = new ObservableCollection<ExpenseClaimGroup>();
        public ObservableCollection<ExpenseClaimGroup> GroupedExpenseClaims
        {
            get { return this._groupedExpenseClaims; }
        }
    }
}
