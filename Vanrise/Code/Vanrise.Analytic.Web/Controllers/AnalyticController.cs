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
    [RoutePrefix(Constants.ROUTE_PREFIX + "Analytic")]
    public class AnalyticController : Vanrise.Web.Base.BaseAPIController
    {
        [HttpGet]
        [Route("GetDimensions")]
        public IEnumerable<DimensionConfiguration> GetDimensions()
        {
            AnalyticConfigurationManager manager = new AnalyticConfigurationManager();
            return manager.GetDimensions();
        }

        [HttpGet]
        [Route("GetMeasures")]
        public IEnumerable<MeasureConfiguration> GetMeasures()
        {
            AnalyticConfigurationManager manager = new AnalyticConfigurationManager();
            return manager.GetMeasures();
        }
    }
}