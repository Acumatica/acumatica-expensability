using System;
using Acumatica.ExpenseClaims;
using Acumatica.ExpenseClaims.Model;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Acumatica.ExpenseClaims.EP301000;
using System.IO;
using Acumatica.Core.Service;

namespace Acumatica.ExpenseClaims.Service
{
    public class ExpenseClaimService : IExpenseClaimService
    {
        private async Task<Content> GetSavedSchema()
        {
            System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(EP301000.Content));

            var file = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFileAsync(@"Schemas\EP301000.xml");
            var stream = await file.OpenStreamForReadAsync();
            return (Content)serializer.Deserialize(stream.AsInputStream().AsStreamForRead());
        }

        public async Task SaveExpenseClaim(ExpenseClaim expenseClaim)
        {
            ScreenSoapClient client = await GetAuthenticatedClient(Common.SettingsStore.SiteUrl, Common.SettingsStore.Username, Common.SettingsStore.Password);
            var schema = await GetSavedSchema();
            await client.SetSchemaAsync(schema);
            var commands = new List<Command>();

            // To prevent new row from being automatically added when we modify an existing one
            schema.ExpenseClaimDetails.ExpenseItem.LinkedCommand = null;

            commands.Add(new Value { Value = expenseClaim.RefNbr, LinkedCommand = schema.DocumentSummary.ReferenceNbr });
            commands.Add(new Value { Value = expenseClaim.Date.ToString(System.Globalization.CultureInfo.InvariantCulture), LinkedCommand = schema.DocumentSummary.Date });
            commands.Add(new Value { Value = expenseClaim.Description, LinkedCommand = schema.DocumentSummary.Description });
            commands.Add(new Value { Value = expenseClaim.Employee, LinkedCommand = schema.DocumentSummary.ClaimedBy });
            commands.Add(new Value { Value = expenseClaim.Hold.ToString(), LinkedCommand = schema.DocumentSummary.Hold });
            commands.Add(new Value { Value = expenseClaim.Location, LinkedCommand = schema.DocumentSummary.Location });
            commands.Add(new Value { Value = expenseClaim.NoteText, LinkedCommand = schema.DocumentSummary.NoteText });

            // Per Andrew Boulanov - to solve issue with being unable to add or update the line ExpenseId field:
            commands.Add(schema.DocumentSummaryRateSelection.CurrRateTypeID); //select to get currency info current reference populated
            commands.Add(new Field { FieldName = "BAccountID", ObjectName = "EPEmployee" }); //select to get employee current reference populated

            foreach (var attachment in expenseClaim.Attachments)
            {
                commands.Add(new Value { FieldName = attachment.FileName, Value = Convert.ToBase64String(attachment.Data), LinkedCommand = schema.DocumentSummary.ServiceCommands.Attachment });
            }

            foreach (var line in expenseClaim.DeletedLines)
            {
                if (line.LineNbr == null) continue; // This line had not been saved yet, no need to ask system to delete it
                commands.Add(new Key { Value = "='" + line.LineNbr.ToString() + "'", FieldName = "ClaimDetailID", ObjectName = "ExpenseClaimDetails" });
                commands.Add(schema.ExpenseClaimDetails.ServiceCommands.DeleteRow);
            }

            foreach (var line in expenseClaim.Lines)
            {
                if (line.LineNbr == null)
                {
                    commands.Add(schema.ExpenseClaimDetails.ServiceCommands.NewRow);
                }
                else
                {
                    commands.Add(new Key { Value = "='" + line.LineNbr.ToString() + "'", FieldName = "ClaimDetailID", ObjectName = "ExpenseClaimDetails" });
                }

                foreach (var attachment in line.Attachments)
                {
                    commands.Add(new Value { FieldName = attachment.FileName, Value = Convert.ToBase64String(attachment.Data), LinkedCommand = schema.ExpenseClaimDetails.ServiceCommands.Attachment });
                }

                commands.Add(new Value { Value = line.ExpenseId, LinkedCommand = schema.ExpenseClaimDetails.ExpenseItem });
                commands.Add(new Value { Value = line.Description, LinkedCommand = schema.ExpenseClaimDetails.Description });
                commands.Add(new Value { Value = line.Quantity.ToString(System.Globalization.CultureInfo.InvariantCulture), LinkedCommand = schema.ExpenseClaimDetails.Quantity });
                commands.Add(new Value { Value = line.UnitCost.ToString(System.Globalization.CultureInfo.InvariantCulture), LinkedCommand = schema.ExpenseClaimDetails.UnitCost });
                commands.Add(new Value { Value = line.Total.ToString(System.Globalization.CultureInfo.InvariantCulture), LinkedCommand = schema.ExpenseClaimDetails.TotalAmount });
                commands.Add(new Value { Value = line.EmployeePart.ToString(System.Globalization.CultureInfo.InvariantCulture), LinkedCommand = schema.ExpenseClaimDetails.EmployeePart });
                commands.Add(new Value { Value = line.Date.ToString(System.Globalization.CultureInfo.InvariantCulture), LinkedCommand = schema.ExpenseClaimDetails.Date });
                commands.Add(new Value { Value = line.Billable.ToString(), LinkedCommand = schema.ExpenseClaimDetails.Billable });
                commands.Add(new Value { Value = line.Customer, LinkedCommand = schema.ExpenseClaimDetails.Customer });
                commands.Add(new Value { Value = line.Location, LinkedCommand = schema.ExpenseClaimDetails.Location });
                commands.Add(new Value { Value = String.IsNullOrEmpty(line.Project) ? "X" : line.Project, LinkedCommand = schema.ExpenseClaimDetails.ProjectContract }); // Non-project code should be fetched from PM Preferences -- this is a shortcut
                commands.Add(new Value { Value = line.ProjectTask, LinkedCommand = schema.ExpenseClaimDetails.ProjectTask });
                commands.Add(new Value { Value = line.NoteText, LinkedCommand = schema.ExpenseClaimDetails.NoteText });
                commands.Add(new Value { Value = line.RefNbr, LinkedCommand = schema.ExpenseClaimDetails.RefNbr, Commit = true });
            }

            commands.Add(schema.Actions.Save);
            commands.Add(schema.DocumentSummary.ReferenceNbr);
            commands.Add(schema.DocumentSummary.Status);
            commands.Add(schema.DocumentSummary.ClaimTotal);

            var result = await client.SubmitAsync(commands.ToArray());
            expenseClaim.RefNbr = result[0].DocumentSummary.ReferenceNbr.Value;
            expenseClaim.Status = result[0].DocumentSummary.Status.Value;
            expenseClaim.Total = decimal.Parse(result[0].DocumentSummary.ClaimTotal.Value, System.Globalization.CultureInfo.InvariantCulture);
        }

