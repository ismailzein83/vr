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
        public SwitchDetail GetSwitchByID(int switchID)
        {
            SwitchManager manager = new SwitchManager();
            return manager.GetSwitchByID(switchID);
        }

        [HttpGet]
        public List<SwitchInfo> GetSwitches()
        {
            SwitchManager manager = new SwitchManager();
            return manager.GetSwitches();
        }

        [HttpGet]
        public List<SwitchInfo> GetSwitchesToLinkTo(int switchID)
        {
            SwitchManager manager = new SwitchManager();
            return manager.GetSwitchesToLinkTo(switchID);
        }

        [HttpGet]
        public List<SwitchAssignedDataSource> GetSwitchAssignedDataSources()
        {
            SwitchManager manager = new SwitchManager();
            return manager.GetSwitchAssignedDataSources();
        }

        [HttpPost]
        public UpdateOperationOutput<SwitchDetail> UpdateSwitch(Switch switchObject)
        {
            SwitchManager manager = new SwitchManager();
            return manager.UpdateSwitch(switchObject);
        }

        [HttpPost]
        public InsertOperationOutput<SwitchDetail> AddSwitch(Switch switchObject)
        {
            SwitchManager manager = new SwitchManager();
            return manager.AddSwitch(switchObject);
        }

        [HttpGet]
        public DeleteOperationOutput<object> DeleteSwitch(int switchID)
        {
            SwitchManager manager = new SwitchManager();
            return manager.DeleteSwitch(switchID);
        }
    }
}