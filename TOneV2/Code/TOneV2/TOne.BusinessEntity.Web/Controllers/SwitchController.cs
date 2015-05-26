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
            SwitchManager manager = new SwitchManager();
            return manager.GetSwitchDetails(switchID);
        }

        [HttpGet]
        public int UpdateSwitch(Switch switchObject)
        {
            System.Threading.Thread.Sleep(1000);
            SwitchManager manager = new SwitchManager();
            return manager.UpdateSwitch(switchObject);
            // return Mappers.MapSwitch(manager.UpdateSwitch(switchObject));
        }


        [HttpPost]
        public int InsertSwitch(Switch switchObject)
        {
            System.Threading.Thread.Sleep(1000);
            SwitchManager manager = new SwitchManager();
            return manager.InsertSwitch(switchObject);
            // return Mappers.MapSwitch(manager.UpdateSwitch(switchObject));
        }

        //[HttpPost]
        //public List<Switch> AddSwitch(Switch newswitch)
        //{
        //    SwitchManager manager = new SwitchManager();
        //    return manager.GetFilteredSwitches(switchName, rowFrom, rowTo);
        //}
    }
}