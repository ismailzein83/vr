using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TOne.Entities;

namespace TOne.Data
{
    public interface IRoleDataManager : IDataManager
    {
        List<Role> GetFilteredRoles(int fromRow, int toRow, string name);

        void DeleteRole(int id);

        Role GetRole(int roleId);

        bool AddRole(Role role, out int insertedId);

        bool UpdateRole(Role role);
    }
}
