using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;

namespace Vanrise.Analytic.Entities
{
    public class AnalyticQuery
    {
        public int TableId { get; set; }
        public DateTime FromTime { get; set; }
        public DateTime ToTime { get; set; }
        public string Currency { get; set; }
        public int? CurrencyId { get; set; }
        public List<string> DimensionFields { get; set; }
        public List<string> ParentDimensions { get; set; }
        public List<string> MeasureFields { get; set; }
        public List<DimensionFilter> Filters { get; set; }
        public RecordFilterGroup FilterGroup { get; set; }

        public bool WithSummary { get; set; }
        public int? TopRecords { get; set; }
        public List<string> OrderBy { get; set; }
    }
}
