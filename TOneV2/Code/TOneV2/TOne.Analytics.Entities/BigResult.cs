using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.Analytics.Entities
{
    public class BigResult<T>
    {
        public string ResultKey { get; set; }

        public IEnumerable<T> Data { get; set; }

        public int TotalCount { get; set; }
    }

    public class TrafficStatisticSummaryQuery
    {
        public DateTime From { get; set; }

        public DateTime To { get; set; }

        public GenericFilter Filter { get; set; }

        public TrafficStatisticGroupKeys[] GroupKeys { get; set; }
    }
}
