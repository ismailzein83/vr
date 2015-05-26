using System;
using System.Collections.Generic;
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
        public List<Entities.Role> GetRoles(int fromRow, int toRow)
        {
            return GetItemsSP("mainmodule.sp_Role_GetAll", (reader) =>
            {
                return new Entities.Role
                {
                    RoleId = Convert.ToInt32(reader["Id"]),
                    Name = reader["Name"] as string,
                    Description = reader["Description"] as string
                };
            }, fromRow, toRow);
        }

        public void DeleteRole(int Id)
        {
            ExecuteNonQuerySP("mainmodule.sp_Role_Delete", Id);
        }

        public Role AddRole(Role Role)
        {
            object obj;

            if (ExecuteNonQuerySP("mainmodule.sp_Role_Insert", out obj, Role.Name, Role.Description) > 0)
            {
                Role.RoleId = (int)obj;
                return Role;
            }
            else
                return null;
        }

        public bool UpdateRole(Role Role)
        {
            if (ExecuteNonQuerySP("mainmodule.sp_Role_Update", Role.RoleId, Role.Name, Role.Description) > 0)
                return true;
            else
                return false;
        }

        public List<Entities.Role> SearchRole(string name)
        {
            return GetItemsSP("mainmodule.sp_Role_Search", (reader) =>
            {
                return new Entities.Role
                {
                    RoleId = Convert.ToInt32(reader["Id"]),
                    Name = reader["Name"] as string,
                    Description = reader["Description"] as string
                };
            }, name);
        }
    }
}