        public async Task<IList<ExpenseClaimGroup>> GetMostRecentExpenseClaimsByStatus()
        {
            //TODO: Find a way to avoid having to fetch all claims, and then filtering out for the top 12 by group
            var list = await GetExpenseClaimsForStatus(String.Empty);

            var groups = new List<ExpenseClaimGroup>();
            foreach (var groupTitle in list.OrderBy(s => s.Status).Select(s => s.Status).Distinct())
            {
                var group = new ExpenseClaimGroup(groupTitle);
                foreach (var item in list.Where(i => i.Status == groupTitle).OrderByDescending(i => i.Date).ThenByDescending(i => i.RefNbr).Take(12))
                {
                    group.Items.Add(item);
                }
                groups.Add(group);
            }

            return groups;
        }

        public Task<IList<ExpenseClaimBase>> GetExpenseClaimsForStatus(string status)
        {
            Filter[] filter = null;

            if (!String.IsNullOrEmpty(status))
            {
                filter = new Filter[] { new Filter { Field = new Field { ObjectName = "ExpenseClaim", FieldName = "Status" }, Condition = FilterCondition.Equals, Value = status } };
            }

            return GetExpenseClaimsList(filter);
        }

        public Task<IList<ExpenseClaimBase>> SearchExpenseClaims(string searchText)
        {
            var filter = new Filter[] { 
                new Filter { Field = new Field { ObjectName="ExpenseClaim", FieldName="DocDesc"}, Condition = FilterCondition.Contain, Value = searchText, Operator = FilterOperator.Or },
                new Filter { Field = new Field { ObjectName="ExpenseClaim", FieldName="EmployeeID"}, Condition = FilterCondition.Contain, Value = searchText, Operator = FilterOperator.Or },
                new Filter { Field = new Field { ObjectName="ExpenseClaim", FieldName="RefNbr"}, Condition = FilterCondition.Equals, Value = searchText, Operator = FilterOperator.Or },
                new Filter { Field = new Field { ObjectName="ExpenseClaim", FieldName="NoteText"}, Condition = FilterCondition.Equals, Value = searchText, Operator = FilterOperator.Or },
            };

            return GetExpenseClaimsList(filter);
        }

