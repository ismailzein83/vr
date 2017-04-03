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
        BEParentChildRelationManager _parentChildRelationManager = new BEParentChildRelationManager();
        BEParentChildRelationDefinitionManager _parentChildRelationDefinitionManager = new BEParentChildRelationDefinitionManager();

        [HttpPost]
        [Route("GetFilteredBEParentChildRelations")]
        public object GetFilteredBEParentChildRelations(Vanrise.Entities.DataRetrievalInput<BEParentChildRelationQuery> input)
        {
            if (!_parentChildRelationDefinitionManager.DoesUserHaveViewAccess(input.Query.RelationDefinitionId))
                return GetUnauthorizedResponse();

            return GetWebResponse(input, _parentChildRelationManager.GetFilteredBEParentChildRelations(input));
        }

        [HttpGet]
        [Route("GetBEParentChildRelation")]
        public BEParentChildRelation GetBEParentChildRelation(Guid beParentChildRelationDefinitionId, int beParentChildRelationId)
        {
            return _parentChildRelationManager.GetBEParentChildRelation(beParentChildRelationDefinitionId, beParentChildRelationId);
        }

        [HttpGet]
        [Route("DoesUserHaveAddAccess")]
        public bool DoesUserHaveAddAccess(Guid beParentChildRelationDefinitionId)
        {
            return _parentChildRelationDefinitionManager.DoesUserHaveAddAccess(beParentChildRelationDefinitionId);
        }

        [HttpPost]
        [Route("AddBEParentChildRelation")]
        public object AddBEParentChildRelation(BEParentChildRelation beParentChildRelationItem)
        {
            if (!DoesUserHaveAddAccess(beParentChildRelationItem.RelationDefinitionId))
                return GetUnauthorizedResponse();

            return _parentChildRelationManager.AddBEParentChildRelation(beParentChildRelationItem);
        }

        [HttpPost]
        [Route("UpdateBEParentChildRelation")]
        public object UpdateBEParentChildRelation(BEParentChildRelation beParentChildRelationItem)
        {
            if (!DoesUserHaveAddAccess(beParentChildRelationItem.RelationDefinitionId))
                return GetUnauthorizedResponse();

            return _parentChildRelationManager.UpdateBEParentChildRelation(beParentChildRelationItem);
        }

        [HttpGet]
        [Route("GetLastAssignedEED")]
        public DateTime? GetLastAssignedEED(Guid beParentChildRelationDefinitionId, string beChildId)
        {
            return _parentChildRelationManager.GetLastAssignedEED(beParentChildRelationDefinitionId, beChildId);
        }

    }
}