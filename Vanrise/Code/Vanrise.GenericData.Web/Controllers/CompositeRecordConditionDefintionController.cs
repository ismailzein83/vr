using System.Collections.Generic;
using System.Web.Http;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;
using Vanrise.Web.Base;

namespace Vanrise.GenericData.Web.Controllers
{
    [JSONWithTypeAttribute] 
    [RoutePrefix(Constants.ROUTE_PREFIX + "CompositeRecordCondition")]

    public class CompositeRecordConditionDefintionController : BaseAPIController
    {
        CompositeRecordConditionDefinitionManager _manager = new CompositeRecordConditionDefinitionManager();

        [HttpGet]
        [Route("GetCompositeRecordConditionDefinitionSettingConfigs")]
        public IEnumerable<CompositeRecordConditionDefinitionSettingConfig> GetCompositeRecordConditionDefinitionSettingConfigs()
        {
            return _manager.GetCompositeRecordConditionDefinitionSettingConfigs();
        }
    }
}