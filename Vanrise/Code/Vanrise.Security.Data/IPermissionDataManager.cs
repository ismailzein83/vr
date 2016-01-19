using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Security.Entities;

namespace Vanrise.Security.Data
{
    public interface IPermissionDataManager : IDataManager
    {
        IEnumerable<Permission> GetPermissions();

        IEnumerable<Permission> GetHolderPermissions(HolderType holderType, string holderId);

        bool UpdatePermission(Permission permission);

        bool DeletePermission(Permission permission);

        bool DeletePermission(HolderType holderType, string holderId, EntityType entityType, string entityId);

        bool ArePermissionsUpdated(ref object updateHandle);
    }
}
