using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Security.Business;

namespace Vanrise.BusinessProcess.MainExtensions
{
    public class TestBPTaskAssignee : BPTaskAssignee
    {
        public List<int> UserIds { get; set; }
        public override IEnumerable<int> GetUserIds(IBPTaskAssigneeContext context)
        {
            return UserIds;
        }

        public override string GetDescription(IBPTaskAssigneeContext context)
        {
            UserManager userManager = new UserManager();
            return userManager.GetUsersNames(UserIds);
        }
    }
}
