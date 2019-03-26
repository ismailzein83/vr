using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Security.Business;

namespace Vanrise.BusinessProcess.MainExtensions
{
    public class GroupsBPTaskAssignee : BPTaskAssignee
    {
        public List<int> GroupIds { get; set; }
        public override string GetDescription(IBPTaskAssigneeContext context)
        {
            var groupManager = new GroupManager();
            return string.Join(",", this.GroupIds.Select(groupId => groupManager.GetGroupName(groupId)));
        }

        public override IEnumerable<int> GetUserIds(IBPTaskAssigneeContext context)
        {
            var groupManager = new GroupManager();
            HashSet<int> userIds = new HashSet<int>();
            foreach (var groupId in this.GroupIds)
            {
                var groupUserIds = groupManager.GetGroupMembers(groupId);
                if (groupUserIds != null)
                {
                    foreach (var userId in groupUserIds)
                    {
                        userIds.Add(userId);
                    }
                }
            }
            return userIds;
        }
    }
}
