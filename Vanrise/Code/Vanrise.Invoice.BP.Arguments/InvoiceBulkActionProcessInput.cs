using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Invoice.Entities;

namespace Vanrise.Invoice.BP.Arguments
{
    public class InvoiceBulkActionProcessInput : Vanrise.BusinessProcess.Entities.BaseProcessInputArgument
    {
        public Guid InvoiceTypeId { get; set; }
        public Guid InvoiceBulkActionIdentifier { get; set; }
        public List<InvoiceBulkActionRuntime> InvoiceBulkActions { get; set; }
        public DateTime MinimumFrom { get; set; }
        public DateTime MaximumTo { get; set; }
        public override string GetTitle()
        {
            return "Invoice Bulk Action Process";
        }
    }
}
