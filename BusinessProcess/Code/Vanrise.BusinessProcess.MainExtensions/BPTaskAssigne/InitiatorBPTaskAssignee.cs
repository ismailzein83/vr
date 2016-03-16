using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Security.Business;

namespace Vanrise.BusinessProcess.MainExtensions
{
    public class InitiatorBPTaskAssignee : BPTaskAssignee
    {
        public List<int> UserIds { get; set; }
        public override IEnumerable<int> GetUserIds(IBPTaskAssigneeContext context)
        {
            UserIds = new List<int>();
            UserIds.Add(context.ProcessInitiaterUserId);
            return UserIds;
        }

        public override string GetDescription(IBPTaskAssigneeContext context)
        {
            UserManager userManager = new UserManager();
            return userManager.GetUsersNames(UserIds);
        }
    }
}