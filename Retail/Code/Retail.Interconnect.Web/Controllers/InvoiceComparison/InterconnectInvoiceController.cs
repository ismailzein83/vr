using Retail.Interconnect.Business;
using Retail.Interconnect.Entities;
using Retail.Interconnect.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Entities;
using Vanrise.Web.Base;

namespace Retail.Voice.Web.Controllers
{
    [JSONWithTypeAttribute]
    [RoutePrefix(Constants.ROUTE_PREFIX + "InterconnectInvoiceController")]
    public class InterconnnectInvoiceController : BaseAPIController
    {

        [HttpGet]
        [Route("GetInvoiceDetails")]
        public ComparisonInvoiceDetail GetInvoiceDetails(long invoiceId, InvoiceCarrierType invoiceCarrierType)
        {
            InterconnectInvoiceManager manager = new InterconnectInvoiceManager();
            return manager.GetInvoiceDetails(invoiceId, invoiceCarrierType);
        }

        [HttpGet]
        [Route("DoesInvoiceReportExist")]
        public bool DoesInvoiceReportExist(bool isCustomer)
        {
            InterconnectInvoiceManager manager = new InterconnectInvoiceManager();
            return manager.DoesInvoiceReportExist(isCustomer);
        }
    }
}