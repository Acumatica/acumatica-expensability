using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Acumatica.ExpenseClaims.Model
{
    public class ExpenseItem
    {
        private string _inventoryID;
        private string _description;

        public ExpenseItem(string inventoryID, string description)
        {
            _inventoryID = inventoryID;
            _description = description;
        }

        public string InventoryID
        {
            get
            {
                return _inventoryID;
            }
        }

        public string Description
        {
            get
            {
                return _description;
            }
        }

        public override string ToString()
        {
            return String.Format("{0} - {1}", _inventoryID, _description);
        }
    }
}
