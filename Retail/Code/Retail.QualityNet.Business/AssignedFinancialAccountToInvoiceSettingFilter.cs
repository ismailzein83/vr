using Retail.BusinessEntity.Business;
using Retail.BusinessEntity.Entities;
using System.Linq;
using Vanrise.Invoice.Business.InvoicePartnerFilter;

namespace Retail.QualityNet.Business
{
    public class AssignedFinancialAccountToInvoiceSettingFilter : PartnerInvoiceSettingFilter, IAccountFilter, IFinancialAccountFilter
    {
        public bool IsExcluded(IAccountFilterContext context)
        {
            var financialAccounts = new FinancialAccountManager().GetFinancialAccounts(context.AccountBEDefinitionId, context.Account.AccountId, false);
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
