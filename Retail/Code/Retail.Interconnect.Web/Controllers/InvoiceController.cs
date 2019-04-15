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
    [RoutePrefix(Constants.ROUTE_PREFIX + "InvoiceController")]
    public class InvoiceController : BaseAPIController
    {

        [HttpPost]
        [Route("UpdateOriginalInvoiceData")]
        public object UpdateOriginalInvoiceData(OriginalInvoiceDataInput input)
        {
            InterconnectInvoiceManager manager = new InterconnectInvoiceManager();
            if (!manager.DoesUserHaveUpdateOriginalInvoiceDataAccess(input.InvoiceId))
                return GetUnauthorizedResponse();
            return manager.UpdateOriginalInvoiceData(input);
        }

        [HttpGet]
        [Route("GetOriginalInvoiceDataRuntime")]
        public OriginalInvoiceDataRuntime GetOriginalInvoiceDataRuntime(long invoiceId)
        {
            InterconnectInvoiceManager manager = new InterconnectInvoiceManager();
            return manager.GetOriginalInvoiceDataRuntime(invoiceId);
        }

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