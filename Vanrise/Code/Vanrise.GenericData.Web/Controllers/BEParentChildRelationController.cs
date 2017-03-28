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
    [RoutePrefix(Constants.ROUTE_PREFIX + "BEParentChildRelation")]
    [JSONWithTypeAttribute]
    public class BEParentChildRelationController : BaseAPIController
    {
        BEParentChildRelationManager _manager = new BEParentChildRelationManager();
        BEParentChildRelationDefinitionManager _defManager = new BEParentChildRelationDefinitionManager();

        [HttpPost]
        [Route("GetFilteredBEParentChildRelations")]
        public object GetFilteredBEParentChildRelations(Vanrise.Entities.DataRetrievalInput<BEParentChildRelationQuery> input)
        {
            if (!_defManager.DoesUserHaveViewAccess(input.Query.RelationDefinitionId))
                return GetUnauthorizedResponse();
            return GetWebResponse(input, _manager.GetFilteredBEParentChildRelations(input));
        }

        [HttpGet]
        [Route("GetBEParentChildRelation")]
        public BEParentChildRelation GetBEParentChildRelation(Guid beParentChildRelationDefinitionId, int beParentChildRelationId)
        {
            return _manager.GetBEParentChildRelation(beParentChildRelationDefinitionId, beParentChildRelationId);
        }
        [HttpGet]
        [Route("DoesUserHaveAddccess")]
        public bool DoesUserHaveAddccess(Guid beParentChildRelationDefinitionId)
        {
            return _defManager.DoesUserHaveAddAccess(beParentChildRelationDefinitionId);
        }
        [HttpPost]
        [Route("AddBEParentChildRelation")]
        public object AddBEParentChildRelation(BEParentChildRelation beParentChildRelationItem)
        {
            if (!DoesUserHaveAddccess(beParentChildRelationItem.RelationDefinitionId))
                return GetUnauthorizedResponse();

            return _manager.AddBEParentChildRelation(beParentChildRelationItem);
        }
        [HttpPost]
        [Route("UpdateBEParentChildRelation")]
        public object UpdateBEParentChildRelation(BEParentChildRelation beParentChildRelationItem)
        {
            if (!DoesUserHaveAddccess(beParentChildRelationItem.RelationDefinitionId))
                return GetUnauthorizedResponse();
            return _manager.UpdateBEParentChildRelation(beParentChildRelationItem);
        }

        //[HttpGet]
        //[Route("GetBEParentChildRelationesInfo")]
        //public IEnumerable<BEParentChildRelationInfo> GetBEParentChildRelationesInfo(string filter = null)
        //{
        //    BEParentChildRelationFilter deserializedFilter = (filter != null) ? Vanrise.Common.Serializer.Deserialize<BEParentChildRelationFilter>(filter) : null;
        //    return _manager.GetBEParentChildRelationesInfo(deserializedFilter);
        //}
    }
}