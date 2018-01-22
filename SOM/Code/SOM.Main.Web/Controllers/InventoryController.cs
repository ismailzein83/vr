using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using SOM.Main.Business;
using Vanrise.Web.Base;

namespace SOM.Main.Web.Controllers
{
    [JSONWithTypeAttribute]
    [RoutePrefix(Constants.ROUTE_PREFIX + "Inventory")]
    public class InventoryController : BaseAPIController
    {
        [HttpGet]
        [Route("GetInventoryDetail")]
        public InventoryItem GetInventoryDetail(string phoneNumber)
        {
            InventoryManager manager = new InventoryManager();
            return manager.GetInventoryItem(phoneNumber);
        }
    }
}