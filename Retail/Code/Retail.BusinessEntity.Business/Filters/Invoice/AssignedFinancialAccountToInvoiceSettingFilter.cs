using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Invoice.Business.InvoicePartnerFilter;

namespace Retail.BusinessEntity.Business
{
    public class AssignedFinancialAccountToInvoiceSettingFilter : PartnerInvoiceSettingFilter, IAccountFilter, IFinancialAccountFilter
    {
        public bool IsExcluded(IAccountFilterContext context)
        {
            FinancialAccountManager manager = new FinancialAccountManager();
            var financialAccounts = manager.GetFinancialAccounts(context.AccountBEDefinitionId, context.Account.AccountId, false);
            if (financialAccounts != null && financialAccounts.All(x => !base.IsMatched(x.FinancialAccountId)))
                return true;
            return false;
        }

        public bool IsMatched(IFinancialAccountFilterContext context)
        {
            if (this.EditablePartnerId != null && context.FinancialAccountId == this.EditablePartnerId)
                return true;
            if (!base.IsMatched(context.FinancialAccountId))
                return false;
            return true;
        }
    }
}
