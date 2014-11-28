using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Acumatica.ExpenseClaims.Model
{
    public class Customer
    {
        private string _customerID;
        private string _name;

        public Customer(string CustomerID, string name)
        {
            _customerID = CustomerID;
            _name = name;
        }

        public string CustomerID
        {
            get
            {
                return _customerID;
            }
        }

        public string Name
        {
            get
            {
                return _name;
            }
        }

        public override string ToString()
        {
            return String.Format("{0} - {1}", _customerID, _name);
        }
    }
}
