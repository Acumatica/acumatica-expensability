using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Acumatica.ExpenseClaims.Model
{
    public class Employee
    {
        private string _employeeID;
        private string _name;

        public Employee(string employeeID, string name)
        {
            _employeeID = employeeID;
            _name = name;
        }

        public string EmployeeID
        {
            get
            {
                return _employeeID;
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
            return String.Format("{0} - {1}", _employeeID, _name);
        }
    }
}
