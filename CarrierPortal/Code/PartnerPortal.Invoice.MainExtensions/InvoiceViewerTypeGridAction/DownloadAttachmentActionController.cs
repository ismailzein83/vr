using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Vanrise.Web.Base;
using System.IO;
using PartnerPortal.Invoice.Business;

namespace PartnerPortal.Invoice.MainExtensions
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "DownloadAttachmentAction")]
    [JSONWithTypeAttribute]
    public class DownloadAttachmentActionController : BaseAPIController
    {
        [HttpGet]
        [Route("DownloadAttachment")]
        public object DownloadAttachment(Guid invoiceViewerTypeId, Guid invoiceViewerTypeGridActionId, long invoiceId)
        {
            DownloadAttachmentActionManager manager = new DownloadAttachmentActionManager();
            var invoiceViewerType = new InvoiceViewerTypeManager().GetInvoiceViewerType(invoiceViewerTypeId);
            var invoice = new InvoiceManager().GetRemoteInvoice(invoiceViewerType.Settings.VRConnectionId, invoiceId);
            string fileName = string.Format("Invoice_{0}.pdf",invoice.SerialNumber);
            return GetExcelResponse(manager.DownloadAttachment(invoiceViewerTypeId, invoiceViewerTypeGridActionId, invoiceId), fileName);
        }
    }
}
