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
    [RoutePrefix(Constants.ROUTE_PREFIX + "Invoice")]
    [JSONWithTypeAttribute]
    public class CPInvoiceController : BaseAPIController
    {
        [HttpPost]
        [Route("GetFilteredInvoices")]
        public object GetFilteredInvoices(Vanrise.Entities.DataRetrievalInput<InvoiceAppQuery> input)
        {
            InvoiceManager manager = new InvoiceManager();
            return GetWebResponse(input, manager.GetFilteredInvoices(input));
        }
       
    }
}