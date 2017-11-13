using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Invoice.Entities
{
    public class InvoiceBulkActionDefinitionEntity
    {
        public InvoiceMenualBulkAction InvoiceMenualBulkAction { get; set; }
        public InvoiceBulkAction InvoiceBulkAction { get; set; }

        public List<InvoiceAttachment> InvoiceAttachments { get; set; }

    }
}
