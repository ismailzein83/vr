using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Invoice.Entities
{
    public abstract class InvoiceFileConverter
    {
        public abstract Guid ConfigId { get; }
        public abstract InvoiceFile ConvertToInvoiceFile(IInvoiceRDLCFileConverterContext context);
    }
    public abstract class InvoiceFile
    {
        public string Name { get; set; }
        public byte[] Content { get; set; }
        public abstract void ConvertToAttachment();
    }
    public class PDFInvoiceFile : InvoiceFile
    {
        public override void ConvertToAttachment()
        {
            throw new NotImplementedException();
        }
    }
}
