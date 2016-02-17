using Demo.Module.Business;
using Demo.Module.Entities;
using System.Web.Http;
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
        public NationalNumberingPlan GetNationalNumberingPlan(int nationalNumberingPlanId)
        {
            NationalNumberingPlanManager manager = new NationalNumberingPlanManager();
            return manager.GetNationalNumberingPlan(nationalNumberingPlanId);
        }

        [HttpPost]
        [Route("AddNationalNumberingPlan")]
        public Vanrise.Entities.InsertOperationOutput<NationalNumberingPlanDetail> AddNationalNumberingPlan(NationalNumberingPlan nationalNumberPlan)
        {
            NationalNumberingPlanManager manager = new NationalNumberingPlanManager();
            return manager.AddNationalNumberingPlan(nationalNumberPlan);
        }
        [HttpPost]
        [Route("UpdateNationalNumberingPlan")]
        public Vanrise.Entities.UpdateOperationOutput<NationalNumberingPlanDetail> UpdateNationalNumberingPlan(NationalNumberingPlan nationalNumberPlan)
        {
            NationalNumberingPlanManager manager = new NationalNumberingPlanManager();
            return manager.UpdateNationalNumberingPlan(nationalNumberPlan);
        }
    }
}