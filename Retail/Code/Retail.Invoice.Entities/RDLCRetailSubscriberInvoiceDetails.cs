using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.Invoice.Entities
{
    public class RDLCRetailSubscriberInvoiceDetails
    {
        public Decimal TotalAmount { get; set; }

        public decimal TotalDuration { get; set; }

        public int CountCDRs { get; set; }

        public RDLCRetailSubscriberInvoiceDetails()
        {

        }

        public IEnumerable<RDLCRetailSubscriberInvoiceDetails> GetRDLCSubscriberInvoiceDetailsSchema()
        {
            return null;
        }
    }
}
