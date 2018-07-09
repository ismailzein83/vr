using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;
using Vanrise.Web.Base;

namespace Vanrise.GenericData.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "LKUPBusinessEntityDefinition")]
    [JSONWithTypeAttribute]
    public class LKUPBusinessEntityDefinitionController : BaseAPIController
    {
        LKUPBusinessEntityDefinitionManager _manager = new LKUPBusinessEntityDefinitionManager();

        [HttpGet]
        [Route("GetLookUpBESelectorRuntimeInfo")]
        public LKUPBESelectorRuntimeInfo GetLookUpBESelectorRuntimeInfo(Guid businessEntityDefinitionId)
        {
            return _manager.GetLookUpBESelectorRuntimeInfo(businessEntityDefinitionId);
        }

        [HttpGet]
        [Route("GetLookUpBEExtendedSettingsConfigs")]
        public IEnumerable<LKUPBEExtendedSettingsConfig> GetLookUpBEExtendedSettingsConfigs()
        {
            return _manager.GetLookUpBEExtendedSettingsConfigs();
        }
    }
}