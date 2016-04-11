using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Vanrise.Analytic.Business;
using Vanrise.Analytic.Entities;
using Vanrise.Web.Base;
namespace Vanrise.Analytic.Web.Controllers
{
    [JSONWithTypeAttribute]
    [RoutePrefix(Constants.ROUTE_PREFIX + "AnalyticConfiguration")]
    public class AnalyticConfigurationController : Vanrise.Web.Base.BaseAPIController
    {
        [HttpGet]
        [Route("GetDimensionsInfo")]
        public IEnumerable<DimensionInfo<DimensionConfiguration>> GetDimensionsInfo()
        {
            AnalyticConfigurationManager manager = new AnalyticConfigurationManager();
            return manager.GetDimensionsInfo();
        }

        [HttpGet]
        [Route("GetMeasuresInfo")]
        public IEnumerable<MeasureInfo> GetMeasuresInfo()
        {
            AnalyticConfigurationManager manager = new AnalyticConfigurationManager();
            return manager.GetMeasuresInfo();
        }

        [HttpPost]
        [Route("GetAnalyticRecords")]
        public Object GetAnalyticRecords(Vanrise.Entities.DataRetrievalInput<AnalyticQuery> input)
        {
            AnalyticManager manager = new AnalyticManager();
            return GetWebResponse(input, manager.GetFiltered(input));
        }
    }
}