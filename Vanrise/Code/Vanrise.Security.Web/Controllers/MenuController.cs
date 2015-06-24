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
            MenuManager manager = new MenuManager();
            List<MenuItem> result = new List<MenuItem>();

            if(Request.Headers.Contains("Auth-Token"))
            {
                //TODO: the logic of reading the token needs to be moved to the BaseAPIController
                result = manager.GetMenuItems(Request.Headers.GetValues("Auth-Token").First());
            }
            
            return result;
        }
        [HttpGet]
        public IEnumerable<MenuItem> GetAllMenuItems()
        {
            MenuManager manager = new MenuManager();
            List<MenuItem> result = new List<MenuItem>();
            result = manager.GetMenuItems();


            return result;
        }
    }
}