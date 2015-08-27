using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using TOne.BusinessEntity.Business;
using TOne.BusinessEntity.Entities;

namespace TOne.BusinessEntity.Web.Controllers
{
    public class RateAnalysisController:Vanrise.Web.Base.BaseAPIController
    {
        [HttpPost]
        public object GetRateAnalysis(Vanrise.Entities.DataRetrievalInput<RateAnalysisQuery> input)
        {
            RateAnalysisManager manager = new RateAnalysisManager();
            return GetWebResponse(input, manager.GetRateAnalysis(input));
        }
    }
}