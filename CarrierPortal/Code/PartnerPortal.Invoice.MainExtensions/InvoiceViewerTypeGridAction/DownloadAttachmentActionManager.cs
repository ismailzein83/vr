using PartnerPortal.Invoice.Business;
using PartnerPortal.Invoice.MainExtensions.InvoiceViewerTypeGridAction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Business;

namespace PartnerPortal.Invoice.MainExtensions
{
    public class DownloadAttachmentActionManager
    {
        public byte[] DownloadAttachment(Guid invoiceViewerTypeId, Guid invoiceViewerTypeGridActionId, long invoiceId)
        {
            var invoiceViewerType = new InvoiceViewerTypeManager().GetInvoiceViewerType(invoiceViewerTypeId);
            if(invoiceViewerType ==null)
                throw new NullReferenceException(string.Format("invoiceViewerType: {0}",invoiceViewerTypeId));
            if (invoiceViewerType.Settings == null)
                throw new NullReferenceException(string.Format("invoiceViewerType.Settings: {0}", invoiceViewerTypeId));
            if (invoiceViewerType.Settings.GridSettings == null)
                throw new NullReferenceException(string.Format("invoiceViewerType.Settings.GridSettings: {0}", invoiceViewerTypeId));
            if (invoiceViewerType.Settings.GridSettings.InvoiceGridActions == null)
                throw new NullReferenceException(string.Format("invoiceViewerType.Settings.GridSettings.InvoiceGridActions: {0}", invoiceViewerTypeId));

            var attachmentAction = invoiceViewerType.Settings.GridSettings.InvoiceGridActions.FirstOrDefault(x => x.InvoiceViewerTypeGridActionId == invoiceViewerTypeGridActionId);

            if (attachmentAction == null)
                throw new NullReferenceException(string.Format("attachmentAction: {0} ", invoiceViewerTypeGridActionId));
            
            var attachmentActionSettings = attachmentAction.Settings as DownloadAttachmentAction;

            if (attachmentActionSettings == null)
                throw new NullReferenceException(string.Format("attachmentActionSettings: of type ", typeof(DownloadAttachmentAction)));

            VRInterAppRestConnection connectionSettings = new InvoiceTypeManager().GetVRInterAppRestConnection(invoiceViewerType.Settings.VRConnectionId);
            return connectionSettings.Get<byte[]>(string.Format("/api/VR_Invoice/Invoice/DownloadAttachment?invoiceTypeId={0}&invoiceAttachmentId={1}&invoiceId={2}", invoiceViewerType.Settings.InvoiceTypeId,attachmentActionSettings.InvoiceAttachmentId, invoiceId));
        }
    }
}
