using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using TOne.Billing.Business;
using TOne.Billing.Entities;

namespace TOne.Billing.Web.Controllers
{
    public class CustomerInvoiceController : Vanrise.Web.Base.BaseAPIController
    {
        [HttpPost]
        public object GetFilteredCustomerInvoices(Vanrise.Entities.DataRetrievalInput<CustomerInvoiceQuery> input)
        {
            CustomerInvoiceManager customerManager = new CustomerInvoiceManager();
            return GetWebResponse(input, customerManager.GetFilteredCustomerInvoices(input));
        }

    }
}