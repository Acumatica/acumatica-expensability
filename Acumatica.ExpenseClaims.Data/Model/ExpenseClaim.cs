using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Acumatica.Core.Helpers;
using System.Runtime.Serialization;

namespace Acumatica.ExpenseClaims.Model
{
    [DataContract]
    public class ExpenseClaim : ExpenseClaimBase
    {
        private bool _hold;
        private DateTime? _approvalDate;
        private string _location;
        private string _noteText;
        private ObservableCollection<ExpenseClaimLine> _lines = new ObservableCollection<ExpenseClaimLine>();
        private ObservableCollection<ExpenseClaimLine> _deletedLines = new ObservableCollection<ExpenseClaimLine>();
        private ObservableCollection<Attachment> _attachments = new ObservableCollection<Attachment>();
        private bool _allowEdit;

        public ExpenseClaim(string description, string employee, string refNbr, decimal total, DateTime date, string status)
            : base(description, employee, refNbr, total, date, status)
        {
            
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
        public bool Hold
        {
            get
            {
                return _hold;
            }
            set
            {
                SetProperty(ref _hold, value);
            }
        }

        [DataMember]
        public DateTime? ApprovalDate
        {
            get
            {
                return _approvalDate;
            }
            set
            {
                SetProperty(ref _approvalDate, value);
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
        public ObservableCollection<ExpenseClaimLine> Lines
        {
            get
            {
                return _lines;
            }
            set
            {
                SetProperty(ref _lines, value);
            }
        }

        [DataMember]
        public ObservableCollection<ExpenseClaimLine> DeletedLines
        {
            get
            {
                return _deletedLines;
            }
            set
            {
                SetProperty(ref _deletedLines, value);
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
