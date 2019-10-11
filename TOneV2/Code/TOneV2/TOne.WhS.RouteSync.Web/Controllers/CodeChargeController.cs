using System.Collections.Generic;
using System.Web.Http;
using TOne.WhS.RouteSync.Business;
using TOne.WhS.RouteSync.Entities.CodeCharge;
using Vanrise.Web.Base;

namespace TOne.WhS.RouteSync.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "CodeCharge")]
    public class CodeChargeController : BaseAPIController
    {
        CodeChargeManager _manager = new CodeChargeManager();

        [HttpGet]
        [Route("GetCodeChargeEvaluatorExtensionConfigs")]
        public IEnumerable<CodeChargeEvaluatorConfig> GetCodeChargeEvaluatorExtensionConfigs()
        {
            return _manager.GetCodeChargeEvaluatorExtensionConfigs();
        }
    }
}