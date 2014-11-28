using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Acumatica.ExpenseClaims.Model
{
    public class CustomerLocation
    {
        private string _locationID;
        private string _name;

        public CustomerLocation(string locationID, string name)
        {
            _locationID = locationID;
            _name = name;
        }

        public string LocationID
        {
            get
            {
                return _locationID;
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
            return String.Format("{0} - {1}", _locationID, _name);
        }
    }
}
