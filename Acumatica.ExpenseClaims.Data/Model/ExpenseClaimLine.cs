using Acumatica.Core.Helpers;
using Acumatica.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;

namespace Acumatica.ExpenseClaims.Model
{
    [DataContract]
    public class ExpenseClaimLine : TrackableModelBase
    {
        private string _parentRefNbr;
        private bool _allowEdit;
        private bool _deleted;
        private int? _lineNbr;
        private DateTime _date;
        private string _refNbr;
        private string _expenseId;
        private decimal _quantity;
        private decimal _unitCost;
        private decimal _total;
        private decimal _employeePart;
        private bool _billable;
        private string _customer;
        private string _location;
        private string _project;
        private string _projectTask;
        private string _description;
        private string _noteText;

        private ObservableCollection<Attachment> _attachments = new ObservableCollection<Attachment>();

        [DataMember]
        public string ParentRefNbr
        {
            get
            {
                return _parentRefNbr;
            }
            set
            {
                SetProperty(ref _parentRefNbr, value);
            }
        }

        [DataMember]
        public bool AllowEdit
        {
            get
            {
                return _allowEdit;
            }
            set
            {
                SetProperty(ref _allowEdit, value);
            }
        }


        [DataMember]
        public bool Deleted
        {
            get
            {
                return _deleted;
            }
            set
            {
                SetProperty(ref _deleted, value);
            }
        }

        [DataMember]
        public int? LineNbr
        {
            get
            {
                return _lineNbr;
            }
            set
            {
                SetProperty(ref _lineNbr, value);
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

        [DataMember]
        public string ExpenseId
        {
            get
            {
                return _expenseId;
            }
            set
            {
                SetProperty(ref _expenseId, value);
            }
        }

        [DataMember]
        public decimal Quantity
        {
            get
            {
                return _quantity;
            }
            set
            {
                if (SetProperty(ref _quantity, value))
                {
                    Total = Quantity * UnitCost;
                }
            }
        }

        [DataMember]
        public decimal UnitCost
        {
            get
            {
                return _unitCost;
            }
            set
            {
                if (SetProperty(ref _unitCost, value))
                {
                    Total = Quantity * UnitCost;
                }
            }
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
        public decimal EmployeePart
        {
            get
            {
                return _employeePart;
            }
            set
            {
                SetProperty(ref _employeePart, value);
            }
        }

        [DataMember]
        public bool Billable
        {
            get
            {
                return _billable;
            }
            set
            {
                SetProperty(ref _billable, value);
            }
        }

        [DataMember]
        public string Customer
        {
            get
            {
                return _customer;
            }
            set
            {
                SetProperty(ref _customer, value);
            }
        }

        [DataMember]
        public string Location
        {
            get
            {
                return _location;
            }
            set
            {
                SetProperty(ref _location, value);
            }
        }

        [DataMember]
        public string Project
        {
            get
            {
                return _project;
            }
            set
            {
                SetProperty(ref  _project, value);
            }
        }

        [DataMember]
        public string ProjectTask
        {
            get
            {
                return _projectTask;
            }
            set
            {
                SetProperty(ref _projectTask, value);
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
        public string NoteText
        {
            get
            {
                return _noteText;
            }
            set
            {
                SetProperty(ref _noteText, value);
            }
        }

        [DataMember]
        public ObservableCollection<Attachment> Attachments
        {
            get
            {
                return _attachments;
            }
            set
            {
                SetProperty(ref _attachments, value);
            }
        }
    }
}
