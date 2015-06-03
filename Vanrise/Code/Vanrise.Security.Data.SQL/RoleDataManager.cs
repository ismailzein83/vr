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
    public class RoleDataManager : BaseSQLDataManager, IRoleDataManager
    {
        public List<Entities.Role> GetFilteredRoles(int fromRow, int toRow, string name)
        {
            return GetItemsSP("sec.sp_Roles_GetFilteredRoles", RoleMapper, fromRow, toRow, name);
        }

        public Entities.Role GetRole(int roleId)
        {
            return GetItemSP("sec.sp_Roles_GetRole", RoleMapper, roleId);
        }


        public bool AddRole(Entities.Role roleObject, out int insertedId)
        {
            object roleID;

            int recordesEffected = ExecuteNonQuerySP("sec.sp_Roles_Insert", out roleID, roleObject.Name,
                !string.IsNullOrEmpty(roleObject.Description) ? roleObject.Description : null);
            insertedId = (int)roleID;
            return (recordesEffected > 0);
        }

        public bool UpdateRole(Entities.Role roleObject)
        {
            int recordesEffected = ExecuteNonQuerySP("sec.sp_Roles_Update", roleObject.RoleId, roleObject.Name,
                !string.IsNullOrEmpty(roleObject.Description) ? roleObject.Description : null);
            return (recordesEffected > 0);
        }

        #region Private Methods

        Role RoleMapper(IDataReader reader)
        {
            Role role = new Role
            {
                RoleId = (int)reader["ID"],
                Name = reader["Name"] as string,
                Description = reader["Description"] as string
            };
            return role;
        }

        #endregion
    }
}
