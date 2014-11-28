using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Acumatica.ExpenseClaims.Model
{
    public class ProjectTask
    {
        private string _projectTaskID;
        private string _description;

        public ProjectTask(string projectTaskID, string description)
        {
            _projectTaskID = projectTaskID;
            _description = description;
        }

        public string ProjectTaskID
        {
            get
            {
                return _projectTaskID;
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
            return String.Format("{0} - {1}", _projectTaskID, _description);
        }
    }
}
