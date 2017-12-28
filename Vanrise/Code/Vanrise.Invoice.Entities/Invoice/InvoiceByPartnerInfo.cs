using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Invoice.Entities
{
    public class InvoiceByPartnerInfo
    {
        public Guid InvoiceTypeId { get; set; }
        public string PartnerId { get; set; }
        public DateTime ToDate { get; set; }
        public DateTime IssueDate { get; set; }
        public DateTime DueDate { get; set; }
    }
}
