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
    [RoutePrefix(Constants.ROUTE_PREFIX + "InterconnectInvoiceCompareTemplateController")]
    public class InterconnectInvoiceCompareTemplateController : BaseAPIController
    {
        [HttpPost]
        [Route("SaveInvoiceCompareTemplate")]
        public bool SaveInvoiceCompareTemplate(InvoiceComparisonTemplate invoiceComparisonTemplate)
        {
            InvoiceComparisonTemplateManager manager = new InvoiceComparisonTemplateManager();
            return manager.TryAddOrUpdateInvoiceCompareTemplate(invoiceComparisonTemplate);
        }
        [HttpGet]
        [Route("GetInvoiceCompareTemplate")]
        public InvoiceComparisonTemplate GetInvoiceCompareTemplate(Guid invoiceTypeId, string partnerId, InvoiceCarrierType invoiceCarrierType)
        {
            InvoiceComparisonTemplateManager manager = new InvoiceComparisonTemplateManager();
            return manager.GetInvoiceCompareTemplate(invoiceTypeId, partnerId);
        }

    }
}