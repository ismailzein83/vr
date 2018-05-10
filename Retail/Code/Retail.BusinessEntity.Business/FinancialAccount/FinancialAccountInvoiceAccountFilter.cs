using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Business
{
    public class FinancialAccountInvoiceAccountFilter : IAccountFilter
    {
        public Guid InvoiceTypeId { get; set; }

        static FinancialAccountManager s_financialAccountManager = new FinancialAccountManager();
        static FinancialAccountDefinitionManager s_financialAccountDefinitionManager = new FinancialAccountDefinitionManager();

        public bool IsExcluded(IAccountFilterContext context)
        {
            var financialAccounts = s_financialAccountManager.GetFinancialAccounts(context.AccountBEDefinitionId, context.Account.AccountId, false);
            if (financialAccounts != null)
            {
                foreach (var financialAccount in financialAccounts)
                {
                    var financialAccountDefinitionSettings = s_financialAccountDefinitionManager.GetFinancialAccountDefinitionSettings(financialAccount.FinancialAccount.FinancialAccountDefinitionId);
                    if (financialAccountDefinitionSettings.InvoiceTypes != null && financialAccountDefinitionSettings.InvoiceTypes.Any(x => x.InvoiceTypeId == this.InvoiceTypeId))
                        return false;//dont exclude
                }
            }
            return true;
        }
    }
}
