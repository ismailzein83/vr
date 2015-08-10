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
        public PermissionDataManager()
            : base(GetConnectionStringName("SecurityDBConnStringKey", "SecurityDBConnString"))
        {

        }

        public List<Entities.Permission> GetPermissions()
        {
            return GetItemsSP("sec.sp_Permission_GetAll", PermissionMapper);
        }

        public List<Entities.Permission> GetPermissionsByHolder(Entities.HolderType holderType, string holderId)
        {
            return GetItemsSP("sec.sp_Permission_GetbyHolder", PermissionMapper, holderType, holderId);
        }

        public bool UpdatePermission(Permission permission)
        {
            string serialziedPermissionFlag = Common.Serializer.Serialize(permission.PermissionFlags, true);

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

        public bool DeletePermission(HolderType holderType, string holderId, EntityType entityType, string entityId)
        {
            int recordsEffected = ExecuteNonQuerySP("sec.sp_Permission_Delete", holderType, holderId, entityType, entityId);
            return (recordsEffected > 0);
        }

        Permission PermissionMapper(IDataReader reader)
        {
            Permission permission = new Permission
            {
                HolderType = (HolderType)reader["HolderType"],
                HolderId = reader["HolderId"] as string,
                EntityType = (EntityType)reader["EntityType"],
                EntityId = reader["EntityId"] as string,
                PermissionFlags = Common.Serializer.Deserialize<List<PermissionFlag>>(reader["PermissionFlags"] as string),
                HolderName = reader["HolderName"] as string
            };
            return permission;
        }
    }
}
