using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Invoice.Entities
{
    public class InvoiceQuery
    {
        public Guid InvoiceTypeId { get; set; }
        public List<string> PartnerIds { get; set; }
        public DateTime FromTime { get; set; }
        public DateTime? ToTime { get; set; }
        public DateTime? IssueDate { get; set; }
    }
}
