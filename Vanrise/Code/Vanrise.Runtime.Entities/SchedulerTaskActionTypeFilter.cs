using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Runtime.Entities
{
    public class SchedulerTaskActionTypeFilter
    {
        public List<ISchedulerTaskActionTypeFilter> Filters { get; set; }
    }

    public interface ISchedulerTaskActionTypeFilter
    {
        bool IsMatched(ISchedulerTaskActionTypeFilterContext context);
    }

    public interface ISchedulerTaskActionTypeFilterContext
    {
        SchedulerTaskActionType SchedulerTaskActionType { get; }
    }

    public class SchedulerTaskActionTypeFilterContext : ISchedulerTaskActionTypeFilterContext
    {
        public SchedulerTaskActionType SchedulerTaskActionType { get; set; }
    }

}
