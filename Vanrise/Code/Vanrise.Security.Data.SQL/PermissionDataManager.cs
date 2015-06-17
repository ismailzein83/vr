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
            return GetItemsSP("sec.sp_Permission_GetAll", PermissionMapper);
        }

        public List<Entities.Permission> GetPermissionsByHolder(Entities.HolderType holderType, string holderId)
        {
            return GetItemsSP("sec.sp_Permission_GetbyHolder", PermissionMapper, holderType, holderId);
        }

        public List<Entities.BEPermission> GetPermissionsByEntity(Entities.EntityType entityType, string entityId)
        {
            return GetItemsSP("sec.sp_Permission_GetbyEntity", BEPermissionMapper, entityType, entityId);
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

        public bool DeletePermission(int holderType, string holderId, int entityType, string entityId)
        {
            int recordsEffected = ExecuteNonQuerySP("sec.sp_Permission_Delete", holderType, holderId, entityType, entityId);
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

        BEPermission BEPermissionMapper(IDataReader reader)
        {
            BEPermission permission = new BEPermission
            {
                HolderType = ((int)reader["HolderType"]) == 0 ? HolderType.USER : HolderType.ROLE,
                HolderId = reader["HolderId"] as string,
                EntityType = ((int)reader["EntityType"]) == 0 ? EntityType.MODULE : EntityType.ENTITY,
                EntityId = reader["EntityId"] as string,
                PermissionFlags = Common.Serializer.Deserialize<List<PermissionFlag>>(reader["PermissionFlags"] as string),
                HolderName = reader["HolderName"] as string
            };
            return permission;
        }
    }
}
