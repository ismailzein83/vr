using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Reprocess.Business;
using Vanrise.Reprocess.Entities;
using Vanrise.Web.Base;
using Vanrise.Entities;

namespace Vanrise.Reprocess.Web.Controllers
{

    [RoutePrefix(Constants.ROUTE_PREFIX + "ReprocessDefinition")]
    [JSONWithTypeAttribute]
    public class ReprocessDefinitionController : BaseAPIController
    {
        ReprocessDefinitionManager _manager = new ReprocessDefinitionManager();

        [HttpPost]
        [Route("GetFilteredReprocessDefinitions")]
        public object GetFilteredReprocessDefinitions(Vanrise.Entities.DataRetrievalInput<ReprocessDefinitionQuery> input)
        {
            return GetWebResponse(input, _manager.GetFilteredReprocessDefinitions(input));
        }

        [HttpGet]
        [Route("GetReprocessDefinition")]
        public ReprocessDefinition GetReprocessDefinition(Guid reprocessDefinitionId)
        {
            return _manager.GetReprocessDefinition(reprocessDefinitionId);
        }

        [HttpPost]
        [Route("AddReprocessDefinition")]
        public Vanrise.Entities.InsertOperationOutput<ReprocessDefinitionDetail> AddReprocessDefinition(ReprocessDefinition reprocessDefinitionItem)
        {
            return _manager.AddReprocessDefinition(reprocessDefinitionItem);
        }

        [HttpPost]
        [Route("UpdateReprocessDefinition")]
        public Vanrise.Entities.UpdateOperationOutput<ReprocessDefinitionDetail> UpdateReprocessDefinition(ReprocessDefinition reprocessDefinitionItem)
        {
            return _manager.UpdateReprocessDefinition(reprocessDefinitionItem);
        }

        [HttpGet]
        [Route("GetReprocessDefinitionsInfo")]
        public IEnumerable<ReprocessDefinitionInfo> GetReprocessDefinitionsInfo(string serializedFilter)
        {
            ReprocessDefinitionInfoFilter filter = serializedFilter != null ? Vanrise.Common.Serializer.Deserialize<ReprocessDefinitionInfoFilter>(serializedFilter) : null;
            return _manager.GetReprocessDefinitionsInfo(filter);
        }
    }
}