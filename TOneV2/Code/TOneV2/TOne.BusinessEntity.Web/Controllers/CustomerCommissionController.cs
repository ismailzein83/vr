using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TOne.BusinessEntity.Business;

namespace TOne.BusinessEntity.Web.Controllers
{
    public class CustomerCommissionController : Vanrise.Web.Base.BaseAPIController
    {
        public object GetCustomerCommissions(Vanrise.Entities.DataRetrievalInput<string> input)
        {
            CustomerCommissionManager manager = new CustomerCommissionManager();
            return GetWebResponse(input, manager.GetCustomerCommissions(input));
        }
    }
}