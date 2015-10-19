using PSTN.BusinessEntity.Business;
using PSTN.BusinessEntity.Entities;
using System.Collections.Generic;
using System.Web.Http;
using Vanrise.Entities;

namespace PSTN.BusinessEntity.Web.Controllers
{
    public class TrunkController : Vanrise.Web.Base.BaseAPIController
    {
        [HttpPost]
        public object GetFilteredTrunks(Vanrise.Entities.DataRetrievalInput<TrunkQuery> input)
        {
            TrunkManager manager = new TrunkManager();
            return GetWebResponse(input, manager.GetFilteredTrunks(input));
        }

        [HttpGet]
        public TrunkDetail GetTrunkById(int trunkId)
        {
            TrunkManager manager = new TrunkManager();
            return manager.GetTrunkDetialById(trunkId);
        }

        [HttpPost]
        public IEnumerable<TrunkInfo> GetTrunksBySwitchIds(TrunkFilter trunkFilterObj)
        {
            TrunkManager manager = new TrunkManager();
            return manager.GetTrunksBySwitchIds(trunkFilterObj);
        }

        [HttpGet]
        public IEnumerable<TrunkInfo> GetTrunks()
        {
            TrunkManager manager = new TrunkManager();
            return manager.GetTrunks();
        }

        [HttpPost]
        public InsertOperationOutput<TrunkDetail> AddTrunk(Trunk trunkObj)
        {
            TrunkManager manager = new TrunkManager();
            return manager.AddTrunk(trunkObj);
        }

        [HttpPost]
        public UpdateOperationOutput<TrunkDetail> UpdateTrunk(Trunk trunkObj)
        {
            TrunkManager manager = new TrunkManager();
            return manager.UpdateTrunk(trunkObj);
        }

        [HttpGet]
        public DeleteOperationOutput<object> DeleteTrunk(int trunkId, int? linkedToTrunkId)
        {
            TrunkManager manager = new TrunkManager();
            return manager.DeleteTrunk(trunkId, linkedToTrunkId);
        }
    }
}