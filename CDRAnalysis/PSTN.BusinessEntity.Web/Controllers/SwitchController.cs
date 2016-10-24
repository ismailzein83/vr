using PSTN.BusinessEntity.Business;
using PSTN.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Web.Http;
using Vanrise.Entities;
using Vanrise.Web.Base;

namespace PSTN.BusinessEntity.Web.Controllers
{
    [RoutePrefix(Constants.ROUTE_PREFIX + "Switch")]
    [JSONWithTypeAttribute]
    public class SwitchController : Vanrise.Web.Base.BaseAPIController
    {
        [HttpPost]
        [Route("GetFilteredSwitches")]
        public object GetFilteredSwitches(Vanrise.Entities.DataRetrievalInput<SwitchQuery> input)
        {
            SwitchManager manager = new SwitchManager();
            return GetWebResponse(input, manager.GetFilteredSwitches(input));
        }

        [HttpGet]
        [Route("GetSwitchById")]
        public Switch GetSwitchById(int switchId)
        {
            SwitchManager manager = new SwitchManager();
            return manager.GetSwitchById(switchId);
        }

        [HttpGet]
        [Route("GetSwitches")]
        public IEnumerable<SwitchInfo> GetSwitches()
        {
            SwitchManager manager = new SwitchManager();
            return manager.GetAllSwitches();
        }
        [HttpGet]
        [Route("GetSwitchesInfo")]
        public IEnumerable<SwitchInfo> GetSwitchesInfo(string serializedFilter)
        {
            SwitchFilter filter = serializedFilter != null ? Vanrise.Common.Serializer.Deserialize<SwitchFilter>(serializedFilter) : null;
            SwitchManager manager = new SwitchManager();
            return manager.GetSwitchesInfo(filter);
           
        }
        [HttpGet]
        [Route("GetSwitchAssignedDataSources")]
        public IEnumerable<Guid> GetSwitchAssignedDataSources()
        {
            SwitchManager manager = new SwitchManager();
            return manager.GetSwitchAssignedDataSources();
        }

        [HttpPost]
        [Route("UpdateSwitch")]
        public UpdateOperationOutput<SwitchDetail> UpdateSwitch(Switch switchObj)
        {
            SwitchManager manager = new SwitchManager();
            return manager.UpdateSwitch(switchObj);
        }

        [HttpPost]
        [Route("AddSwitch")]
        public InsertOperationOutput<SwitchDetail> AddSwitch(Switch switchObj)
        {
            SwitchManager manager = new SwitchManager();
            return manager.AddSwitch(switchObj);
        }

        [HttpGet]
        [Route("DeleteSwitch")]
        public DeleteOperationOutput<object> DeleteSwitch(int switchId)
        {
            SwitchManager manager = new SwitchManager();
            return manager.DeleteSwitch(switchId);
        }
    }
}