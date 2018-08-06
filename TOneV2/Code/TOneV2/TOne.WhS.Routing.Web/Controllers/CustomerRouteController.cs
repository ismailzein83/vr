using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using TOne.WhS.Routing.Business;
using TOne.WhS.Routing.Entities;
using Vanrise.Web.Base;

namespace TOne.WhS.Routing.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "CustomerRoute")]
    public class CustomerRouteController : BaseAPIController
    {
        [HttpPost]
        [Route("GetFilteredCustomerRoutes")]
        public object GetFilteredCustomerRoutes(Vanrise.Entities.DataRetrievalInput<CustomerRouteQuery> input)
        {
            CustomerRouteManager manager = new CustomerRouteManager();
            return GetWebResponse(input, manager.GetFilteredCustomerRoutes(input),"Customer Routes");
        }
    }
}