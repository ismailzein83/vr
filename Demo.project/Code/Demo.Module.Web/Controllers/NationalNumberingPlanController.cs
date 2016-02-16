using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Demo.Module.Business;
using Demo.Module.Entities;
using Vanrise.Web.Base;

namespace Demo.Module.Web.Controllers
{
     [RoutePrefix(Constants.ROUTE_PREFIX + "NationalNumberingPlan")]
    public class Demo_NationalNumberingPlanController : BaseAPIController
    {
        [HttpPost]
        [Route("GetFilteredNationalNumberingPlans")]
         public object GetFilteredNationalNumberingPlans(Vanrise.Entities.DataRetrievalInput<NationalNumberingPlanQuery> input)
        {
            NationalNumberingPlanManager manager = new NationalNumberingPlanManager();
            return GetWebResponse(input, manager.GetFilteredNationalNumberingPlans(input));
        }

        [HttpGet]
        [Route("GetNationalNumberingPlan")]
        public NationalNumberingPlan GetNationalNumberingPlan(int operatorProfileId)
        {
            NationalNumberingPlanManager manager = new NationalNumberingPlanManager();
            return manager.GetNationalNumberingPlan(operatorProfileId);
        }

        [HttpGet]
        [Route("GetNationalNumberingPlansInfo")]
        public IEnumerable<NationalNumberingPlanInfo> GetNationalNumberingPlansInfo()
        {
            NationalNumberingPlanManager manager = new NationalNumberingPlanManager();
            return manager.GetNationalNumberingPlansInfo();
        }

        [HttpPost]
        [Route("AddNationalNumberingPlan")]
        public Vanrise.Entities.InsertOperationOutput<NationalNumberingPlanDetail> AddNationalNumberingPlan(NationalNumberingPlan operatorProfile)
        {
            NationalNumberingPlanManager manager = new NationalNumberingPlanManager();
            return manager.AddNationalNumberingPlan(operatorProfile);
        }
        [HttpPost]
        [Route("UpdateNationalNumberingPlan")]
        public Vanrise.Entities.UpdateOperationOutput<NationalNumberingPlanDetail> UpdateNationalNumberingPlan(NationalNumberingPlan operatorProfile)
        {
            NationalNumberingPlanManager manager = new NationalNumberingPlanManager();
            return manager.UpdateNationalNumberingPlan(operatorProfile);
        }
    }
}