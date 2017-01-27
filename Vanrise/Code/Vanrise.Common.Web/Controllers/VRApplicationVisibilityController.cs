using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.Web.Base;

namespace Vanrise.Common.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "VRApplicationVisibility")]
    [JSONWithTypeAttribute]
    public class VRApplicationVisibilityController : BaseAPIController
    {
        VRApplicationVisibilityManager _manager = new VRApplicationVisibilityManager();

        [HttpPost]
        [Route("GetFilteredVRApplicationVisibilities")]
        public object GetFilteredVRApplicationVisibilities(Vanrise.Entities.DataRetrievalInput<VRApplicationVisibilityQuery> input)
        {
            return GetWebResponse(input, _manager.GetFilteredVRApplicationVisibilities(input));
        }

        [HttpGet]
        [Route("GetVRApplicationVisibilityEditorRuntime")]
        public VRApplicationVisibilityEditorRuntime GetVRApplicationVisibilityEditorRuntime(Guid vrApplicationVisibilityId)
        {
            return _manager.GetVRApplicationVisibilityEditorRuntime(vrApplicationVisibilityId);
        }

        [HttpPost]
        [Route("AddVRApplicationVisibility")]
        public Vanrise.Entities.InsertOperationOutput<VRApplicationVisibilityDetail> AddVRApplicationVisibility(VRApplicationVisibility vrApplicationVisibilityItem)
        {
            return _manager.AddVRApplicationVisibility(vrApplicationVisibilityItem);
        }

        [HttpPost]
        [Route("UpdateVRApplicationVisibility")]
        public Vanrise.Entities.UpdateOperationOutput<VRApplicationVisibilityDetail> UpdateVRApplicationVisibility(VRApplicationVisibility vrApplicationVisibilityItem)
        {
            return _manager.UpdateVRApplicationVisibility(vrApplicationVisibilityItem);
        }

        [HttpGet]
        [Route("GetVRApplicationVisibiltiesInfo")]
        public IEnumerable<VRApplicationVisibilityInfo> GetVRApplicationVisibiltiesInfo(string filter = null)
        {
            VRApplicationVisibilityFilter deserializedFilter = (filter != null) ? Vanrise.Common.Serializer.Deserialize<VRApplicationVisibilityFilter>(filter) : null;
            return _manager.GetVRApplicationVisibiltiesInfo(deserializedFilter);
        }

        [HttpGet]
        [Route("GetVRModuleVisibilityExtensionConfigs")]
        public IEnumerable<VRModuleVisibilityConfig> GetVRModuleVisibilityExtensionConfigs()
        {
            return _manager.GetVRModuleVisibilityExtensionConfigs();
        }
    }
}