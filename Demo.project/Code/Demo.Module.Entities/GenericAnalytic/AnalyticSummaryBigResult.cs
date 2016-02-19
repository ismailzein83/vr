using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Module.Entities
{
    public class AnalyticSummaryBigResult<T> : Vanrise.Entities.BigResult<AnalyticRecord>
    {
        public T Summary { get; set; }
    }
}
