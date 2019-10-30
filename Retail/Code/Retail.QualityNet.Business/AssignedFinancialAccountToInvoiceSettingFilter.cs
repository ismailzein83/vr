using Retail.BusinessEntity.Entities;
using Vanrise.Invoice.Business.InvoicePartnerFilter;

namespace Retail.QualityNet.Business
{
    public class AssignedFinancialAccountToInvoiceSettingFilter : PartnerInvoiceSettingFilter, IAccountFilter
    {
        public bool IsExcluded(IAccountFilterContext context)
        {
            if (!base.IsMatched(context.Account.AccountId.ToString()))
                return true;
            return false;
        }
    }
}