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
    public class SwitchManagementController : ApiController
    {


        [HttpGet]
        public List<SwitchInfo> GetFilteredSwitches(string switchName, int rowFrom, int rowTo)
        {
            SwitchManager manager = new SwitchManager();
            return manager.GetFilteredSwitches(switchName, rowFrom, rowTo);
            //List<SwitchInfo> allSwitchs = manager.GetFilteredSwitches(switchName, rowFrom, rowTo);
            //if (allSwitchs.Count > 0)
            //    return allSwitchs.Where(s => s.Name.StartsWith(switchName)).ToList();
            //return new List<SwitchInfo>();
        }
    }
}