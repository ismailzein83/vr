using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Web.Base;
using Vanrise.Invoice.Entities;
using Vanrise.Invoice.Business;
namespace Vanrise.Invoice.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "InvoiceType")]
    public class InvoiceController:BaseAPIController
    {
        [HttpPost]
        [Route("GenerateInvoice")]
        public void GenerateInvoice(GenerateInvoiceInput createInvoiceInput)
        {
            InvoiceManager manager = new InvoiceManager();
            manager.GenerateInvoice(createInvoiceInput);
        }
        [HttpPost]
        [Route("GetFilteredInvoices")]
        public object GetFilteredInvoices(Vanrise.Entities.DataRetrievalInput<InvoiceQuery> input)
        {
            InvoiceManager manager = new InvoiceManager();
            return GetWebResponse(input, manager.GetFilteredInvoices(input));
        }

    }
}