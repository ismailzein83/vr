using System.Collections.Generic;
using System.Web.Http;
using TOne.WhS.RouteSync.Business;
using TOne.WhS.RouteSync.Entities.NumberLength;
using Vanrise.Web.Base;

namespace TOne.WhS.RouteSync.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "NumberLength")]
    public class NumberLengthController : BaseAPIController
    {
        NumberLengthManager _manager = new NumberLengthManager();

        [HttpGet]
        [Route("GetNumberLengthEvaluatorExtensionConfigs")]
        public IEnumerable<NumberLengthEvaluatorConfig> GetNumberLengthEvaluatorExtensionConfigs()
        {
            return _manager.GetNumberLengthEvaluatorExtensionConfigs();
        }
    }
}