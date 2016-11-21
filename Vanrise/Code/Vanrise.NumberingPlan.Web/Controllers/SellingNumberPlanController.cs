using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.NumberingPlan.Business;
using Vanrise.NumberingPlan.Entities;
using Vanrise.Web.Base;

namespace Vanrise.NumberingPlan.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "SellingNumberPlan")]
    public class NP_SellingNumberPlanController : BaseAPIController
    {
        [HttpGet]
        [Route("GetSellingNumberPlans")]
        public IEnumerable<SellingNumberPlanInfo> GetSellingNumberPlans()
        {
            SellingNumberPlanManager manager = new SellingNumberPlanManager();
            return manager.GetSellingNumberPlans();
        }
    }
}