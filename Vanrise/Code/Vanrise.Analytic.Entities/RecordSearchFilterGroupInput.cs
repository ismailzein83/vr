using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Analytic.Entities
{
    public class RecordSearchFilterGroupInput
    {
        public List<DimensionFilter> DimensionFilters { get; set; }
        public int ReportId { get; set; }
        public string SourceName { get; set; }
        public int TableId { get; set; }
    }
}
