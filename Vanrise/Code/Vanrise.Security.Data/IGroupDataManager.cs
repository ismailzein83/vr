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
        Vanrise.Entities.BigResult<Group> GetFilteredGroups(Vanrise.Entities.DataRetrievalInput<GroupQuery> input);

        Group GetGroup(int roleId);

        List<Group> GetGroups();

        List<int> GetUserGroups(int userId);
        
        bool AddGroup(Group role, out int insertedId);
        
        bool UpdateGroup(Group role);

        void AssignMembers(int roleId, int[] members);
    }
}
