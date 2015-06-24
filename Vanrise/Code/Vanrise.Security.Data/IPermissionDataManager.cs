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
        List<Permission> GetPermissions();

        List<Permission> GetPermissionsByHolder(HolderType holderType, string holderId);

        bool UpdatePermission(Permission permission);

        bool DeletePermission(Permission permission);

        bool DeletePermission(HolderType holderType, string holderId, EntityType entityType, string entityId);
    }
}