        private async Task<IList<ExpenseClaimBase>> GetExpenseClaimsList(Filter[] filter)
        {
            ScreenSoapClient client = await GetAuthenticatedClient(Common.SettingsStore.SiteUrl, Common.SettingsStore.Username, Common.SettingsStore.Password);
            var schema = await GetSavedSchema();
            await client.SetSchemaAsync(schema);

            var result = await client.ExportAsync(new Command[] {
				    schema.DocumentSummary.Description,
					schema.DocumentSummary.ClaimedBy,
                    schema.DocumentSummary.ReferenceNbr,
                    schema.DocumentSummary.ClaimTotal,
                    schema.DocumentSummary.Date,
                    schema.DocumentSummary.Status
				},
                filter,
                0, false, true);

            var list = new List<ExpenseClaimBase>();
            for (int i = 0; i < result.ExportResult.Length; i++)
            {
                list.Add(new ExpenseClaimBase(
                    result.ExportResult[i][0],
                    result.ExportResult[i][1].Trim(),
                    result.ExportResult[i][2],
                    decimal.Parse(result.ExportResult[i][3], System.Globalization.CultureInfo.InvariantCulture),
                    DateTime.Parse(result.ExportResult[i][4], System.Globalization.CultureInfo.InvariantCulture),
                    result.ExportResult[i][5].Trim()));
            }
            return list;
        }

