using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using TOne.WhS.Routing.Business;
using TOne.WhS.Routing.Entities;
using Vanrise.Web.Base;

namespace TOne.WhS.Routing.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "QualityConfiguration")]
    [JSONWithTypeAttribute]
    public class QualityConfigurationController : Vanrise.Web.Base.BaseAPIController
    {
        [HttpGet]
        [Route("GetQualityConfigurationFields")]
        public List<AnalyticMeasureInfo> GetQualityConfigurationFields()
        {
            ConfigManager configManager = new ConfigManager();
            return configManager.GetQualityConfigurationFields();
        }

        [HttpGet]
        [Route("GetQualityConfigurationInfo")]
        public IEnumerable<QualityConfigurationInfo> GetQualityConfigurationInfo(string filter = null)
        {
            QualityConfigurationInfoFilter qualityConfigurationInfoFilter = filter != null ? Vanrise.Common.Serializer.Deserialize<QualityConfigurationInfoFilter>(filter) : null;
            return new QualityConfigurationManager().GetQualityConfigurationInfo(qualityConfigurationInfoFilter);
        }

        [HttpGet]
        [Route("TryCompileQualityConfigurationExpression")]
        public bool TryCompileQualityConfigurationExpression(string qualityConfigurationExpression)
        {
            return new QualityConfigurationManager().TryCompileQualityConfigurationExpression(qualityConfigurationExpression);
        }
    }
}