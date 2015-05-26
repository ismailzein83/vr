using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TOne.Entities;

namespace TOne.Data
{
    public interface IRoleDataManager : IDataManager
    {
        List<Role> GetRoles(int fromRow, int toRow);

        void DeleteRole(int Id);

        Role AddRole(Role Role);

        bool UpdateRole(Role Role);

        List<Entities.Role> SearchRole(string name);
    }
}
