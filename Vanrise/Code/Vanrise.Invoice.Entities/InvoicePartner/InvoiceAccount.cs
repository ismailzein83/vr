using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.Invoice.Entities
{
    public class VRInvoiceAccount
    {
        public long InvoiceAccountId { get; set; }
        public Guid InvoiceTypeId { get; set; }
        public string PartnerId { get; set; }
        public DateTime? BED { get; set; }
        public DateTime? EED { get; set; }
        public VRAccountStatus Status { get; set; }
        public bool IsDeleted { get; set; }

    }
}
