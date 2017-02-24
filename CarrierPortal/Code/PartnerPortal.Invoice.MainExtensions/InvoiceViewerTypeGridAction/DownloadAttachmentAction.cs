using PartnerPortal.Invoice.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartnerPortal.Invoice.MainExtensions.InvoiceViewerTypeGridAction
{
    public class DownloadAttachmentAction : InvoiceViewerTypeGridActionSettings
    {
        public override string ActionTypeName { get { return "DownloadAttachment"; } }
        public override Guid ConfigId
        {
            get { return new Guid("609A37C4-466A-40A9-8140-F23F5D80D3C3"); }
        }
        public Guid InvoiceAttachmentId { get; set; }
    }
}
