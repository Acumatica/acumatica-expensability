using Acumatica.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Acumatica.ExpenseClaims.Model
{
    [DataContract]
    public class ExpenseClaimBase : TrackableModelBase
    {
        private string _description;
        private string _employee;
        private string _refNbr;
        private decimal _total;
        private DateTime _date;
        private string _status;

        public ExpenseClaimBase(string description, string employee, string refNbr, decimal total, DateTime date, string status)
        {
            _description = description;
            _employee = employee;
            _refNbr = refNbr;
            _total = total;
            _date = date;
            _status = status;
        }

        [DataMember]
        public decimal Total
        {
            get
            {
                return _total;
            }
            set
            {
                SetProperty(ref _total, value);
            }
        }

        [DataMember]
        public string Description
        {
            get
            {
                return _description;
            }
            set
            {
                SetProperty(ref _description, value);
            }

        }

        [DataMember]
        public string Employee
        {
            get
            {
                return _employee;
            }
            set
            {
                SetProperty(ref _employee, value);
            }
        }

        [DataMember]
        public string Status
        {
            get
            {
                return _status;
            }
            set
            {
                SetProperty(ref _status, value);
            }
        }

        [DataMember]
        public DateTime Date
        {
            get
            {
                return _date;
            }
            set
            {
                SetProperty(ref _date, value);
            }
        }

        [DataMember]
        public string RefNbr
        {
            get
            {
                return _refNbr;
            }
            set
            {
                SetProperty(ref _refNbr, value);
            }
        }
    }
}
