using PartnerPortal.Invoice.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Invoice.Entities;
using Vanrise.Web.Base;

namespace PartnerPortal.Invoice.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "InvoiceType")]
    [JSONWithTypeAttribute]
    public class CPInvoiceTypeController : BaseAPIController
    {
        InvoiceTypeManager _manager = new InvoiceTypeManager();
        [HttpGet]
        [Route("GetRemoteInvoiceTypeInfo")]
        public IEnumerable<InvoiceTypeInfo> GetRemoteInvoiceTypeInfo(Guid connectionId, string serializedFilter = null)
        {
            return _manager.GetRemoteInvoiceTypeInfo(connectionId, serializedFilter);
        }
        [HttpGet]
        [Route("GetRemoteInvoiceFieldsInfo")]
        public IEnumerable<InvoiceFieldInfo> GetRemoteInvoiceFieldsInfo(Guid connectionId, string serializedFilter = null)
        {
            return _manager.GetRemoteInvoiceFieldsInfo(connectionId, serializedFilter);
        }
        [HttpGet]
        [Route("GetRemoteInvoiceTypeCustomFieldsInfo")]
        public IEnumerable<string> GetRemoteInvoiceTypeCustomFieldsInfo(Guid connectionId, Guid invoiceTypeId)
        {
            return _manager.GetRemoteInvoiceTypeCustomFieldsInfo(connectionId, invoiceTypeId);
        }
        [HttpGet]
        [Route("GetRemoteInvoiceAttachmentsInfo")]
        public IEnumerable<InvoiceAttachmentInfo> GetRemoteInvoiceAttachmentsInfo(Guid connectionId, Guid invoiceTypeId)
        {
            return _manager.GetRemoteInvoiceAttachmentsInfo(connectionId, invoiceTypeId);
        }
        
    }
}