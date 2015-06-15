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

        [HttpGet]
        public List<PermissionResult> GetEffectivePermissions(string token)
        {
            PermissionManager manager = new PermissionManager();
            Dictionary<string, Dictionary<string, Flag>> effectivePermissions = manager.GetEffectivePermissions(token);

            List<PermissionResult> result = new List<PermissionResult>();

            foreach (KeyValuePair<string, Dictionary<string, Flag>> permKvp in effectivePermissions)
            {
                PermissionResult permission = new PermissionResult() { PermissionPath = permKvp.Key };
                permission.PermissionFlags = new List<PermissionFlag>();

                foreach (KeyValuePair<string, Flag> flagKvp in permKvp.Value)
                {
                    permission.PermissionFlags.Add(new PermissionFlag() { FlagName = flagKvp.Key, FlagValue = flagKvp.Value });
                }

                result.Add(permission);
            }

            return result;
        }

        [HttpPost]
        public Vanrise.Entities.UpdateOperationOutput<object> UpdatePermissions(Permission [] permissionObject)
        {
            PermissionManager manager = new PermissionManager();
            return manager.UpdatePermissions(permissionObject);
        } 
       
        public class PermissionResult
        {
            public string PermissionPath { get; set; }

            public List<PermissionFlag> PermissionFlags { get; set; }
        }

        public class PermissionFlag
        {
            public string FlagName { get; set; }

            public Flag FlagValue { get; set; }
        }


    }
}