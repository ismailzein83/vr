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
        public IEnumerable<Permission> GetPermissionsByHolder(HolderType holderType, string holderId)
        {
            PermissionManager manager = new PermissionManager();
            return manager.GetPermissionsByHolder(holderType, holderId);
        }

        [HttpGet]
        public IEnumerable<Permission> GetPermissionsByEntity(EntityType entityType, string entityId)
        {
            PermissionManager manager = new PermissionManager();
            return manager.GetPermissionsByEntity(entityType, entityId);
        }

        [HttpGet]
        public PermissionResultWrapper GetEffectivePermissions()
        {
            PermissionManager manager = new PermissionManager();
            EffectivePermissionsWrapper effectivePermissionsWrapper = manager.GetEffectivePermissions(SecurityContext.Current.GetLoggedInUserId());

            List<PermissionResult> permissionResults = new List<PermissionResult>();

            foreach (KeyValuePair<string, Dictionary<string, Flag>> permKvp in effectivePermissionsWrapper.EffectivePermissions)
            {
                PermissionResult permission = new PermissionResult() { PermissionPath = permKvp.Key };
                permission.PermissionFlags = new List<PermissionFlag>();

                foreach (KeyValuePair<string, Flag> flagKvp in permKvp.Value)
                {
                    permission.PermissionFlags.Add(new PermissionFlag() { FlagName = flagKvp.Key, FlagValue = flagKvp.Value });
                }

                permissionResults.Add(permission);
            }

            PermissionResultWrapper wrapperResult = new PermissionResultWrapper();
            wrapperResult.PermissionResults = permissionResults;
            wrapperResult.BreakInheritanceEntities = effectivePermissionsWrapper.BreakInheritanceEntities;

            return wrapperResult;
        }

        [HttpGet]
        public Vanrise.Entities.UpdateOperationOutput<object> DeletePermission(HolderType holderType, string holderId, EntityType entityType, string entityId)
        {
            PermissionManager manager = new PermissionManager();
            return manager.DeletePermission(holderType, holderId, entityType, entityId);
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

        public class PermissionResultWrapper
        {
            public List<PermissionResult> PermissionResults { get; set; }

            public HashSet<string> BreakInheritanceEntities { get; set; }
        }

    }
}