using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.Analytics.Entities
{
    public class TrafficStatisticSummaryBigResult<T> : Vanrise.Entities.BigResult<TrafficStatisticGroupSummary<T>>
    {
        public T Summary { get; set; }
    }
}
