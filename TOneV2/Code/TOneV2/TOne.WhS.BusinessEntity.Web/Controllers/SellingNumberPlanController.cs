﻿using System.Collections.Generic;
using System.Web.Http;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Web.Base;

namespace TOne.WhS.BusinessEntity.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "SellingNumberPlan")]
    public class SellingNumberPlanController : BaseAPIController
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
            return GetWebResponse(input, manager.GetFilteredSellingNumberPlans(input));
        }

        [HttpPost]
        [Route("AddSellingNumberPlan")]
        public TOne.Entities.InsertOperationOutput<SellingNumberPlanDetail> AddSellingNumberPlan(SellingNumberPlan sellingNumberPlan)
        {
            SellingNumberPlanManager manager = new SellingNumberPlanManager();
            return manager.AddSellingNumberPlan(sellingNumberPlan);
        }

        [HttpPost]
        [Route("UpdateSellingNumberPlan")]
        public TOne.Entities.UpdateOperationOutput<SellingNumberPlanDetail> UpdateSellingNumberPlan(SellingNumberPlanToEdit sellingNumberPlan)
        {
            SellingNumberPlanManager manager = new SellingNumberPlanManager();
            return manager.UpdateSellingNumberPlan(sellingNumberPlan);
        }

    }
}