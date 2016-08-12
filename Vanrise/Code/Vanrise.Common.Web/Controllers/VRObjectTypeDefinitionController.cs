using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Common.Business.VRObject;
using Vanrise.Entities;
using Vanrise.Web.Base;

namespace Vanrise.Common.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "VRObjectTypeDefinition")]
    [JSONWithTypeAttribute]
    public class VRObjectTypeDefinitionController : BaseAPIController
    {
        VRObjectTypeDefinitionManager _manager = new VRObjectTypeDefinitionManager();

        [HttpPost]
        [Route("GetFilteredVRObjectTypeDefinitions")]
        public object GetFilteredVRObjectTypeDefinitions(Vanrise.Entities.DataRetrievalInput<VRObjectTypeDefinitionQuery> input)
        {
            return GetWebResponse(input, _manager.GetFilteredVRObjectTypeDefinitions(input));
        }

        [HttpGet]
        [Route("GetVRObjectTypeDefinition")]
        public VRObjectTypeDefinition GetVRObjectTypeDefinition(Guid VRObjectTypeDefinitionId)
        {
            return _manager.GetVRObjectTypeDefinition(VRObjectTypeDefinitionId);
        }

        [HttpPost]
        [Route("AddVRObjectTypeDefinition")]
        public Vanrise.Entities.InsertOperationOutput<VRObjectTypeDefinitionDetail> AddVRObjectTypeDefinition(VRObjectTypeDefinition VRObjectTypeDefinitionItem)
        {
            return _manager.AddVRObjectTypeDefinition(VRObjectTypeDefinitionItem);
        }

        [HttpPost]
        [Route("UpdateVRObjectTypeDefinition")]
        public Vanrise.Entities.UpdateOperationOutput<VRObjectTypeDefinitionDetail> UpdateVRObjectTypeDefinition(VRObjectTypeDefinition VRObjectTypeDefinitionItem)
        {
            return _manager.UpdateVRObjectTypeDefinition(VRObjectTypeDefinitionItem);
        }

        [HttpGet]
        [Route("GetVRObjectTypeDefinitionsInfo")]
        public IEnumerable<VRObjectTypeDefinitionInfo> GetVRObjectTypeDefinitionsInfo(string filter = null)
        {
            StyleDefinitionFilter deserializedFilter = (filter != null) ? Vanrise.Common.Serializer.Deserialize<StyleDefinitionFilter>(filter) : null;
            return _manager.GetVRObjectTypeDefinitionsInfo(deserializedFilter);
        }
    }
}