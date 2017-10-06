using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TOne.WhS.Invoice.Entities;
using TOne.WhS.Invoice.Business;
using Vanrise.Web.Base;
using System.Web.Http;
using Vanrise.Invoice.Entities;

namespace TOne.WhS.Invoice.Web.Controllers
{
    [JSONWithTypeAttribute]
    [RoutePrefix(Constants.ROUTE_PREFIX + "WhSInvoice")]
    public class WhSInvoiceController:BaseAPIController
    {

        [HttpPost]
        [Route("CompareInvoices")]
        public object CompareInvoices(Vanrise.Entities.DataRetrievalInput<InvoiceComparisonInput> input)
        {
            InvoiceManager manager = new InvoiceManager();
            return GetWebResponse(input, manager.CompareInvoices(input));
        }

        [HttpPost]
        [Route("UpdateOriginalInvoiceData")]
        public bool UpdateOriginalInvoiceData(OriginalInvoiceDataInput input)
        {
            InvoiceManager manager = new InvoiceManager();
            return manager.UpdateOriginalInvoiceData(input);
        }


        
    }
}