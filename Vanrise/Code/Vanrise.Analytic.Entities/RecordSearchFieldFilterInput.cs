using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Analytic.Entities
{
    public class RecordSearchFieldFilterInput
    {
        public List<FieldFilter> FieldFilters { get; set; }
        public Guid ReportId { get; set; }
        public string SourceName { get; set; }
    }
    public class FieldFilter
    {
        public string FieldName { get; set; }
        public List<Object> FilterValues { get; set; }
    }
}
