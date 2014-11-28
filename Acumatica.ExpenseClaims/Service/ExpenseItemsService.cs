using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Acumatica.ExpenseClaims.Model;
using Acumatica.ExpenseClaims.IN202000;
using System.IO;

namespace Acumatica.ExpenseClaims.Service
{
    public class ExpenseItemsService : IExpenseItemsService
    {
        private async Task<Content> GetSavedSchema()
        {
            System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(IN202000.Content));

            var file = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFileAsync(@"Schemas\IN202000.xml");
            var stream = await file.OpenStreamForReadAsync();
            return (Content)serializer.Deserialize(stream.AsInputStream().AsStreamForRead());
        }

        public async Task<IList<ExpenseItem>> GetExpenseItems()
        {
            ScreenSoapClient client = await GetAuthenticatedClient(Common.SettingsStore.SiteUrl, Common.SettingsStore.Username, Common.SettingsStore.Password);
            var schema = await GetSavedSchema();
            await client.SetSchemaAsync(schema); 

            var result = await client.ExportAsync(new Command[] {
                    schema.NonStockItemSummary.ServiceCommands.EveryInventoryID,
                    schema.NonStockItemSummary.InventoryID,
                    schema.NonStockItemSummary.Description

                },
                new Filter[] { new Filter { Field = schema.GeneralSettingsItemDefaults.Type, Condition = FilterCondition.Equals, Value = "Expense", Operator = FilterOperator.And },
                               /*TODO: not working as it should new Filter { Field = schema.NonStockItemSummary.ItemStatus, Condition = FilterCondition.Equals, Value = "Active", Operator = FilterOperator.And }*/},
                0, false, true);

            IList<ExpenseItem> list = new List<ExpenseItem>();
            for (int i = 0; i < result.ExportResult.Length; i++)
            {
                list.Add(new ExpenseItem(result.ExportResult[i][0].Trim(), result.ExportResult[i][1]));
            }
            return list;
        }

        public async Task<string> GetExpenseItemDescription(string inventoryID)
        {
            ScreenSoapClient client = await GetAuthenticatedClient(Common.SettingsStore.SiteUrl, Common.SettingsStore.Username, Common.SettingsStore.Password);
            var schema = await GetSavedSchema();
            await client.SetSchemaAsync(schema); 

            var result = await client.ExportAsync(new Command[] {
                    new Value { Value = inventoryID, LinkedCommand = schema.NonStockItemSummary.InventoryID},
                    schema.NonStockItemSummary.Description
                },
                null,
                0, false, true);

            if(result.ExportResult.Length == 0)
            {
                return String.Empty;
            }
            else
            {
                return result.ExportResult[0][0];
            }
        }

        private async Task<ScreenSoapClient> GetAuthenticatedClient(string siteUrl, string username, string password)
        {
            var client = new ScreenSoapClient(ServiceHelpers.GetDefaultBinding(siteUrl), ServiceHelpers.GetServiceAddress(siteUrl, "IN202000"));
            await client.LoginAsync(username, password);
            return client;
        }
    }
}
