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

        List<BEPermission> GetPermissionsByEntity(EntityType entityType, string entityId);

        bool UpdatePermission(Permission permission);

        bool DeletePermission(Permission permission);

        bool DeletePermission(int holderType, string holderId, int entityType, string entityId);
    }
}
