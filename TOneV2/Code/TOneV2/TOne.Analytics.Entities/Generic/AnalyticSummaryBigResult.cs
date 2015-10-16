using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.Analytics.Entities
{
    public class AnalyticSummaryBigResult<T> : Vanrise.Entities.BigResult<AnalyticRecord>
    {
        public T Summary { get; set; }
    }
}
