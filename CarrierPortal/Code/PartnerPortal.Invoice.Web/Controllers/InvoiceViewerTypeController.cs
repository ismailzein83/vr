using PartnerPortal.Invoice.Business;
using PartnerPortal.Invoice.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Web.Base;

namespace PartnerPortal.Invoice.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "InvoiceViewerType")]
    [JSONWithTypeAttribute]
    public class InvoiceViewerTypeController:BaseAPIController
    {
        InvoiceViewerTypeManager _manager = new InvoiceViewerTypeManager();

        [HttpGet]
        [Route("GetInvoiceViewerTypeInfo")]
        public IEnumerable<InvoiceViewerTypeInfo> GetInvoiceViewerTypeInfo(string serializedFilter = null)
        {
            InvoiceViewerTypeInfoFilter deserializedFilter = serializedFilter != null ? Vanrise.Common.Serializer.Deserialize<InvoiceViewerTypeInfoFilter>(serializedFilter) : null;
            return _manager.GetInvoiceViewerTypeInfo(deserializedFilter);
        }
        [HttpGet]
        [Route("GetInvoiceViewerType")]
        public InvoiceViewerType GetInvoiceViewerType(Guid invoiceViewerTypeId)
        {
            return _manager.GetInvoiceViewerType(invoiceViewerTypeId);
        }
        [HttpGet]
        [Route("GetInvoiceViewerTypeRuntime")]
        public InvoiceViewerTypeRuntime GetInvoiceViewerTypeRuntime(Guid invoiceViewerTypeId)
        {
            return _manager.GetInvoiceViewerTypeRuntime(invoiceViewerTypeId);
        }
    }
}