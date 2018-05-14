using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Invoice.Business.InvoicePartnerFilter;

namespace Retail.Interconnect.Business
{
    public class AssignedFinancialAccountToInvoiceSettingFilter : PartnerInvoiceSettingFilter, IAccountFilter, IFinancialAccountFilter
    {
        public bool IsExcluded(IAccountFilterContext context)
        {
            throw new NotImplementedException();
        }

        public bool IsMatched(IFinancialAccountFilterContext context)
        {
            throw new NotImplementedException();
        }
    }
}
