using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using TOne.Billing.Business;
using TOne.Billing.Entities;

namespace TOne.Billing.Web.Controllers
{
    public class SupplierInvoiceController : Vanrise.Web.Base.BaseAPIController
    {
        [HttpPost]
        public object GetFilteredSupplierInvoices(Vanrise.Entities.DataRetrievalInput<SupplierInvoiceQuery> input)
        {
            SupplierInvoiceManager manager = new SupplierInvoiceManager();
            return GetWebResponse(input, manager.GetFilteredSupplierInvoices(input));
        }

        [HttpPost]
        public object GetFilteredSupplierInvoiceDetails(Vanrise.Entities.DataRetrievalInput<int> input)
        {
            SupplierInvoiceManager manager = new SupplierInvoiceManager();
            return GetWebResponse(input, manager.GetFilteredSupplierInvoiceDetails(input));
        }
    }
}