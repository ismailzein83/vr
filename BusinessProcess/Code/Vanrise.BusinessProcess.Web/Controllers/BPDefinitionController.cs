using System;
using System.Collections.Generic;
using System.Web.Http;
using Vanrise.BusinessProcess.Business;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Web.Base;

namespace Vanrise.BusinessProcess.Web.Controllers
{
    [JSONWithType]
    [RoutePrefix(Constants.ROUTE_PREFIX + "BPDefinition")]
    public class BPDefinitionController : BaseAPIController
    {
        BPDefinitionManager manager = new BPDefinitionManager();

        [HttpPost]
        [Route("GetFilteredBPDefinitions")]
        public object GetFilteredBPDefinitions(Vanrise.Entities.DataRetrievalInput<BPDefinitionQuery> input)
        {
            return GetWebResponse(input, manager.GetFilteredBPDefinitions(input, Security.Entities.ContextFactory.GetContext().GetLoggedInUserId()));
        }

        [HttpPost]
        [Route("GetFilteredBPDefinitionsForTechnical")]
        public object GetFilteredBPDefinitionsForTechnical(Vanrise.Entities.DataRetrievalInput<BPDefinitionQuery> input)
        {
            return GetWebResponse(input, manager.GetFilteredBPDefinitions(input, null));
        }

        [HttpPost]
        [Route("AddBPDefinition")]
        public Vanrise.Entities.InsertOperationOutput<BPDefinitionDetail> AddBPDefinition(BPDefinition bpDefinition)
        {
            return manager.AddBPDefinition(bpDefinition);
        }

        [HttpPost]
        [Route("UpdateBPDefinition")]
        public Vanrise.Entities.UpdateOperationOutput<BPDefinitionDetail> UpdateBPDefinition(BPDefinition bPDefinition)
        {
            return manager.UpdateBPDefinition(bPDefinition);
        }

        [HttpGet]
        [Route("GetBPDefinitionsInfo")]
        public IEnumerable<BPDefinitionInfo> GetBPDefinitionsInfo(string serializedFilter)
        {
            BPDefinitionInfoFilter filter = serializedFilter != null ? Vanrise.Common.Serializer.Deserialize<BPDefinitionInfoFilter>(serializedFilter) : null;
            return manager.GetBPDefinitionsInfo(filter);
        }

        [HttpGet]
        [Route("GetBPDefintion")]
        public BPDefinition GetBPDefintion(Guid bpDefinitionId)
        {
            return manager.GetBPDefinition(bpDefinitionId);
        }

        [HttpGet]
        [Route("GetDefinitions")]
        public IEnumerable<BPDefinition> GetDefinitions(string serializedFilter)
        {
            BPDefinitionInfoFilter filter = serializedFilter != null ? Vanrise.Common.Serializer.Deserialize<BPDefinitionInfoFilter>(serializedFilter) : null;
            return manager.GetBPDefinitions(filter);
        }
    }
}