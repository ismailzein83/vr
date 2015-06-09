using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace Vanrise.Security.Web.Controllers
{
    public class SecurityController : Vanrise.Web.Base.BaseAPIController
    {
        [HttpGet]
        public string Authenticate(string userName, string password)
        {
            //PermissionManager manager = new PermissionManager();
            //return manager.GetEntityNodes();

            return "";
        }
    }
}