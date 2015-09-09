using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using TOne.BusinessEntity.Business;
using TOne.BusinessEntity.Entities;

namespace TOne.BusinessEntity.Web.Controllers
{
    public class CustomerCommissionController : Vanrise.Web.Base.BaseAPIController
    {
        [HttpPost]
        public object GetCustomerCommissions(Vanrise.Entities.DataRetrievalInput<CustomerCommissionQuery> input)
        {
            CustomerCommissionManager manager = new CustomerCommissionManager();
            return GetWebResponse(input, manager.GetCustomerCommissions(input));
        }
    }
}