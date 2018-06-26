using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Analytic.Entities
{
    public class AnalyticSummaryBigResult<T> : Vanrise.Entities.BigResult<AnalyticRecord>
    {
        public T Summary { get; set; }

        public List<AnalyticResultSubTable> SubTables { get; set; }
    }
}
