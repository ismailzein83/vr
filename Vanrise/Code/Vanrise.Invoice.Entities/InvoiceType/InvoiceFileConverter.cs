using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

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
        public abstract VRMailAttachement ConvertToAttachment();
    }
    public class PDFInvoiceFile : InvoiceFile
    {
        public override VRMailAttachement ConvertToAttachment()
        {
            return new VRMailAttachmentPDF
            {
                Content = this.Content,
                Name = string.Format("{0}.pdf", this.Name != null?this.Name:"Invoice"),
            };
        }
    }
    public class ExcelInvoiceFile : InvoiceFile
    {
        public override VRMailAttachement ConvertToAttachment()
        {
            return new VRMailAttachmentExcel
            {
                Content = this.Content,
                Name = string.Format("{0}.xls", this.Name != null ? this.Name : "Invoice"),
            };
        }
    }
}
