using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;

namespace Vanrise.Analytic.Entities
{
    public class RecordSearchFieldFilterInput
    {
        public List<DataRecordFilter> FieldFilters { get; set; }
        public Guid ReportId { get; set; }
        public string SourceName { get; set; }
    }
   
}
