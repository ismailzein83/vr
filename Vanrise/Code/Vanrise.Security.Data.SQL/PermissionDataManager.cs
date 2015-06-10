using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;
using Vanrise.Security.Entities;

namespace Vanrise.Security.Data.SQL
{
    public class PermissionDataManager : BaseSQLDataManager, IPermissionDataManager
    {
        public List<Entities.Permission> GetPermissions()
        {
            return GetItemsSP("sec.sp_Permissions_GetPermissions", PermissionMapper);
        }

        public List<Entities.Permission> GetPermissions(Entities.HolderType entType, string holderId)
        {
            return GetItemsSP("sec.sp_Permissions_GetPermissionsbyHolderTypeandId", PermissionMapper, entType, holderId);
        }

        public bool UpdatePermission(Permission permission)
        {
            string serialziedPermissionFlag = Common.Serializer.Serialize(permission.PermissionFlags);

            int recordsEffected = ExecuteNonQuerySP("sec.sp_Permission_Update", permission.HolderType,
                permission.HolderId, permission.EntityType, permission.EntityId, serialziedPermissionFlag);

            return (recordsEffected > 0);
        }

        public bool DeletePermission(Permission permission)
        {
            int recordsEffected = ExecuteNonQuerySP("sec.sp_Permission_Delete", permission.HolderType,
                permission.HolderId, permission.EntityType, permission.EntityId);

            return (recordsEffected > 0);
        }

        Permission PermissionMapper(IDataReader reader)
        {
            Permission permission = new Permission
            {
                HolderType =  ((int)reader["HolderType"]) == 0 ? HolderType.USER : HolderType.ROLE,
                HolderId = reader["HolderId"] as string,
                EntityType = ((int)reader["EntityType"]) == 0 ? EntityType.MODULE : EntityType.ENTITY,
                EntityId = reader["EntityId"] as string,
                PermissionFlags = Common.Serializer.Deserialize<List<PermissionFlag>>(reader["PermissionFlags"] as string)
            };
            return permission;
        }
    }
}
