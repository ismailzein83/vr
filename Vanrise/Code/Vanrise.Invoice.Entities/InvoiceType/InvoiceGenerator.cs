using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Invoice.Entities
{
    public abstract class InvoiceGenerator
    {
        public Guid ConfigId { get; set; }

        public abstract void GenerateInvoice(IInvoiceGenerationContext context);
    }

    public interface IInvoiceGenerationContext
    {
        string PartnerId { get; }

        DateTime FromDate { get; }

        DateTime ToDate { get; }

        dynamic CustomSectionPayload { get; }

        GeneratedInvoice Invoice { set; }
    }
}
