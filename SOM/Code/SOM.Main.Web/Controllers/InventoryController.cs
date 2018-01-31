using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using SOM.Main.Business;
using SOM.Main.Entities;
using Vanrise.Web.Base;

namespace SOM.Main.Web.Controllers
{
    [JSONWithTypeAttribute]
    [RoutePrefix(Constants.ROUTE_PREFIX + "Inventory")]
    public class InventoryController : BaseAPIController
    {
        [HttpGet]
        [Route("GetInventoryPhoneItem")]
        public InventoryPhoneItem GetInventoryPhoneItem(string phoneNumber)
        {
            InventoryManager manager = new InventoryManager();
            return manager.GetInventoryPhoneItem(phoneNumber);
        }

        [HttpGet]
        [Route("GetAvailableNumbers")]
        public List<PhoneNumber> GetAvailableNumbers(string cabinetPort, string dpPort, bool isGold, bool isISDN, string startsWith)
        {
            InventoryManager manager = new InventoryManager();
            return manager.GetAvailableNumbers(cabinetPort, dpPort, isGold, isISDN, startsWith);
        }
    }
}