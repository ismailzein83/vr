using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Vanrise.Security.Business;
using Vanrise.Security.Entities;

namespace Vanrise.Security.Web.Controllers
{
    public class RolesController : Vanrise.Web.Base.BaseAPIController
    {
        public List<Role> GetRoles()
        {
            RoleManager manager = new RoleManager();
            return manager.GetRoles();
        }
    }
}