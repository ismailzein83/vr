using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Notification.Business;
using Vanrise.Notification.Entities;
using Vanrise.Web.Base;

namespace Vanrise.Notification.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "VRActionDefinition")]
    [JSONWithTypeAttribute]
    public class VRActionDefinitionController : BaseAPIController
    {
        [HttpGet]
        [Route("GetVRActionDefinitionConfigs")]
        public IEnumerable<VRActionDefinitionConfig> GetVRActionDefinitionConfigs()
        {
            VRActionDefinitionManager manager = new VRActionDefinitionManager();
            return manager.GetVRActionDefinitionConfigs();
        }

        [HttpGet]
        [Route("GetVRActionDefinitionsInfo")]
        public IEnumerable<VRActionDefinitionInfo> GetVRActionDefinitionsInfo(string filter = null)
        {
            VRActionDefinitionFilter deserializedFilter = (filter != null) ? Vanrise.Common.Serializer.Deserialize<VRActionDefinitionFilter>(filter) : null;
            VRActionDefinitionManager manager = new VRActionDefinitionManager();
            return manager.GetVRActionDefinitionsInfo(deserializedFilter);
        }

        [HttpGet]
        [Route("GetVRActionDefinition")]
        public VRActionDefinition GetVRActionDefinition(Guid VRActionDefinitionId)
        {
            VRActionDefinitionManager manager = new VRActionDefinitionManager();
            return manager.GetVRActionDefinition(VRActionDefinitionId);
        }
    }
}