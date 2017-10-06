using System;

namespace Vanrise.Invoice.Entities
{
    public class InvoicePartner
    {
        public string PartnerId { get; set; }
        public string PartnerName { get; set; }
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
    }
}
