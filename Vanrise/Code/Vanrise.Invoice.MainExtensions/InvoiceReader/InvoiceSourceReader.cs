using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BEBridge.Entities;

namespace Vanrise.Invoice.MainExtensions.InvoiceReader
{
    public class InvoiceSourceReader : SourceBEReader
    {
        public InvoiceSourceReaderSetting Setting { get; set; }
        public override void RetrieveUpdatedBEs(ISourceBEReaderRetrieveUpdatedBEsContext context)
        {
            throw new NotImplementedException();
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
