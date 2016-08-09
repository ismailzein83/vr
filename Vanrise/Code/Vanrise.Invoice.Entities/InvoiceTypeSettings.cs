using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;

namespace Vanrise.Invoice.Entities
{
    public abstract class InvoiceTypeSettings
    {
        public virtual InvoiceTypeUISettings UISettings { get; set; }

        public virtual List<DataRecordField> InvoiceFields { get; set; }

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
