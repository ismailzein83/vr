using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Invoice.Entities
{
    public class CarrierInvoiceAccountData
    {
        public int InvoiceAccountId { get; set; }

        public Guid InvoiceTypeId { get; set; }

        public DateTime BED { get; set; }

        public DateTime? EED { get; set; }
    }
}
