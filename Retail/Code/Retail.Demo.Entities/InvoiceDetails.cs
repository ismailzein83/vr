using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.Demo.Entities
{
    public class InvoiceDetails
    {
        public Decimal Amount { get; set; }
        public int Currency { get; set; }
        public InvoiceDetails()
        {

        }
        public IEnumerable<InvoiceDetails> GetInvoiceDetailsRDLCSchema()
        {
            return null;
        }
       
    }
}
