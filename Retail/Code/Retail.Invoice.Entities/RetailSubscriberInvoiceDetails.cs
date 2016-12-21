using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.Invoice.Entities
{
    public class RetailSubscriberInvoiceDetails
    {
        public Decimal TotalAmount { get; set; }

        public decimal TotalDuration { get; set; }

        public int CountCDRs { get; set; }

        public int CurrencyId { get; set; }

    }
}
