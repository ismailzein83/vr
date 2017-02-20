using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Invoice.Entities
{
    public class PartnerInvoiceSetting
    {
        public Guid PartnerInvoiceSettingId { get; set; }
        public string PartnerId { get; set; }
        public Guid InvoiceSettingID { get; set; }
        public PartnerInvoiceSettingDetails Details { get; set; }
    }
    public class PartnerInvoiceSettingDetails
    {
        public Dictionary<Guid, InvoiceSettingPart> InvoiceSettingParts { get; set; }
    }
}
