using System.Collections.Generic;
using System.Web.Http;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.Web.Base;

namespace Vanrise.Common.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "HttpCallConnectionInterceptors")]
    public class HttpCallConnectionInterceptorsController : BaseAPIController
    {
        [HttpGet]
        [Route("GetHttpConnectionCallInterceptorTemplateConfigs")]
        public IEnumerable<VRHttpConnectionCallInterceptorConfig> GetHttpConnectionCallInterceptorTemplateConfigs()
        {
            VRHttpConnectionCallInterceptorManager manager = new VRHttpConnectionCallInterceptorManager();
            return manager.GetHttpConnectionCallInterceptorTemplateConfigs();
        }
    }
}