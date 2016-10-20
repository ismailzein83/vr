using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Runtime.Entities
{
    public class SchedulerTaskQuery
    {
        public string NameFilter { get; set; }
        public List<ISchedulerTaskFilter> Filters { get; set; }
        public List<Guid> TaskIds { get; set; }
    }
}
