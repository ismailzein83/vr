using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Security.Business;
using Vanrise.Security.Entities;

namespace Vanrise.Security.Web.Controllers
{
    public class MenuController : Vanrise.Web.Base.BaseAPIController
    {
        [HttpGet]
        public IEnumerable<MenuItem> GetMenuItems()
        {
            //TODO: handle the exception Key Not found in case the auth-toekn was null
            MenuManager manager = new MenuManager();
            return manager.GetMenuItems(SecurityContext.GetSecurityToken());
        }
        
        [HttpGet]
        public IEnumerable<MenuItem> GetAllMenuItems()
        {
            MenuManager manager = new MenuManager();
            return manager.GetMenuItems();
        }
    }
}