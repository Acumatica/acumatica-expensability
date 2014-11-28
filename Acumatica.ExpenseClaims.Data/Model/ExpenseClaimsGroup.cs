using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Acumatica.ExpenseClaims.Model
{
    public class ExpenseClaimGroup
    {
        private ObservableCollection<ExpenseClaimBase> _items = new ObservableCollection<ExpenseClaimBase>();

        public ExpenseClaimGroup(string groupTitle)
        {
            GroupTitle = groupTitle;
        }

        public string GroupTitle { get; private set; }

        public ObservableCollection<ExpenseClaimBase> Items 
        {
            get
            {
                return _items;
            }
        }
    }
}
