using System.Collections.Generic;
using System.Web.Http;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Entities;
using Vanrise.Web.Base;

namespace TOne.WhS.BusinessEntity.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "Switch")]
    public class WhSBE_SwitchController : BaseAPIController
    {
        [HttpPost]
        [Route("GetFilteredSwitches")]
        public object GetFilteredSwitches(Vanrise.Entities.DataRetrievalInput<SwitchQuery> input)
        {
            SwitchManager manager = new SwitchManager();
            return GetWebResponse(input, manager.GetFilteredSwitches(input));
        }

        [HttpGet]
        [Route("GetSwitch")]
        public Switch GetSwitch(int switchId)
        {
            SwitchManager manager = new SwitchManager();
            return manager.GetSwitch(switchId);
        }

        [HttpGet]
        [Route("GetSwitchesInfo")]
        public IEnumerable<SwitchInfo> GetSwitchesInfo()
        {
            SwitchManager manager = new SwitchManager();
            return manager.GetSwitchesInfo();
        }

        [HttpPost]
        [Route("AddSwitch")]
        public InsertOperationOutput<SwitchDetail> AddSwitch(Switch whsSwitch)
        {
            SwitchManager manager = new SwitchManager();
            return manager.AddSwitch(whsSwitch);
        }
        [HttpPost]
        [Route("UpdateSwitch")]
        public UpdateOperationOutput<SwitchDetail> UpdateSwitch(Switch whsSwitch)
        {
            SwitchManager manager = new SwitchManager();
            return manager.UpdateSwitch(whsSwitch);
        }


        [HttpGet]
        [Route("DeleteSwitch")]
        public DeleteOperationOutput<SwitchDetail> DeleteSwitch(int switchId)
        {

            SwitchManager manager = new SwitchManager();
            return manager.DeleteSwitch(switchId);
        }

    }
}