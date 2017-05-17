using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Invoice.Entities
{
    public enum InvoiceAccountStatus { Active = 0 }
    public class InvoiceAccount
    {
        public long InvoiceAccountId { get; set; }
        public Guid InvoiceTypeId { get; set; }
        public string PartnerId { get; set; }
        public DateTime? BED { get; set; }
        public DateTime? EED { get; set; }
        public InvoiceAccountStatus Status { get; set; }
        public bool IsDeleted { get; set; }

    }
}
