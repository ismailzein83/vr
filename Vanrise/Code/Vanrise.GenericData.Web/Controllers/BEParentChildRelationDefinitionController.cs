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
    [RoutePrefix(Constants.ROUTE_PREFIX + "BEParentChildRelationDefinition")]
    [JSONWithTypeAttribute]
    public class BEParentChildRelationDefinitionController : BaseAPIController
    {
        [HttpGet]
        [Route("GetBEParentChildRelationDefinitionsInfo")]
        public IEnumerable<BEParentChildRelationDefinitionInfo> GetBEParentChildRelationDefinitionsInfo(string serializedFilter = null)
        {
            BEParentChildRelationDefinitionManager manager = new BEParentChildRelationDefinitionManager();
            return manager.GetBEParentChildRelationDefinitionsInfo();
        }

        [HttpGet]
        [Route("GetBEParentChildRelationGridColumnNames")]
        public IEnumerable<string> GetBEParentChildRelationGridColumnNames(Guid beParentChildRelationDefinitionId)
        {
            BEParentChildRelationDefinitionManager manager = new BEParentChildRelationDefinitionManager();
            return manager.GetBEParentChildRelationGridColumnNames(beParentChildRelationDefinitionId);
        }

        [HttpGet]
        [Route("GetBEParentChildRelationDefinition")]
        public BEParentChildRelationDefinition GetBEParentChildRelationDefinition(Guid beParentChildRelationDefinitionId)
        {
            BEParentChildRelationDefinitionManager manager = new BEParentChildRelationDefinitionManager();
            return manager.GetBEParentChildRelationDefinition(beParentChildRelationDefinitionId);
        }
    }
}