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
    [RoutePrefix(Constants.ROUTE_PREFIX + "Invoice")]
    [JSONWithTypeAttribute]
    public class InvoiceBulkActionsDraftController : BaseAPIController
    {
        //[HttpPost]
        //[Route("EvaluateInvoicesBulkActionsDraft")]
        //public bool EvaluateInvoicesBulkActionsDraft(InvoiceBulkActionDraftInput input)
        //{
        //    InvoiceBulkActionsDraftManager manager = new InvoiceBulkActionsDraftManager();
        //    return manager.EvaluateInvoicesBulkActionsDraft(input);
        //}
    }
}