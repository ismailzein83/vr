using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Invoice.Entities
{
    public class InvoiceBulkActionRuntime
    {
        public Guid InvoiceBulkActionId { get; set; }
        public AutomaticInvoiceActionRuntimeSettings Settings { get; set; }
    }
}
