using Retail.RA.Business;
using Retail.RA.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Web.Base;

namespace Retail.RA.Web.Controller
{
    [JSONWithTypeAttribute]
    [RoutePrefix(Constants.ROUTE_PREFIX + "RAPeriodDefinitionController")]
    public class RAPeriodDefinitionController : BaseAPIController
    {
        //[HttpPost]
        //[Route("GetPeriodDefinitionInfo")]
        //public IEnumerable<PeriodDefinitionInfo> GetPeriodDefinitionInfo(PeriodDefinitionInfoInput periodDefinitionInfoInput)
        //{
        //    var manager = new PeriodDefinitionManager();
        //    return manager.GetPeriodDefinitionInfo(periodDefinitionInfoInput.OperatorId, periodDefinitionInfoInput.TrafficType);
        //}
        
    }
    public class PeriodDefinitionInfoInput
    {
        public long? OperatorId { get; set; }
        public TrafficType? TrafficType { get; set; }
    }
}