using PSTN.BusinessEntity.Business;
using PSTN.BusinessEntity.Entities;
using System.Collections.Generic;
using System.Web.Http;
using Vanrise.Entities;

namespace PSTN.BusinessEntity.Web.Controllers
{
    public class SwitchController : Vanrise.Web.Base.BaseAPIController
    {
        [HttpGet]
        public List<SwitchType> GetSwitchTypes()
        {
            SwitchManager manager = new SwitchManager();
            return manager.GetSwitchTypes();
        }

        [HttpPost]
        public object GetFilteredSwitches(Vanrise.Entities.DataRetrievalInput<SwitchQuery> input)
        {
            SwitchManager manager = new SwitchManager();
            return GetWebResponse(input, manager.GetFilteredSwitches(input));
        }

        [HttpGet]
        public Switch GetSwitchByID(int switchID)
        {
            SwitchManager manager = new SwitchManager();
            return manager.GetSwitchByID(switchID);
        }

        [HttpGet]
        public List<Switch> GetSwitches()
        {
            SwitchManager manager = new SwitchManager();
            return manager.GetSwitches();
        }

        [HttpPost]
        public UpdateOperationOutput<Switch> UpdateSwitch(Switch switchObject)
        {
            SwitchManager manager = new SwitchManager();
            return manager.UpdateSwitch(switchObject);
        }

        [HttpPost]
        public InsertOperationOutput<Switch> AddSwitch(Switch switchObject)
        {
            SwitchManager manager = new SwitchManager();
            return manager.AddSwitch(switchObject);
        }
    }
}