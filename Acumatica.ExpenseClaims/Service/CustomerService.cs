using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Acumatica.ExpenseClaims.AR303000;
using Acumatica.ExpenseClaims.Model;
using System.IO;

namespace Acumatica.ExpenseClaims.Service
{
    public class CustomerService : ICustomerService 
    {
        private async Task<Content> GetSavedSchema()
        {
            System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(AR303000.Content));

            var file = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFileAsync(@"Schemas\AR303000.xml");
            var stream = await file.OpenStreamForReadAsync();
            return (Content)serializer.Deserialize(stream.AsInputStream().AsStreamForRead());
        }

        public async Task<IList<Customer>> GetCustomers()
        {
            ScreenSoapClient client = await GetAuthenticatedClient(Common.SettingsStore.SiteUrl, Common.SettingsStore.Username, Common.SettingsStore.Password);
            var schema = await GetSavedSchema();
            await client.SetSchemaAsync(schema); 

            var result = await client.ExportAsync(new Command[] {
                    schema.CustomerSummary.ServiceCommands.EveryCustomerID,
                    schema.CustomerSummary.CustomerID,
                    schema.CustomerSummary.CustomerName,
                },
                new Filter[] { new Filter { Field = schema.CustomerSummary.Status, Condition = FilterCondition.Equals, Value = "Active" } },
                0, false, true);

            IList<Customer> list = new List<Customer>();
            for (int i = 0; i < result.ExportResult.Length; i++)
            {
                list.Add(new Customer(result.ExportResult[i][0].Trim(), result.ExportResult[i][1]));
            }
            return list;
        }

        public async Task<IList<CustomerLocation>> GetCustomerLocations(string customerID)
        {
            ScreenSoapClient client = await GetAuthenticatedClient(Common.SettingsStore.SiteUrl, Common.SettingsStore.Username, Common.SettingsStore.Password);
            var schema = await GetSavedSchema();
            await client.SetSchemaAsync(schema); 

            var result = await client.ExportAsync(new Command[] {
                    new Value { Value = customerID, LinkedCommand = schema.CustomerSummary.CustomerID},
                    schema.Locations.LocationID,
                    schema.Locations.LocationName,
                },
                new Filter[] { new Filter { Field = schema.Locations.Active, Condition = FilterCondition.Equals, Value = "True" } },
                0, false, true);

            IList<CustomerLocation> list = new List<CustomerLocation>();
            for (int i = 0; i < result.ExportResult.Length; i++)
            {
                list.Add(new CustomerLocation(result.ExportResult[i][0].Trim(), result.ExportResult[i][1]));
            }
            return list;
        }

        private async Task<ScreenSoapClient> GetAuthenticatedClient(string siteUrl, string username, string password)
        {
            var client = new ScreenSoapClient(ServiceHelpers.GetDefaultBinding(siteUrl), ServiceHelpers.GetServiceAddress(siteUrl, "AR303000"));
            await client.LoginAsync(username, password);
            return client;
        }
    }
}
