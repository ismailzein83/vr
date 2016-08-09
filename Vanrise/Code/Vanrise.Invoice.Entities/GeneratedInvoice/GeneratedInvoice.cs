using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Invoice.Entities
{
    public class GeneratedInvoice
    {
        public dynamic InvoiceDetails { get; set; }

        public List<GeneratedInvoiceItemSet> InvoiceItemSets { get; set; }
    }
}
