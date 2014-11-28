using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Acumatica.ExpenseClaims.PM301000;
using Acumatica.ExpenseClaims.Model;
using System.IO;

namespace Acumatica.ExpenseClaims.Service
{
    public class ProjectService : IProjectService
    {
        private async Task<Content> GetSavedSchema()
        {
            System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(PM301000.Content));

            var file = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFileAsync(@"Schemas\PM301000.xml");
            var stream = await file.OpenStreamForReadAsync();
            return (Content)serializer.Deserialize(stream.AsInputStream().AsStreamForRead());
        }

        public async Task<IList<Project>> GetProjects()
        {
            ScreenSoapClient client = await GetAuthenticatedClient(Common.SettingsStore.SiteUrl, Common.SettingsStore.Username, Common.SettingsStore.Password);
            var schema = await GetSavedSchema();
            await client.SetSchemaAsync(schema); 

            var result = await client.ExportAsync(new Command[] {
                    schema.ProjectSummary.ServiceCommands.EveryProjectID,
                    schema.ProjectSummary.ProjectID,
                    schema.ProjectSummary.Description
                },
                new Filter[] { new Filter { Field = schema.ProjectSummary.Status, Condition = FilterCondition.Equals, Value = "Active" } },
                0, false, true);

            IList<Project> list = new List<Project>();
            for (int i = 0; i < result.ExportResult.Length; i++)
            {
                list.Add(new Project(result.ExportResult[i][0].Trim(), result.ExportResult[i][1]));
            }
            return list;
        }

        public async Task<IList<ProjectTask>> GetProjectTasks(string projectID)
        {
            ScreenSoapClient client = await GetAuthenticatedClient(Common.SettingsStore.SiteUrl, Common.SettingsStore.Username, Common.SettingsStore.Password);
            var schema = await GetSavedSchema();
            await client.SetSchemaAsync(schema); 

            var result = await client.ExportAsync(new Command[] {
                    new Value { Value = projectID, LinkedCommand = schema.ProjectSummary.ProjectID},
                    schema.Tasks.TaskID,
                    schema.Tasks.Description
                },
                new Filter[] { new Filter { Field = schema.Tasks.Status, Condition = FilterCondition.Equals, Value = "Active" } },
                0, false, true);

            IList<ProjectTask> list = new List<ProjectTask>();
            for (int i = 0; i < result.ExportResult.Length; i++)
            {
                list.Add(new ProjectTask(result.ExportResult[i][0].Trim(), result.ExportResult[i][1]));
            }
            return list;
        }


        private async Task<ScreenSoapClient> GetAuthenticatedClient(string siteUrl, string username, string password)
        {
            var client = new ScreenSoapClient(ServiceHelpers.GetDefaultBinding(siteUrl), ServiceHelpers.GetServiceAddress(siteUrl, "PM301000"));
            await client.LoginAsync(username, password);
            return client;
        }
    }
}
