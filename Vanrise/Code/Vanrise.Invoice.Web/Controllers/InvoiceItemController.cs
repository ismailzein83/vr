using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Entities;
using Vanrise.Invoice.Business;
using Vanrise.Invoice.Entities;
using Vanrise.Web.Base;

namespace Vanrise.Invoice.Web.Controllers
{
     [RoutePrefix(Constants.ROUTE_PREFIX + "InvoiceItem")]
     [JSONWithTypeAttribute]

    public class InvoiceItemController : BaseAPIController
    {
         [HttpPost]
         [Route("GetFilteredInvoiceItems")]
         public object GetFilteredInvoiceItems(Vanrise.Entities.DataRetrievalInput<InvoiceItemQuery> input)
         {
             InvoiceItemManager manager = new InvoiceItemManager();
             return GetWebResponse(input, manager.GetFilteredInvoiceItems(input));
         }
         
    }
}