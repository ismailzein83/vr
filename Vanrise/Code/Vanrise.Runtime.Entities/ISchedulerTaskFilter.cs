using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Runtime.Entities
{
    public interface ISchedulerTaskFilter
    {
        bool IsMatched(SchedulerTask task);
    }

    public class SchedulerTaskFilter
    {
        public List<ISchedulerTaskFilter> Filters { get; set; }
    }
}