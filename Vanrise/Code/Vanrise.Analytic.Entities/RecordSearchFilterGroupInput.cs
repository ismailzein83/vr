using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;

namespace Vanrise.Analytic.Entities
{
    public class RecordSearchFilterGroupInput
    {
        public RecordFilterGroup FilterGroup { get; set; }
        public List<DimensionFilter> DimensionFilters { get; set; }
        public Guid ReportId { get; set; }
        public string SourceName { get; set; }
        public int TableId { get; set; }
    }
}
