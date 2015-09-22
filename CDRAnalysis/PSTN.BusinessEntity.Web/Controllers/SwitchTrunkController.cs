using PSTN.BusinessEntity.Business;
using PSTN.BusinessEntity.Entities;
using System.Web.Http;
using Vanrise.Entities;

namespace PSTN.BusinessEntity.Web.Controllers
{
    public class SwitchTrunkController : Vanrise.Web.Base.BaseAPIController
    {
        [HttpPost]
        public object GetFilteredSwitchTrunks(Vanrise.Entities.DataRetrievalInput<SwitchTrunkDetailQuery> input)
        {
            SwitchTrunkManager manager = new SwitchTrunkManager();
            return GetWebResponse(input, manager.GetFilteredSwitchTrunks(input));
        }

        [HttpGet]
        public SwitchTrunkDetail GetSwitchTrunkByID(int trunkID)
        {
            SwitchTrunkManager manager = new SwitchTrunkManager();
            return manager.GetSwitchTrunkByID(trunkID);
        }

        [HttpPost]
        public InsertOperationOutput<SwitchTrunkDetail> AddSwitchTrunk(SwitchTrunk trunkObject)
        {
            SwitchTrunkManager manager = new SwitchTrunkManager();
            return manager.AddSwitchTrunk(trunkObject);
        }

        [HttpPost]
        public UpdateOperationOutput<SwitchTrunkDetail> UpdateSwitchTrunk(SwitchTrunk trunkObject)
        {
            SwitchTrunkManager manager = new SwitchTrunkManager();
            return manager.UpdateSwitchTrunk(trunkObject);
        }
    }
}