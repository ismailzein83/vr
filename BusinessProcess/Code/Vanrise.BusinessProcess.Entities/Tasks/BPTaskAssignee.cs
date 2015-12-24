using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.BusinessProcess.Entities
{
    public abstract class BPTaskAssignee
    {
        public abstract IEnumerable<int> GetUserIds(IBPTaskAssigneeContext context);

        public abstract string GetDescription(IBPTaskAssigneeContext context);
    }
}
