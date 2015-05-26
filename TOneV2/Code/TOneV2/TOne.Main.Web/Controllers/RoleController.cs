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
        public IEnumerable<Role> GetRoles(int fromRow, int toRow)
        {
            RoleManager manager = new RoleManager();
            //return ((IEnumerable<Role>)(manager.GetRoles())).Skip(pageNumber * pageSize).Take(pageSize);
            return ((IEnumerable<Role>)(manager.GetRoles(fromRow, toRow)));
        }

        [HttpGet]
        public void DeleteRole(int Id)
        {
            RoleManager manager = new RoleManager();
            manager.DeleteRole(Id);
        }

        [HttpPost]
        public bool UpdateRole(Role Role)
        {
            RoleManager manager = new RoleManager();
            return manager.UpdateRole(Role);
        }

        [HttpPost]
        public Role AddRole(Role Role)
        {
            RoleManager manager = new RoleManager();
            return manager.AddRole(Role);
        }


        [HttpGet]
        public List<Role> SearchRole(string Name)
        {
            RoleManager manager = new RoleManager();
            return manager.SearchRole(Name);
        }

      
    }
}