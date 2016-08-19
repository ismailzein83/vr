using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Invoice.Business;
using Vanrise.Invoice.Entities;
using Vanrise.Web.Base;

namespace Vanrise.Invoice.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "InvoiceType")]
    public class InvoiceTypeController:BaseAPIController
    {
        [HttpGet]
        [Route("GetInvoiceType")]
        public InvoiceType GetInvoiceType(Guid invoiceTypeId)
        {
            InvoiceTypeManager manager = new InvoiceTypeManager();
            return manager.GetInvoiceType(invoiceTypeId);
        }

        [HttpPost]
        [Route("GetFilteredInvoiceTypes")]
        public object GetFilteredInvoiceTypes(Vanrise.Entities.DataRetrievalInput<InvoiceTypeQuery> input)
        {
            InvoiceTypeManager manager = new InvoiceTypeManager();
            return GetWebResponse(input, manager.GetFilteredInvoiceTypes(input));
        }
    }
}