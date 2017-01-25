using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Invoice.Entities;

namespace Vanrise.Invoice.Business.InvoicePartnerFilter
{
    public class NotAssignedPartnerToInvoiceSettingFilter : IInvoicePartnerFilter
    {
        public string EditablePartnerId { get; set; }
        public bool IsMatched(IInvoicePartnerFilterContext context)
        {
            if ((EditablePartnerId != null && context.PartnerId == EditablePartnerId) || !new PartnerInvoiceSettingManager().CheckIfPartnerAssignedToInvoiceSetting(context.PartnerId))
                return true;
            return false;
        }
    }
}
