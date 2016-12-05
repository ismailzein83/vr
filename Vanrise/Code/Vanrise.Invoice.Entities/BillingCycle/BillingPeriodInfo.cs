using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Invoice.Entities
{
    public class BillingPeriodInfo
    {
        public string PartnerId { get; set; }
        public Guid InvoiceTypeId { get; set; }
        public DateTime NextPeriodStart { get; set; }
    }
}
