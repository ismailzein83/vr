using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Invoice.Business.InvoicePartnerFilter;

namespace Retail.Zajil.MainExtensions
{
    public class AssignedFinancialAccountPartnerInvoiceSettingFilter : PartnerInvoiceSettingFilter, IAccountFilter
    {
        public bool IsExcluded(IAccountFilterContext context)
        {
            if (!base.IsMatched(context.Account.AccountId.ToString()))
                return true;
            return false;
        }
    }
}
