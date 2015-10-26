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
        List<Group> GetGroups();

        bool AddGroup(Group role, out int insertedId);

        bool UpdateGroup(Group role);

        void AssignMembers(int roleId, int[] members);

        bool AreGroupsUpdated(ref object updateHandle);

        List<int> GetUserGroups(int userId);
    }
}
