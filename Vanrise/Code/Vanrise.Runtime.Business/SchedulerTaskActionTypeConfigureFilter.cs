using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Runtime.Entities;
using Vanrise.Security.Entities;

namespace Vanrise.Runtime.Business
{
    public class SchedulerTaskActionTypeConfigureFilter : ISchedulerTaskActionTypeFilter
    {
        public bool IsMatched(ISchedulerTaskActionTypeFilterContext context)
        {
            return new SchedulerTaskActionTypeManager().DoesUserHaveConfigureAccess(ContextFactory.GetContext().GetLoggedInUserId(), context.SchedulerTaskActionType.Info);
        }
    }
}
