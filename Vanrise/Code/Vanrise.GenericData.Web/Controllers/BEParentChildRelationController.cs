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

        [HttpPost]
        [Route("GetFilteredBEParentChildRelations")]
        public object GetFilteredBEParentChildRelations(Vanrise.Entities.DataRetrievalInput<BEParentChildRelationQuery> input)
        {
            return GetWebResponse(input, _manager.GetFilteredBEParentChildRelations(input));
        }

        [HttpGet]
        [Route("GetBEParentChildRelation")]
        public BEParentChildRelation GetBEParentChildRelation(int beParentChildRelationId)
        {
            return _manager.GetBEParentChildRelation(beParentChildRelationId);
        }

        [HttpPost]
        [Route("AddBEParentChildRelation")]
        public Vanrise.Entities.InsertOperationOutput<BEParentChildRelationDetail> AddBEParentChildRelation(BEParentChildRelation beParentChildRelationItem)
        {
            return _manager.AddBEParentChildRelation(beParentChildRelationItem);
        }

        [HttpPost]
        [Route("UpdateBEParentChildRelation")]
        public Vanrise.Entities.UpdateOperationOutput<BEParentChildRelationDetail> UpdateBEParentChildRelation(BEParentChildRelation beParentChildRelationItem)
        {
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