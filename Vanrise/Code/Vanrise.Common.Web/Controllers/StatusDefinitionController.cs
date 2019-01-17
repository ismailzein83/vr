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
    [RoutePrefix(Constants.ROUTE_PREFIX + "StatusDefinition")]
    [JSONWithTypeAttribute]
    public class StatusDefinitionController:BaseAPIController
    {
        StatusDefinitionManager _manager = new StatusDefinitionManager();

        [HttpPost]
        [Route("GetFilteredStatusDefinitions")]
        public object GetFilteredStatusDefinitions(Vanrise.Entities.DataRetrievalInput<StatusDefinitionQuery> input)
        {
            return GetWebResponse(input, _manager.GetFilteredStatusDefinitions(input), "Status Definitions");
        }

        [HttpGet]
        [Route("GetStatusDefinitionStylesByBusinessEntityId")]
        public List<StatusDefinitionStyle> GetStatusDefinitionStylesByBusinessEntityId(Guid businessEntityId)
        {
            return _manager.GetStatusDefinitionStylesByBusinessEntityId(businessEntityId);
        }


        [HttpPost]
        [Route("AddStatusDefinition")]
        public Vanrise.Entities.InsertOperationOutput<StatusDefinitionDetail> AddStatusDefinition(StatusDefinition statusDefinitionItem)
        {
            return _manager.AddStatusDefinition(statusDefinitionItem);
        }

        [HttpPost]
        [Route("UpdateStatusDefinition")]
        public Vanrise.Entities.UpdateOperationOutput<StatusDefinitionDetail> UpdateStatusDefinition(StatusDefinition statusDefinitionItem)
        {
            return _manager.UpdateStatusDefinition(statusDefinitionItem);
        }

        [HttpGet]
        [Route("GetStatusDefinition")]
        public StatusDefinition GetStatusDefinition(Guid statusDefinitionId)
        {
            return _manager.GetStatusDefinition(statusDefinitionId);
        }

        [HttpGet]
        [Route("GetStatusDefinitionsInfo")]
        public IEnumerable<StatusDefinitionInfo> GetStatusDefinitionsInfo(string filter = null)
        {
            StatusDefinitionInfoFilter deserializedFilter = (filter != null) ? Vanrise.Common.Serializer.Deserialize<StatusDefinitionInfoFilter>(filter) : null;
            return _manager.GetStatusDefinitionsInfo(deserializedFilter);
        }
        [HttpGet]
        [Route("GetRemoteStatusDefinitionsInfo")]
        public IEnumerable<StatusDefinitionInfo> GetRemoteStatusDefinitionsInfo(Guid connectionId, string filter = null)
        {
            return _manager.GetRemoteStatusDefinitionsInfo(connectionId, filter);
        }


    }
}