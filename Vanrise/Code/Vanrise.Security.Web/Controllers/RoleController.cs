using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Security.Business;
using Vanrise.Security.Entities;

namespace Vanrise.Security.Web.Controllers
{
    public class RolesController : Vanrise.Web.Base.BaseAPIController
    {
        [HttpGet]
        public IEnumerable<Role> GetFilteredRoles(int fromRow, int toRow, string name)
        {
            RoleManager manager = new RoleManager();
            return manager.GetFilteredRoles(fromRow, toRow, name);
        }

        [HttpGet]
        public Role GetRole(int roleId)
        {
            RoleManager manager = new RoleManager();
            return manager.GetRole(roleId);
        }

        [HttpPost]
        public Vanrise.Security.Entities.InsertOperationOutput<Role> AddRole(Role roleObject)
        {
            RoleManager manager = new RoleManager();
            return manager.AddRole(roleObject);
        }

        [HttpPost]
        public Vanrise.Security.Entities.UpdateOperationOutput<Role> UpdateRole(Role roleObject)
        {
            RoleManager manager = new RoleManager();
            return manager.UpdateRole(roleObject);
        }

    }
}