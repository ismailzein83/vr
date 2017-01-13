using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BEBridge.Entities;
using Vanrise.Invoice.Business;
using Vanrise.Invoice.Entities;

namespace Vanrise.Invoice.MainExtensions.InvoiceReader
{
    public class InvoiceSourceReader : SourceBEReader
    {
        public InvoiceSourceReaderSetting Setting { get; set; }
        public override void RetrieveUpdatedBEs(ISourceBEReaderRetrieveUpdatedBEsContext context)
        {
            InvoiceSourceReaderState state = context.ReaderState as InvoiceSourceReaderState;
            if (state == null)
                state = new InvoiceSourceReaderState();

            InvoiceManager invoiceManager = new InvoiceManager();
            List<Entities.Invoice> invoices = new List<Entities.Invoice>();
            invoiceManager.LoadInvoicesAfterImportedId(Setting.InvoiceTypeId, state.LastImportedInvoiceId, (invoice) =>
            {
                invoices.Add(invoice);
                state.LastImportedInvoiceId++;
                if (invoices.Count > Setting.BatchSize)
                {
                    InvoiceSourceBatch batch = new InvoiceSourceBatch
                    {
                        Invoices = invoices
                    };
                    context.OnSourceBEBatchRetrieved(batch, null);
                    invoices = new List<Entities.Invoice>();
                }
            });

            if (invoices.Count > Setting.BatchSize)
            {
                InvoiceSourceBatch batch = new InvoiceSourceBatch
                {
                    Invoices = invoices
                };
                context.OnSourceBEBatchRetrieved(batch, null);
            }
            context.ReaderState = state;
        }
    }

    public class InvoiceSourceReaderSetting
    {
        public Guid InvoiceTypeId { get; set; }
        public int BatchSize { get; set; }
    }

    public class InvoiceSourceReaderState
    {
        public long LastImportedInvoiceId { get; set; }
    }
}
