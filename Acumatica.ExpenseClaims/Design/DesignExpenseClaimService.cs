using System;
using Acumatica.ExpenseClaims.Model;
using System.Threading.Tasks;
using System.Collections.Generic;
using Acumatica.ExpenseClaims.Service;

namespace Acumatica.ExpenseClaims.Design
{
    public class DesignExpenseClaimService : IExpenseClaimService
    {
        public Task Login(string siteUrl, string username, string password)
        {
            return null;
        }

        public Task SaveExpenseClaim(ExpenseClaim expenseClaim)
        {
            return null;
        }

        public Task<IList<ExpenseClaimGroup>> GetMostRecentExpenseClaimsByStatus()
        {
            return Task.Factory.StartNew<IList<ExpenseClaimGroup>>(() =>
            {
                var list = new List<ExpenseClaimGroup>();

                var group1 = new ExpenseClaimGroup("Open");
                group1.Items.Add(new ExpenseClaimBase("A", "John Smith", "00002", 10, DateTime.Today, "Open"));
                group1.Items.Add(new ExpenseClaimBase("B", "John Smith", "00003", 20, DateTime.Today.AddDays(-1), "Open"));
                list.Add(group1);

                var group2 = new ExpenseClaimGroup("Approved");
                group2.Items.Add(new ExpenseClaimBase("C", "John Smith", "00001", 45322.67M, DateTime.Today.AddMonths(-1), "Approved"));
                list.Add(group2);

                return list;
            });
        }

        public Task<IList<ExpenseClaimBase>> GetExpenseClaimsForStatus(string status)
        {
            return Task.Factory.StartNew<IList<ExpenseClaimBase>>(() =>
            {
                var list = new List<ExpenseClaimBase>();
                list.Add(new ExpenseClaimBase("A", "John Smith", "00002", 10, DateTime.Today, status));
                list.Add(new ExpenseClaimBase("B", "John Smith", "00003", 20, DateTime.Today.AddDays(-1), status));
                return list;
            });
        }

        public Task<IList<ExpenseClaimBase>> SearchExpenseClaims(string searchText)
        {
            return GetExpenseClaimsForStatus("");
        }

        public Task<ExpenseClaim> GetExpenseClaim(string refNbr)
        {
            return Task.Factory.StartNew <ExpenseClaim>(() => 
            {
                var claim = new ExpenseClaim("Sample Expense Claim", "MICG", refNbr, 334.56M, DateTime.Today, "Hold");
                claim.Hold = true;
                claim.Location = "MAIN";
                claim.NoteText = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Mauris lectus est, pellentesque quis luctus sit amet, hendrerit sed nunc";
                claim.AllowEdit = true;

                var line = new ExpenseClaimLine();
                line.ParentRefNbr = refNbr;
                line.AllowEdit = true;
                line.Date = DateTime.Today.AddDays(-5);
                line.RefNbr = "1234";
                line.ExpenseId = "DEP";
                line.Quantity = 3;
                line.UnitCost = 100;
                line.Total = 300;
                line.EmployeePart = 10;
                line.Description = "Line 1 Description";
                claim.Lines.Add(line);

                var line2 = new ExpenseClaimLine();
                line2.ParentRefNbr = refNbr;
                line2.AllowEdit = true;
                line2.Date = DateTime.Today.AddDays(-3);
                line2.RefNbr = "4567";
                line2.ExpenseId = "FUEL";
                line2.Total = 34.56M;
                line2.Description = "Line 2 Description";
                claim.Lines.Add(line2);

                return claim;
            });
        }

        public void Logout()
        {
        }

        public string CurrentLoginName
        {
            get
            {
                return "admin@Demo";
            }
        }
    }
}