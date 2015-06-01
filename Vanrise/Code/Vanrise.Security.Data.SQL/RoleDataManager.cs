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
        public List<Entities.Role> GetRoles()
        {
            return GetItemsSP("secu.sp_Roles_GetAll", RoleMapper);
        }

        #region Private Methods

        Role RoleMapper(IDataReader reader)
        {
            Role role = new Role
            {
                RoleId = (int)reader["ID"],
                Name = reader["Name"] as string
            };
            return role;
        }

        #endregion
    }
}
