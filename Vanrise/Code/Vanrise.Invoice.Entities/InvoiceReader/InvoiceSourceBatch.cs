using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BEBridge.Entities;

namespace Vanrise.Invoice.Entities
{
    public class InvoiceSourceBatch : SourceBEBatch
    {
        public override string BatchName
        {
            get { return "Invoices Batch"; }
        }

        public IEnumerable<Invoice> Invoices { get; set; }
    }
}
