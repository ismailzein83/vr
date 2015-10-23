using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Web.Base;

namespace TOne.WhS.BusinessEntity.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "CustomerZone")]
    public class CustomerZoneController : BaseAPIController
    {
        [HttpGet]
        [Route("GetCustomerZone")]
        public CustomerZones GetCustomerZone(int customerId)
        {
            CustomerZoneManager manager = new CustomerZoneManager();
            return manager.GetCustomerZone(customerId, DateTime.Now, false);
        }
    }
}