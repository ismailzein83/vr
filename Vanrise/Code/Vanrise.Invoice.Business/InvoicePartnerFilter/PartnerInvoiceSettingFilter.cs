using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Invoice.Entities;

namespace Vanrise.Invoice.Business.InvoicePartnerFilter
{
    public class PartnerInvoiceSettingFilter
    {
        public Guid InvoiceTypeId { get; set; }
        public string EditablePartnerId { get; set; }
        public bool? OnlyAssignedToInvoiceSetting { get; set; }
        public Guid? InvoiceSettingId { get; set; }
        public bool IsMatched(string partnerId)
        {

            string errorMessage;
            if (EditablePartnerId != null && EditablePartnerId == partnerId)
                return true;
            var partnerInvoiceSettingManager = new PartnerInvoiceSettingManager();
            if (OnlyAssignedToInvoiceSetting.HasValue && OnlyAssignedToInvoiceSetting.Value && InvoiceSettingId.HasValue)
            {
                if (!partnerInvoiceSettingManager.IsPartnerAssignedToInvoiceSetting(partnerId, InvoiceSettingId.Value))
                    return false;
                return true;
            }
            if (!partnerInvoiceSettingManager.ValidatePartnerInvoiceSetting(partnerId, InvoiceTypeId, out errorMessage))
                return false;
            return true;
        }
    }
}
