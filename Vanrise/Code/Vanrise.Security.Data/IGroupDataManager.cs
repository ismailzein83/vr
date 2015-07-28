using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Security.Entities;

namespace Vanrise.Security.Data
{
    public interface IGroupDataManager : IDataManager
    {
        List<Group> GetFilteredGroups(int fromRow, int toRow, string name);

        Group GetGroup(int roleId);

        List<Group> GetGroups();

        List<int> GetUserGroups(int userId);
        
        bool AddGroup(Group role, out int insertedId);
        
        bool UpdateGroup(Group role);

        void AssignMembers(int roleId, int[] members);
    }
}
