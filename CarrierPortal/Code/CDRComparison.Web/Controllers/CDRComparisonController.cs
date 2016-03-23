using CDRComparison.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Entities;
using Vanrise.Web.Base;

namespace CDRComparison.Web.Controllers
{
    [JSONWithType]
    [RoutePrefix(Constants.ROUTE_PREFIX + "CDRComparison")]
    public class CDRComparisonController : BaseAPIController
    {
        CDRComparisonManager _manager = new CDRComparisonManager();

        [HttpGet]
        [Route("GetCDRSourceTemplateConfigs")]
        public IEnumerable<TemplateConfig> GetCDRSourceTemplateConfigs()
        {
            return _manager.GetCDRSourceTemplateConfigs();
        }

        [HttpGet]
        [Route("GetFileReaderTemplateConfigs")]
        public IEnumerable<TemplateConfig> GetFileReaderTemplateConfigs()
        {
            return _manager.GetFileReaderTemplateConfigs();
        }
    }
}