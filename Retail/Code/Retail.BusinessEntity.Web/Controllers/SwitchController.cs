using Retail.BusinessEntity.Business;
using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Entities;
using Vanrise.Web.Base;

namespace Retail.BusinessEntity.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "Switch")]
    [JSONWithTypeAttribute]
    public class SwitchController : BaseAPIController
    {
        SwitchManager _manager = new SwitchManager();

        [HttpGet]
        [Route("GetSwitchSettingsTemplateConfigs")]
        public IEnumerable<SwitchIntegrationConfig> GetSwitchSettingsTemplateConfigs()
        {
            return _manager.GetSwitchSettingsTemplateConfigs();
        }

        [HttpPost]
        [Route("AddSwitch")]
        public Vanrise.Entities.InsertOperationOutput<SwitchDetail> AddSwitch(Switch switchItem)
        {
            return _manager.AddSwitch(switchItem);
        }

        [HttpPost]
        [Route("UpdateSwitch")]
        public Vanrise.Entities.UpdateOperationOutput<SwitchDetail> UpdateSwitch(Switch switchItem)
        {
            return _manager.UpdateSwitch(switchItem);
        }

        [HttpPost]
        [Route("GetFilteredSwitches")]
        public object GetFilteredSwitches(Vanrise.Entities.DataRetrievalInput<SwitchQuery> input)
        {
            return GetWebResponse(input, _manager.GetFilteredSwitches(input), "Switches");
        }

        [HttpGet]
        [Route("GetSwitch")]
        public Switch GetSwitch(int switchId)
        {
            return _manager.GetSwitch(switchId);
        }
    }
}