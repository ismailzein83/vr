using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Invoice.Entities
{
    public class GenerateInvoiceInput
    {
        public long? InvoiceId { get; set; }
        public Guid InvoiceActionId { get; set; }
        public Guid InvoiceTypeId { get; set; }
        public string PartnerId { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public DateTime IssueDate { get; set; }
        public dynamic CustomSectionPayload { get; set; }
        public Boolean IsAutomatic { get; set; }
    }
}
 