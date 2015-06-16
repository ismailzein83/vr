using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Security.Entities;

namespace Vanrise.Security.Data
{
    public interface IRoleDataManager : IDataManager
    {
        List<Role> GetFilteredRoles(int fromRow, int toRow, string name);

        Role GetRole(int roleId);

        List<Role> GetRoles();

        List<int> GetUserRoles(int userId);
        
        bool AddRole(Role role, out int insertedId);
        
        bool UpdateRole(Role role);

        void AssignMembers(int roleId, int[] members);
    }
}
