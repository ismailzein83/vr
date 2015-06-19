using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.Analytics.Entities
{
    public class VariationReportResult
    {
        public List<VariationReports> VariationReportsData { get; set; }
        public DataTable TimeRange;
    }
}
