using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Acumatica.ExpenseClaims.Model
{
    public class Project
    {
        private string _projectID;
        private string _description;

        public Project(string projectID, string description)
        {
            _projectID = projectID;
            _description = description;
        }

        public string ProjectID
        {
            get
            {
                return _projectID;
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
            return String.Format("{0} - {1}", _projectID, _description);
        }
    }
}
