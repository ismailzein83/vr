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
    [RoutePrefix(Constants.ROUTE_PREFIX + "VRComponentType")]
    [JSONWithTypeAttribute]
    public class VRComponentTypeController : BaseAPIController
    {
        VRComponentTypeManager _manager = new VRComponentTypeManager();

        [HttpPost]
        [Route("GetFilteredVRComponentTypes")]
        public object GetFilteredVRComponentTypes(Vanrise.Entities.DataRetrievalInput<VRComponentTypeQuery> input)
        {
            return GetWebResponse(input, _manager.GetFilteredVRComponentTypes(input), "Component Types");
        }

        [HttpGet]
        [Route("GetVRComponentType")]
        public VRComponentType GetVRComponentType(Guid vrComponentTypeId)
        {
            return _manager.GetComponentType(vrComponentTypeId);
        }

        [HttpPost]
        [Route("AddVRComponentType")]
        public Vanrise.Entities.InsertOperationOutput<VRComponentTypeDetail> AddVRComponentType(VRComponentType vrComponentTypeItem)
        {
            return _manager.AddVRComponentType(vrComponentTypeItem);
        }

        [HttpPost]
        [Route("UpdateVRComponentType")]
        public Vanrise.Entities.UpdateOperationOutput<VRComponentTypeDetail> UpdateVRComponentType(VRComponentType vrComponentTypeItem)
        {
            return _manager.UpdateVRComponentType(vrComponentTypeItem);
        }

        [HttpGet]
        [Route("GetVRComponentTypeExtensionConfigs")]
        public IEnumerable<VRComponentTypeConfig> GetVRComponentTypeExtensionConfigs()
        {
            return _manager.GetVRComponentTypeExtensionConfigs();
        }

        [HttpGet]
        [Route("GetVRComponentTypeExtensionConfigById")]
        public VRComponentTypeConfig GetVRComponentTypeExtensionConfigById(Guid extensionConfigId)
        {
            return _manager.GetVRComponentTypeExtensionConfigById(extensionConfigId);
        }
        [HttpGet]
        [Route("GetComponentTypeInfo")]
        public IEnumerable<ComponentTypeInfo> GetComponentTypeInfo(string filter = null)
        {
            ComponentTypeInfoFilter deserializedFilter = (filter != null) ? Vanrise.Common.Serializer.Deserialize<ComponentTypeInfoFilter>(filter) : null;
            return _manager.GetComponentTypeInfo(deserializedFilter);
        }
    }
}