using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Invoice.Entities
{
    public abstract class InvoiceTypeSettings
    {
        public abstract void CreateInvoice(IInvoiceCreationContext context);
    }

    public interface IInvoiceCreationContext
    {
        string PartnerId { get; }

        DateTime FromDate { get; }

        DateTime ToDate { get; }

        Invoice Invoice { set; }

        List<InvoiceItem> InvoiceItems { set; }
    }
}
