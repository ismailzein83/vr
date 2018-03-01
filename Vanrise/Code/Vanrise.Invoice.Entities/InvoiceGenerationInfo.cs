using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Invoice.Entities
{
    public class InvoiceGenerationInfo
    {
        public string PartnerId { get; set; }
        public string PartnerName { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public dynamic CustomPayload { get; set; }
    }
}
