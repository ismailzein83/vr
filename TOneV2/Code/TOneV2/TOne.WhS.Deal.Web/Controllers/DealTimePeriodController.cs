using System;
using System.Collections.Generic;
using System.Web.Http;
using TOne.WhS.Deal.Business;
using TOne.WhS.Deal.Entities;
using Vanrise.Web.Base;

namespace TOne.WhS.Deal.Web.Controllers
{
    [JSONWithTypeAttribute]
    [RoutePrefix(Constants.ROUTE_PREFIX + "DealTimePeriod")]
    public class DealTimePeriodController : Vanrise.Web.Base.BaseAPIController
    {
        [HttpGet]
        [Route("GetDealTimePeriodTemplateConfigs")]
        public IEnumerable<DealTimePeriodConfig> GetDealTimePeriodTemplateConfigs()
        {
            return new DealTimePeriodManager().GetDealTimePeriodTemplateConfigs();
        }
    }
}