        public async Task<ExpenseClaim> GetExpenseClaim(string refNbr)
        {
            ScreenSoapClient client = await GetAuthenticatedClient(Common.SettingsStore.SiteUrl, Common.SettingsStore.Username, Common.SettingsStore.Password);
            var schema = await GetSavedSchema();
            await client.SetSchemaAsync(schema);

            var result = await client.ExportAsync(new Command[] {
                    new Value { Value = refNbr, LinkedCommand = schema.DocumentSummary.ReferenceNbr },
				    schema.DocumentSummary.Description,
					schema.DocumentSummary.ClaimedBy,
                    schema.DocumentSummary.ClaimTotal,
                    schema.DocumentSummary.Date,
                    schema.DocumentSummary.Status,
                    schema.DocumentSummary.ApprovalDate,
                    schema.DocumentSummary.Hold,
                    schema.DocumentSummary.Location,
                    schema.DocumentSummary.NoteText,
                    new Field() { FieldName = "ClaimDetailID", ObjectName = "ExpenseClaimDetails" },
                    schema.ExpenseClaimDetails.Date,
                    schema.ExpenseClaimDetails.RefNbr,
                    schema.ExpenseClaimDetails.ExpenseItem,
                    schema.ExpenseClaimDetails.Quantity,
                    schema.ExpenseClaimDetails.UnitCost,
                    schema.ExpenseClaimDetails.TotalAmount,
                    schema.ExpenseClaimDetails.EmployeePart,
                    schema.ExpenseClaimDetails.Billable,
                    schema.ExpenseClaimDetails.Customer,
                    schema.ExpenseClaimDetails.Location,
                    schema.ExpenseClaimDetails.ProjectContract,
                    schema.ExpenseClaimDetails.ProjectTask,
                    schema.ExpenseClaimDetails.Description,
                    schema.ExpenseClaimDetails.NoteText,
				},
                null,
                0, false, true);

            if (result.ExportResult.Length == 0) return null;

            var claim = new ExpenseClaim(result.ExportResult[0][0].Trim(),
                result.ExportResult[0][1].Trim(),
                refNbr,
                decimal.Parse(result.ExportResult[0][2].Trim(), System.Globalization.CultureInfo.InvariantCulture),
                DateTime.Parse(result.ExportResult[0][3].Trim(), System.Globalization.CultureInfo.InvariantCulture),
                result.ExportResult[0][4].Trim());

            if (!String.IsNullOrEmpty(result.ExportResult[0][5]))
            {
                claim.ApprovalDate = DateTime.Parse(result.ExportResult[0][5], System.Globalization.CultureInfo.InvariantCulture);
            }

            claim.Hold = bool.Parse(result.ExportResult[0][6]);
            claim.Location = result.ExportResult[0][7];
            claim.NoteText = result.ExportResult[0][8];

            for (int i = 0; i < result.ExportResult.Length; i++)
            {
                var lineNbr = result.ExportResult[i][9];
                if (String.IsNullOrEmpty(lineNbr)) continue;

                var line = new ExpenseClaimLine();
                line.ParentRefNbr = refNbr;
                line.LineNbr = int.Parse(lineNbr, System.Globalization.CultureInfo.InvariantCulture);
                line.Date = DateTime.Parse(result.ExportResult[i][10], System.Globalization.CultureInfo.InvariantCulture);
                line.RefNbr = result.ExportResult[i][11];
                line.ExpenseId = result.ExportResult[i][12].Trim();
                line.Quantity = decimal.Parse(result.ExportResult[i][13], System.Globalization.CultureInfo.InvariantCulture);
                line.UnitCost = decimal.Parse(result.ExportResult[i][14], System.Globalization.CultureInfo.InvariantCulture);
                line.Total = decimal.Parse(result.ExportResult[i][15], System.Globalization.CultureInfo.InvariantCulture);
                line.EmployeePart = decimal.Parse(result.ExportResult[i][16], System.Globalization.CultureInfo.InvariantCulture);
                line.Billable = bool.Parse(result.ExportResult[i][17]);
                line.Customer = result.ExportResult[i][18].Trim();
                line.Location = result.ExportResult[i][19].Trim();
                line.Project = result.ExportResult[i][20].Trim();
                line.ProjectTask = result.ExportResult[i][21].Trim();
                line.Description = result.ExportResult[i][22];
                line.NoteText = result.ExportResult[i][23];
                claim.Lines.Add(line);
            }

            var claimStatus = await client.SubmitAsync(new Command[] 
            {
                new Value { Value = refNbr, LinkedCommand = schema.DocumentSummary.ReferenceNbr },
                schema.DocumentSummary.Description
            });

            if (claimStatus.Length > 0)
            {
                // The schema needs to be generated with schema mode == detailed for this to work
                claim.AllowEdit = !claimStatus[0].DocumentSummary.Description.Descriptor.IsDisabled;
            }

            foreach (var line in claim.Lines)
            {
                line.AllowEdit = claim.AllowEdit;
                line.HasUnsavedChanges = false;
            }

            claim.HasUnsavedChanges = false;
            return claim;
        }

        private async Task<ScreenSoapClient> GetAuthenticatedClient(string siteUrl, string username, string password)
        {
            var client = new ScreenSoapClient(ServiceHelpers.GetDefaultBinding(siteUrl), ServiceHelpers.GetServiceAddress(siteUrl, "EP301000"));
            await client.LoginAsync(username, password);
            return client;
        }

        public async Task Login(string siteUrl, string username, string password)
        {
            // Only used by the logging-in screen
            var result = await GetAuthenticatedClient(siteUrl, username, password);
        }

        public void Logout()
        {
            var settings = Acumatica.Core.Ioc.Container.Default.GetInstance<ISettingsService>();
            settings.SetValue("Password", null);
        }

        public string CurrentLoginName
        {
            get
            {
                return Common.SettingsStore.Username;
            }
        }
    }
}