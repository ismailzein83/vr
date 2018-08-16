using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using SOM.Main.Business;
using SOM.Main.Entities;
using Vanrise.Web.Base;
using Vanrise.Common;

namespace SOM.Main.Web.Controllers
{
    [JSONWithTypeAttribute]
    [RoutePrefix(Constants.ROUTE_PREFIX + "Billing")]
    public class BillingController : BaseAPIController
    {
        [HttpGet]
        [Route("GetServices")]
        public List<Service> GetServices(string ratePlanId)
        {
            throw new NotImplementedException();
            //BillingManager manager = new BillingManager();
            //return manager.GetServices().FindAll(c => c.RatePlanId.ToLower() == ratePlanId.ToLower());
        }
        [HttpGet]
        [Route("GetRatePlans")]
        public List<RatePlan> GetRatePlans()
        {
            throw new NotImplementedException();
            //BillingManager manager = new BillingManager();
            //return manager.GetRatePlans();
        }
    }
}