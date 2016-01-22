using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Security.Business;
using Vanrise.Security.Entities;
using Vanrise.Web.Base;

namespace Vanrise.Security.Web.Controllers
{
    [JSONWithTypeAttribute]
    [RoutePrefix(Constants.ROUTE_PREFIX + "Menu")]
    public class MenuController : Vanrise.Web.Base.BaseAPIController
    {
        MenuManager _manager;
        public MenuController()
        {
            _manager = new MenuManager();
        }

        [HttpGet]
        [Route("GetMenuItems")]
        public IEnumerable<MenuItem> GetMenuItems()
        {
            return _manager.GetMenuItems(SecurityContext.Current.GetLoggedInUserId());
        }

        [HttpGet]
        [Route("GetAllMenuItems")]
        public IEnumerable<MenuItem> GetAllMenuItems()
        {
            return _manager.GetMenuItems();
        }
    }
}