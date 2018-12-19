using System.Collections.Generic;
using System.Web.Http;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;
using Vanrise.Web.Base;

namespace Vanrise.GenericData.Web.Controllers
{
    [JSONWithTypeAttribute]
    [RoutePrefix(Constants.ROUTE_PREFIX + "CompositeRecordCondition")]

    public class CompositeRecordConditionController : BaseAPIController
    {
        CompositeRecordConditionManager _manager = new CompositeRecordConditionManager();

        [HttpGet]
        [Route("GetCompositeRecordConditionConfigs")]
        public IEnumerable<CompositeRecordConditionConfig> GetCompositeRecordConditionConfigs()
        {
            return _manager.GetCompositeRecordConditionConfigs();
        }
    }
}