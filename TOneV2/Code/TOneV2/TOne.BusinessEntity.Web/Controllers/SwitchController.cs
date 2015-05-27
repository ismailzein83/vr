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
        public List<Switch> getFilteredSwitches(string switchName, int rowFrom, int rowTo)
        {
            SwitchManager manager = new SwitchManager();
            return manager.GetFilteredSwitches(switchName, rowFrom, rowTo);
        }

        [HttpGet]
        public Switch getSwitchDetails(int switchID)
        {
            System.Threading.Thread.Sleep(2000);
            SwitchManager manager = new SwitchManager();
            return manager.GetSwitchDetails(switchID);
        }

        [HttpPost]
        public int updateSwitch(Switch switchObject)
        {
            SwitchManager manager = new SwitchManager();
            return manager.UpdateSwitch(switchObject);
        }


        [HttpPost]
        public int insertSwitch(Switch switchObject)//UpdateOperationOutput
        {
            SwitchManager manager = new SwitchManager();
            return manager.InsertSwitch(switchObject);
        }


    }
}