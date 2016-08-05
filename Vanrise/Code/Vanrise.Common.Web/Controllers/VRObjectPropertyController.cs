using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Web.Base;
using Vanrise.Common.Business;
using Vanrise.Entities;

namespace Vanrise.Common.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "VRObjectProperty")]
    [JSONWithTypeAttribute]
    public class VRObjectPropertyController : BaseAPIController
    {
        VRObjectPropertyEvaluatorManager _manager = new VRObjectPropertyEvaluatorManager();

        [HttpGet]
        [Route("GetObjectPropertyExtensionConfigs")]
        public IEnumerable<VRObjectPropertyEvaluatorConfig> GetObjectPropertyExtensionConfigs(string configType)
        {
            System.Threading.Thread.Sleep(2000);
            return _manager.GetObjectPropertyExtensionConfigs(configType);
        }
    }
}