using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Analytic.Entities
{
    public abstract class TimeRangeFilter
    {
        public virtual Guid ConfigId { get; set; }

        public abstract void Evaluate(ITimeRangeFilterContext context);
    }

    public interface ITimeRangeFilterContext
    {
        DateTime FromTime { set; }

        DateTime ToTime { set; }
    }
}
