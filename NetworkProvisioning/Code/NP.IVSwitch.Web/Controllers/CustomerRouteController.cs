using NP.IVSwitch.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using NP.IVSwitch.Business;
using Vanrise.Web.Base;

namespace NP.IVSwitch.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "CustomerRoute")]
    [JSONWithTypeAttribute]
    public class NP_IVSwitch_CustomerRouteController : BaseAPIController
    {
        CustomerRouteManager _manager = new CustomerRouteManager();

        [HttpPost]
        [Route("GetFilteredCustomerRoutes")]
        public object GetFilteredCustomerRoutes(Vanrise.Entities.DataRetrievalInput<CustomerRouteQuery> input)
        {
            return GetWebResponse(input, _manager.GetFilteredCustomerRoutes(input));
        }

    }
}