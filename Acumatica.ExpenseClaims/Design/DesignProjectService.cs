using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Acumatica.ExpenseClaims.Service;
using Acumatica.ExpenseClaims.Model;

namespace Acumatica.ExpenseClaims.Design
{
    public class DesignProjectService : IProjectService
    {
        public Task<IList<Project>> GetProjects()
        {
            return Task.Factory.StartNew<IList<Project>>(() =>
            {
                var list = new List<Project>();
                list.Add(new Project("A", "Project A"));
                list.Add(new Project("B", "Project B"));
                return list;
            });
        }

        public Task<IList<ProjectTask>> GetProjectTasks(string projectID)
        {
            return Task.Factory.StartNew<IList<ProjectTask>>(() =>
            {
                var list = new List<ProjectTask>();
                list.Add(new ProjectTask("DESIGN", "Designing"));
                list.Add(new ProjectTask("PROGRAM", "Programming"));
                return list;
            });
        }
    }
}
