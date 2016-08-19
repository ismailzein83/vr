using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TOne.WhS.Invoice.Entities;
using TOne.WhS.Invoice.Business;
using Vanrise.Web.Base;
using System.Web.Http;

namespace TOne.WhS.Invoice.Web.Controllers
{
    [JSONWithTypeAttribute]
    [RoutePrefix(Constants.ROUTE_PREFIX + "WhSInvoice")]
    public class WhSInvoiceController:BaseAPIController
    {
        [HttpPost]
        [Route("GetInvoiceCarriers")]
        public IEnumerable<InvoiceCarrier> GetInvoiceCarriers(InvoiceCarrierFilter filter)
        {
            InvoiceManager manager = new InvoiceManager();
            return manager.GetInvoiceCarriers(filter);
        }
    }
}