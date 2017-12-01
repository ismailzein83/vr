using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Invoice.Business.InvoicePartnerFilter;

namespace TOne.WhS.Invoice.Business
{
    public class AssignedPartnerInvoiceSettingFilter : PartnerInvoiceSettingFilter, IWHSFinancialAccountFilter
    {
        public bool IsMatched(IWHSFinancialAccountFilterContext context)
        {
            if (!base.IsMatched(context.FinancialAccountId.ToString()))
                return false;
            return true;
        }
    }
}
