using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Entities;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;
using Vanrise.Web.Base;

namespace Vanrise.GenericData.Web.Controllers
{
    [JSONWithTypeAttribute]
    [RoutePrefix(Constants.ROUTE_PREFIX + "BELookupRuleDefinition")]
    public class BELookupRuleDefinitionController : BaseAPIController
    {
        BELookupRuleDefinitionManager _manager = new BELookupRuleDefinitionManager();

        [HttpPost]
        [Route("GetFilteredBELookupRuleDefinitions")]
        public object GetFilteredBELookupRuleDefinitions(DataRetrievalInput<BELookupRuleDefinitionQuery> input)
        {
            return GetWebResponse(input, _manager.GetFilteredBELookupRuleDefinitions(input));
        }

        [HttpGet]
        [Route("GetBELookupRuleDefinitionsInfo")]
        public IEnumerable<BELookupRuleDefinitionInfo> GetBELookupRuleDefinitionsInfo(string filter = null)
        {
            BELookupRuleDefinitionFilter deserializedFilter = filter != null ? Vanrise.Common.Serializer.Deserialize<BELookupRuleDefinitionFilter>(filter) : null;
            return _manager.GetBELookupRuleDefinitionsInfo(deserializedFilter);
        }

        [HttpGet]
        [Route("GetBELookupRuleDefinition")]
        public BELookupRuleDefinition GetBELookupRuleDefinition(Guid beLookupRuleDefinitionId)
        {
            return _manager.GetBELookupRuleDefinition(beLookupRuleDefinitionId);
        }

        [HttpPost]
        [Route("AddBELookupRuleDefinition")]
        public Vanrise.Entities.InsertOperationOutput<BELookupRuleDefinitionDetail> AddBELookupRuleDefinition(BELookupRuleDefinition beLookupRuleDefinition)
        {
            return _manager.AddBELookupRuleDefinition(beLookupRuleDefinition);
        }

        [HttpPost]
        [Route("UpdateBELookupRuleDefinition")]
        public Vanrise.Entities.UpdateOperationOutput<BELookupRuleDefinitionDetail> UpdateBELookupRuleDefinition(BELookupRuleDefinition beLookupRuleDefinition)
        {
            return _manager.UpdateBELookupRuleDefinition(beLookupRuleDefinition);
        }
    }
}