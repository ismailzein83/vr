using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Invoice.Entities
{
    public enum VRInvoiceAccountStatus { Active = 0 }
    public class VRInvoiceAccount
    {
        public long InvoiceAccountId { get; set; }
        public Guid InvoiceTypeId { get; set; }
        public string PartnerId { get; set; }
        public DateTime? BED { get; set; }
        public DateTime? EED { get; set; }
        public VRInvoiceAccountStatus Status { get; set; }
        public bool IsDeleted { get; set; }

    }
}
