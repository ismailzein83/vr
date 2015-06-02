using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using TOne.Entities;
using System.Web;
using TOne.Business;
using System.Threading.Tasks;
using TOne.Data;

namespace TOne.Data.SQL
{
    public class RoleDataManager : BaseTOneDataManager, IRoleDataManager
    {
        public List<Entities.Role> GetFilteredRoles(int fromRow, int toRow, string name)
        {
            return GetItemsSP("mainmodule.sp_Role_GetFilteredRoles", (reader) =>
            {
                return new Entities.Role
                {
                    RoleId = Convert.ToInt32(reader["Id"]),
                    Name = reader["Name"] as string,                   
                    Description = reader["Description"] as string
                };
            }, fromRow, toRow, name);
        }

        public Role GetRole(int roleId)
        {
            return GetItemsSP("mainmodule.sp_Role_GetRole", RoleMapper, roleId).FirstOrDefault();
        }

        public bool AddRole(Role roleObject, out int insertedId)
        {
            object RoleID;

            int recordesEffected = ExecuteNonQuerySP("mainmodule.sp_Role_Insert", out RoleID,
            !string.IsNullOrEmpty(roleObject.Name) ? roleObject.Name : null,
               !string.IsNullOrEmpty(roleObject.Description) ? roleObject.Description : null
            );
            insertedId = (int)RoleID;
            if (recordesEffected > 0)
                return true;
            return false;
        }

        public bool UpdateRole(Role roleObject)
        {
            int recordesEffected = ExecuteNonQuerySP("mainmodule.sp_Role_Update",
                 roleObject.RoleId,
                 !string.IsNullOrEmpty(roleObject.Name) ? roleObject.Name : null,
                 !string.IsNullOrEmpty(roleObject.Description) ? roleObject.Description : null
            );
            if (recordesEffected > 0)
                return true;
            return false;
        }

        private Role RoleMapper(IDataReader reader)
        {
            return new Entities.Role
            {
                RoleId = Convert.ToInt32(reader["Id"]),
                Name = reader["Name"] as string,
                Description = reader["Description"] as string
            };
        }
        
        public void DeleteRole(int roleId)
        {
            ExecuteNonQuerySP("mainmodule.sp_Role_Delete", roleId);
        }
    }
}
