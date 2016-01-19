﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Security.Business;
using Vanrise.Security.Entities;
using Vanrise.Web.Base;

namespace Vanrise.Security.Web.Controllers
{
    [JSONWithTypeAttribute]
    [RoutePrefix(Constants.ROUTE_PREFIX + "Permission")]
    public class PermissionController : Vanrise.Web.Base.BaseAPIController
    {
        [HttpGet]
        [Route("GetHolderPermissions")]
        public IEnumerable<PermissionDetail> GetHolderPermissions(HolderType holderType, string holderId)
        {
            PermissionManager manager = new PermissionManager();
            return manager.GetHolderPermissions(holderType, holderId);
        }

        [HttpPost]
        [Route("GetFilteredEntityPermissions")]
        public object GetFilteredEntityPermissions(Vanrise.Entities.DataRetrievalInput<PermissionQuery> input)
        {
            PermissionManager manager = new PermissionManager();
            return GetWebResponse(input, manager.GetFilteredEntityPermissions(input));
        }

        [HttpGet]
        [Route("GetEffectivePermissions")]
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

        [HttpPost]
        [Route("UpdatePermissions")]
        public Vanrise.Entities.UpdateOperationOutput<object> UpdatePermissions(Permission[] permissionObject)
        {
            PermissionManager manager = new PermissionManager();
            return manager.UpdatePermissions(permissionObject);
        } 

        [HttpGet]
        [Route("DeletePermissions")]
        public Vanrise.Entities.UpdateOperationOutput<object> DeletePermissions(HolderType holderType, string holderId, EntityType entityType, string entityId)
        {
            PermissionManager manager = new PermissionManager();
            return manager.DeletePermissions(holderType, holderId, entityType, entityId);
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