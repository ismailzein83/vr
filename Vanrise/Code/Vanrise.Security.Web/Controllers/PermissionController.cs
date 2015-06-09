using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Security.Business;
using Vanrise.Security.Entities;

namespace Vanrise.Security.Web.Controllers
{
    public class PermissionController : Vanrise.Web.Base.BaseAPIController
    {
        [HttpGet]
        public IEnumerable<BusinessEntityNode> GetEntityNodes()
        {
            PermissionManager manager = new PermissionManager();
            return manager.GetEntityNodes();
        }

        [HttpGet]
        public IEnumerable<Permission> GetPermissions(int holderType, string holderId)
        {
            PermissionManager manager = new PermissionManager();
            return manager.GetPermissions(holderType, holderId);
        }

        [HttpPost]
        public Vanrise.Entities.UpdateOperationOutput<object> UpdatePermissions(Permission [] permissionObject)
        {
            PermissionManager manager = new PermissionManager();
            return manager.UpdatePermissions(permissionObject);
        }
    }
}