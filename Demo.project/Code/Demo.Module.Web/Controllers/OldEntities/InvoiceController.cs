using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Web.Base;
using Demo.Module.Entities;
using Demo.Module.Business;
using Vanrise.Entities;


namespace Demo.Module.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "Invoice")]
    [JSONWithTypeAttribute]
    public class InvoiceController : BaseAPIController
    {
        InvoiceManager invoiceManager = new InvoiceManager();

        [HttpGet]
        [Route("GetInvoices")]
        public List<Invoice> GetInvoices()
        {
            return invoiceManager.GetInvoices();

        }
       
    }
}
