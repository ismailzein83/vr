using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Invoice.Entities;

namespace PartnerPortal.Invoice.Entities
{
    public class InvoiceAppQuery
    {
        public DateTime FromTime { get; set; }
        public DateTime? ToTime { get; set; }
        public Guid InvoiceViewerTypeId { get; set; }
        public List<string> PartnerIds { get; set; }
    }
}
