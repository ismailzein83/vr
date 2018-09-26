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

        [HttpGet]
        [Route("GetSellingNumberPlan")]
        public SellingNumberPlan GetSellingNumberPlan(int sellingNumberPlanId)
        {
            SellingNumberPlanManager manager = new SellingNumberPlanManager();
            return manager.GetSellingNumberPlan(sellingNumberPlanId);
        }
        [HttpGet]
        [Route("GetMasterSellingNumberPlan")]
        public SellingNumberPlan GetMasterSellingNumberPlan()
        {
            SellingNumberPlanManager manager = new SellingNumberPlanManager();
            return manager.GetMasterSellingNumberPlan();
        }
        [HttpPost]
        [Route("GetFilteredSellingNumberPlans")]
        public object GetFilteredSellingNumberPlans(Vanrise.Entities.DataRetrievalInput<SellingNumberPlanQuery> input)
        {
            SellingNumberPlanManager manager = new SellingNumberPlanManager();
            return GetWebResponse(input, manager.GetFilteredSellingNumberPlans(input), "Selling Number Plans");
        }

        [HttpPost]
        [Route("AddSellingNumberPlan")]
        public Vanrise.Entities.InsertOperationOutput<SellingNumberPlanDetail> AddSellingNumberPlan(SellingNumberPlan sellingNumberPlan)
        {
            SellingNumberPlanManager manager = new SellingNumberPlanManager();
            return manager.AddSellingNumberPlan(sellingNumberPlan);
        }

        [HttpPost]
        [Route("UpdateSellingNumberPlan")]
        public Vanrise.Entities.UpdateOperationOutput<SellingNumberPlanDetail> UpdateSellingNumberPlan(SellingNumberPlanToEdit sellingNumberPlan)
        {
            SellingNumberPlanManager manager = new SellingNumberPlanManager();
            return manager.UpdateSellingNumberPlan(sellingNumberPlan);
        }

    }
}