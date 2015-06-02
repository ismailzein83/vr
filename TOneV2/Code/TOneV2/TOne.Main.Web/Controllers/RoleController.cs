using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TOne.Business;
using TOne.Entities;

namespace TOne.Main.Web.Controllers
{
    public class RoleController : ApiController
    {
        [HttpGet]
        public IEnumerable<Role> GetFilteredRoles(int fromRow, int toRow, string name)
        {
            RoleManager manager = new RoleManager();
            return ((IEnumerable<Role>)(manager.GetFilteredRoles(fromRow, toRow, name)));
        }

        [HttpGet]
        public Role GetRole(int roleId)
        {
            RoleManager manager = new RoleManager();
            return ((Role)(manager.GetRole(roleId)));
        }

        [HttpPost]
        public TOne.Entities.UpdateOperationOutput<Role> UpdateRole(Role roleObject)
        {
            RoleManager manager = new RoleManager();
            return manager.UpdateRole(roleObject);
        }

        [HttpPost]
        public TOne.Entities.InsertOperationOutput<Role> AddRole(Role roleObject)
        {
            RoleManager manager = new RoleManager();
            return manager.AddRole(roleObject);
        }

        [HttpGet]
        public void DeleteRole(int id)
        {
            RoleManager manager = new RoleManager();
            manager.DeleteRole(id);
        }
    }
}