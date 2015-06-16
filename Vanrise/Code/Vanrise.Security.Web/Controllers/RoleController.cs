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

        [HttpGet]
        public List<Role> GetRoles()
        {
            RoleManager manager = new RoleManager();
            return manager.GetRoles();
        }

        [HttpPost]
        public Vanrise.Entities.InsertOperationOutput<Role> AddRole(RoleEditorInput roleObject)
        {
            RoleManager manager = new RoleManager();
            Role role = new Role() 
            { 
                Name = roleObject.Name,
                Description = roleObject.Description
            };

            return manager.AddRole(role, roleObject.Members);
        }

        [HttpPost]
        public Vanrise.Entities.UpdateOperationOutput<Role> UpdateRole(RoleEditorInput roleObject)
        {
            RoleManager manager = new RoleManager();
            Role role = new Role()
            {
                RoleId = roleObject.RoleId,
                Name = roleObject.Name,
                Description = roleObject.Description
            };
            return manager.UpdateRole(roleObject, roleObject.Members);
        }

    }

    public class RoleEditorInput : Role
    {
        public int[] Members { get; set; }
    }

}