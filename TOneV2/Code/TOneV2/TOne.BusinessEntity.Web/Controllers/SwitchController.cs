using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TOne.BusinessEntity.Business;
using TOne.BusinessEntity.Entities;
namespace TOne.BusinessEntity.Web.Controllers
{
    public class SwitchController : ApiController
    {


        [HttpGet]
        public List<Switch> GetFilteredSwitches(string switchName, int rowFrom, int rowTo)
        {
            SwitchManager manager = new SwitchManager();
            return manager.GetFilteredSwitches(switchName, rowFrom, rowTo);
        }

        [HttpGet]
        public Switch GetSwitchDetails(int switchID)
        {
            System.Threading.Thread.Sleep(1000);
            SwitchManager manager = new SwitchManager();
            return manager.GetSwitchDetails(switchID);
        }

        [HttpPost]
        public TOne.Entities.UpdateOperationOutput<Switch> UpdateSwitch(Switch switchObject)
        {
            SwitchManager manager = new SwitchManager();
            return manager.UpdateSwitch(switchObject);
        }


        [HttpPost]
        public TOne.Entities.InsertOperationOutput<Switch> InsertSwitch(Switch switchObject)//UpdateOperationOutput
        {
            SwitchManager manager = new SwitchManager();
            return manager.InsertSwitch(switchObject);
        }
         [HttpGet]
        public string GetSwitchName(int switchId)
        {
            SwitchManager manager = new SwitchManager();
            return manager.GetSwitchName(switchId);
        }


    }
}