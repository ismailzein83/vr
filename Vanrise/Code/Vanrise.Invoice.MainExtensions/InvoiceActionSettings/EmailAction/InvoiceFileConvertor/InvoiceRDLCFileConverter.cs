using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Invoice.Business;
using Vanrise.Invoice.Entities;

namespace Vanrise.Invoice.MainExtensions
{
    public enum InvoiceAttachmentType { PDF = 0, Excel = 1 }
    public class InvoiceRDLCFileConverter : InvoiceFileConverter
    {
        public override Guid ConfigId { get { return new Guid("CE1699D9-2696-4931-B332-D519AECF526E"); } }
        public Guid InvoiceActionId { get; set; }
        public InvoiceAttachmentType AttachmentType { get; set; }
        public override InvoiceFile ConvertToInvoiceFile(IInvoiceRDLCFileConverterContext context)
        {
            InvoiceFile invoiceFile = null;
            OpenRDLCReportActionManager openRDLCReportActionManager = new OpenRDLCReportActionManager();
            ReportViewer reportViewer = new ReportViewer();
            openRDLCReportActionManager.BuildRdlcReport(reportViewer, new ReportInput
            {
                ActionId = this.InvoiceActionId,
                Context = new PhysicalInvoiceActionContext { InvoiceId = context.InvoiceId }
            });
            Warning[] warnings;
            string[] streamids;
            string mimeType;
            string encoding;
            string filenameExtension;
            var invoice = new InvoiceManager().GetInvoice(context.InvoiceId);
            switch (this.AttachmentType)
            {
                case InvoiceAttachmentType.PDF:
                    invoiceFile = new PDFInvoiceFile();
                    invoiceFile.Content = reportViewer.LocalReport.Render("PDF", null, out mimeType, out encoding, out filenameExtension, out streamids, out warnings);
                    invoiceFile.Name = new PartnerManager().EvaluateInvoiceFileNamePattern(invoice.InvoiceTypeId, invoice.PartnerId, invoice);

                    break;
                case InvoiceAttachmentType.Excel:
                    invoiceFile = new ExcelInvoiceFile();
                    invoiceFile.Name = new PartnerManager().EvaluateInvoiceFileNamePattern(invoice.InvoiceTypeId, invoice.PartnerId, invoice);
                    invoiceFile.Content = reportViewer.LocalReport.Render("Excel", null, out mimeType, out encoding, out filenameExtension, out streamids, out warnings);
                    break;
            }
            return invoiceFile;
        }
    }
}
