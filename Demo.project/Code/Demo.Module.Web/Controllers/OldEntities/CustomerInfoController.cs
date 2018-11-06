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
    [RoutePrefix(Constants.ROUTE_PREFIX + "CustomerInfo")]
    [JSONWithTypeAttribute]
    public class CustomerInfoController : BaseAPIController
    {
        CustomerInfoManager customerInfoManager = new CustomerInfoManager();

        [HttpPost]
        [Route("GetCustomerInfo")]
        public CustomerInfo GetCustomerInfo()
        {
            return customerInfoManager.GetCustomerInfo();
        }
    }
}
