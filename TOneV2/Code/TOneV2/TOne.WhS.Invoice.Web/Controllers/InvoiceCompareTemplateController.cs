using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TOne.WhS.Invoice.Entities;
using TOne.WhS.Invoice.Business;
using Vanrise.Web.Base;
using System.Web.Http;
using Vanrise.Invoice.Entities;
using Vanrise.Entities;

namespace TOne.WhS.Invoice.Web.Controllers
{
    [JSONWithTypeAttribute]
    [RoutePrefix(Constants.ROUTE_PREFIX + "InvoiceCompareTemplateController")]
    public class InvoiceCompareTemplateController : BaseAPIController
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
        public InvoiceComparisonTemplate GetInvoiceCompareTemplate(Guid invoiceTypeId, string partnerId)
        {
            InvoiceComparisonTemplateManager manager = new InvoiceComparisonTemplateManager();
            return manager.GetInvoiceCompareTemplate(invoiceTypeId, partnerId);
        }
        [HttpGet]
        [Route("GetComparisonInvoiceDetail")]
        public ComparisonInvoiceDetail GetComparisonInvoiceDetail(long invoiceId)
        {
            InvoiceComparisonTemplateManager manager = new InvoiceComparisonTemplateManager();
            return manager.GetComparisonInvoiceDetail(invoiceId);
        }
    }
}