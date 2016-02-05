using PSTN.BusinessEntity.Business;
using PSTN.BusinessEntity.Entities;
using System.Collections.Generic;
using System.Web.Http;
using Vanrise.Entities;
using Vanrise.Web.Base;

namespace PSTN.BusinessEntity.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "Trunk")]
    [JSONWithTypeAttribute]
    public class TrunkController : Vanrise.Web.Base.BaseAPIController
    {
        [HttpPost]
        [Route("GetFilteredTrunks")]
        public object GetFilteredTrunks(Vanrise.Entities.DataRetrievalInput<TrunkQuery> input)
        {
            TrunkManager manager = new TrunkManager();
            return GetWebResponse(input, manager.GetFilteredTrunks(input));
        }


        [HttpGet]
        [Route("GetTrunkById")]
        public Trunk GetTrunkById(int trunkId)
        {
            TrunkManager manager = new TrunkManager();
            return manager.GetTrunkById(trunkId);
        }

        [HttpGet]
        [Route("GetTrunksInfo")]
        public IEnumerable<TrunkInfo> GetTrunksInfo(string serializedFilter)
        {
            TrunkFilter filter = serializedFilter != null ? Vanrise.Common.Serializer.Deserialize<TrunkFilter>(serializedFilter) : null;
            TrunkManager manager = new TrunkManager();
            return manager.GetTrunksInfo(filter);
        }

        [HttpPost]
        [Route("GetTrunksBySwitchIds")]
        public IEnumerable<TrunkInfo> GetTrunksBySwitchIds(TrunkFilter trunkFilterObj)
        {
            TrunkManager manager = new TrunkManager();
            return manager.GetTrunksBySwitchIds(trunkFilterObj);
        }

        [HttpGet]
        [Route("GetTrunks")]
        public IEnumerable<TrunkInfo> GetTrunks()
        {
            TrunkManager manager = new TrunkManager();
            return manager.GetTrunks();
        }

        [HttpPost]
        [Route("AddTrunk")]
        public InsertOperationOutput<TrunkDetail> AddTrunk(Trunk trunkObj)
        {
            TrunkManager manager = new TrunkManager();
            return manager.AddTrunk(trunkObj);
        }

        [HttpPost]
        [Route("UpdateTrunk")]
        public UpdateOperationOutput<TrunkDetail> UpdateTrunk(Trunk trunkObj)
        {
            TrunkManager manager = new TrunkManager();
            return manager.UpdateTrunk(trunkObj);
        }

        [HttpGet]
        [Route("DeleteTrunk")]
        public DeleteOperationOutput<object> DeleteTrunk(int trunkId, int? linkedToTrunkId)
        {
            TrunkManager manager = new TrunkManager();
            return manager.DeleteTrunk(trunkId, linkedToTrunkId);
        }
    }
}