using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using SOM.Main.Business;
using SOM.Main.Entities;
using Vanrise.Web.Base;

namespace SOM.Main.Web.Controllers
{
    [JSONWithTypeAttribute]
    [RoutePrefix(Constants.ROUTE_PREFIX + "Billing")]
    public class BillingController : BaseAPIController
    {
        [HttpGet]
        [Route("GetServices")]
        public List<TelephoneService> GetServices()
        {
            BillingManager manager = new BillingManager();
            return manager.GetServices();
        }
        [HttpGet]
        [Route("GetRatePlans")]
        public List<RatePlan> GetRatePlans()
        {
            BillingManager manager = new BillingManager();
            return manager.GetRatePlans();
        }
    }
}