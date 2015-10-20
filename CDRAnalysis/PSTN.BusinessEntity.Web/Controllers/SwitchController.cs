using PSTN.BusinessEntity.Business;
using PSTN.BusinessEntity.Entities;
using System.Collections.Generic;
using System.Web.Http;
using Vanrise.Entities;

namespace PSTN.BusinessEntity.Web.Controllers
{
    public class SwitchController : Vanrise.Web.Base.BaseAPIController
    {
        [HttpPost]
        public object GetFilteredSwitches(Vanrise.Entities.DataRetrievalInput<SwitchQuery> input)
        {
            SwitchManager manager = new SwitchManager();
            return GetWebResponse(input, manager.GetFilteredSwitches(input));
        }

        [HttpGet]
        public Switch GetSwitchById(int switchId)
        {
            SwitchManager manager = new SwitchManager();
            return manager.GetSwitchById(switchId);
        }

        [HttpGet]
        public IEnumerable<SwitchInfo> GetSwitches()
        {
            SwitchManager manager = new SwitchManager();
            return manager.GetAllSwitches();
        }

        [HttpGet]
        public IEnumerable<SwitchInfo> GetSwitchesToLinkTo(int switchId)
        {
            SwitchManager manager = new SwitchManager();
            return manager.GetSwitchesToLinkTo(switchId);
        }

        [HttpGet]
        public IEnumerable<int> GetSwitchAssignedDataSources()
        {
            SwitchManager manager = new SwitchManager();
            return manager.GetSwitchAssignedDataSources();
        }

        [HttpPost]
        public UpdateOperationOutput<SwitchDetail> UpdateSwitch(Switch switchObj)
        {
            SwitchManager manager = new SwitchManager();
            return manager.UpdateSwitch(switchObj);
        }

        [HttpPost]
        public InsertOperationOutput<SwitchDetail> AddSwitch(Switch switchObj)
        {
            SwitchManager manager = new SwitchManager();
            return manager.AddSwitch(switchObj);
        }

        [HttpGet]
        public DeleteOperationOutput<object> DeleteSwitch(int switchId)
        {
            SwitchManager manager = new SwitchManager();
            return manager.DeleteSwitch(switchId);
        }
    }
}