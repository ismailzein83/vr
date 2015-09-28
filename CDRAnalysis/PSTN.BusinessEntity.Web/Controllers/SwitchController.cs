﻿using PSTN.BusinessEntity.Business;
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

        [HttpGet]
        public List<Switch> GetSwitchesToLinkTo(int switchID)
        {
            SwitchManager manager = new SwitchManager();
            return manager.GetSwitchesToLinkTo(switchID);
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

        [HttpGet]
        public DeleteOperationOutput<object> DeleteSwitch(int switchID)
        {
            SwitchManager manager = new SwitchManager();
            return manager.DeleteSwitch(switchID);
        }
    }
}