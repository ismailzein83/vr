using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.GenericData.Entities
{
    public class GenericBusinessEntityQuery
    {
        public Guid BusinessEntityDefinitionId { get; set; }
        public Dictionary<string, object> FilterValuesByFieldPath { get; set; }
        public DateTime? FromTime { get; set; }
        public DateTime? ToTime { get; set; }
        public RecordFilterGroup FilterGroup { get; set; }
       
        public List<GenericBusinessEntityFilter> Filters { get; set; }
    }
    public class GenericBusinessEntityFilter
    {
        public string FieldName { get; set; }
        public List<Object> FilterValues { get; set; }
    }
}
