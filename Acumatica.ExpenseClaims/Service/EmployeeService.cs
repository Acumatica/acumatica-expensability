using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Acumatica.ExpenseClaims.EP203000;
using Acumatica.ExpenseClaims.Model;
using System.IO;

namespace Acumatica.ExpenseClaims.Service
{
    public class EmployeeService : IEmployeeService
    {
        private async Task<Content> GetSavedSchema()
        {
            System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(EP203000.Content));

            var file = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFileAsync(@"Schemas\EP203000.xml");
            var stream = await file.OpenStreamForReadAsync();
            return (Content)serializer.Deserialize(stream.AsInputStream().AsStreamForRead());
        }

        public async Task<IList<Employee>> GetEmployees()
        {
            ScreenSoapClient client = await GetAuthenticatedClient(Common.SettingsStore.SiteUrl, Common.SettingsStore.Username, Common.SettingsStore.Password);
            var schema = await GetSavedSchema();
            await client.SetSchemaAsync(schema); 

            var result = await client.ExportAsync(new Command[] {
                    schema.EmployeeInfo.ServiceCommands.EveryEmployeeID,
                    schema.EmployeeInfo.EmployeeID,
                    schema.GeneralInfoContactInfo.FirstName,
                    schema.GeneralInfoContactInfo.LastName
                },
                new Filter[] { new Filter { Field = schema.EmployeeInfo.Status, Condition = FilterCondition.Equals, Value = "Active" } },
                0, false, true);

            IList<Employee> list = new List<Employee>();
            for (int i = 0; i < result.ExportResult.Length; i++)
            {
                list.Add(new Employee(result.ExportResult[i][0].Trim(), result.ExportResult[i][1] + " " + result.ExportResult[i][2]));
            }
            return list;
        }

        private async Task<ScreenSoapClient> GetAuthenticatedClient(string siteUrl, string username, string password)
        {
            var client = new ScreenSoapClient(ServiceHelpers.GetDefaultBinding(siteUrl), ServiceHelpers.GetServiceAddress(siteUrl, "EP203000"));
            await client.LoginAsync(username, password);
            return client;
        }
    }
}
