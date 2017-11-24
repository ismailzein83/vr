using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Invoice.Entities;

namespace Vanrise.Invoice.Business.InvoicePartnerFilter
{
    public class NotAssignedPartnerToInvoiceSettingFilter
    {
        public Guid InvoiceTypeId { get; set; }
        public string EditablePartnerId { get; set; }
        public bool IsMatched(string partnerId)
        {

            string errorMessage;
            if (EditablePartnerId != null && EditablePartnerId == partnerId)
                return true;

            if (!new PartnerInvoiceSettingManager().ValidatePartnerInvoiceSetting(partnerId, InvoiceTypeId, out errorMessage))
                return false;
            return true;
        }
    }
}
