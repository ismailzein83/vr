using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Business;
using Vanrise.Invoice.Entities;

namespace PartnerPortal.Invoice.Business
{
    public class InvoiceTypeManager
    {
        public IEnumerable<InvoiceTypeInfo> GetRemoteInvoiceTypeInfo(Guid connectionId, string serializedFilter)
        {
            VRInterAppRestConnection connectionSettings = GetVRInterAppRestConnection(connectionId);
            return connectionSettings.Get<IEnumerable<InvoiceTypeInfo>>(string.Format("/api/VR_Invoice/InvoiceType/GetInvoiceTypesInfo?serializedFilter={0}", serializedFilter));
        }
        public IEnumerable<InvoiceFieldInfo> GetRemoteInvoiceFieldsInfo(Guid connectionId, string serializedFilter)
        {
            VRInterAppRestConnection connectionSettings = GetVRInterAppRestConnection(connectionId);
            return connectionSettings.Get<IEnumerable<InvoiceFieldInfo>>(string.Format("/api/VR_Invoice/InvoiceType/GetRemoteInvoiceFieldsInfo"));
        }
        public IEnumerable<string> GetRemoteInvoiceTypeCustomFieldsInfo(Guid connectionId, Guid invoiceTypeId)
        {
            VRInterAppRestConnection connectionSettings = GetVRInterAppRestConnection(connectionId);
            return connectionSettings.Get<IEnumerable<string>>(string.Format("/api/VR_Invoice/InvoiceType/GetRemoteInvoiceTypeCustomFieldsInfo?invoiceTypeId={0}", invoiceTypeId));
        }
        public IEnumerable<InvoiceUIGridColumnRunTime> GetInvoiceTypeGridColumns(Guid connectionId, Guid invoiceTypeId)
        {
            VRInterAppRestConnection connectionSettings = GetVRInterAppRestConnection(connectionId);
            return connectionSettings.Get<IEnumerable<InvoiceUIGridColumnRunTime>>(string.Format("/api/VR_Invoice/InvoiceType/GetInvoiceTypeGridColumns?invoiceTypeId={0}", invoiceTypeId));
        }
        public IEnumerable<InvoiceAttachmentInfo> GetRemoteInvoiceAttachmentsInfo(Guid connectionId, Guid invoiceTypeId)
        {
            VRInterAppRestConnection connectionSettings = GetVRInterAppRestConnection(connectionId);
            return connectionSettings.Get<IEnumerable<InvoiceAttachmentInfo>>(string.Format("/api/VR_Invoice/InvoiceType/GetRemoteInvoiceTypeAttachmentsInfo?invoiceTypeId={0}", invoiceTypeId));
        }
        private VRInterAppRestConnection GetVRInterAppRestConnection(Guid connectionId)
        {
            VRConnectionManager connectionManager = new VRConnectionManager();
            var vrConnection = connectionManager.GetVRConnection<VRInterAppRestConnection>(connectionId);
            return vrConnection.Settings as VRInterAppRestConnection;
        }
    }
}
