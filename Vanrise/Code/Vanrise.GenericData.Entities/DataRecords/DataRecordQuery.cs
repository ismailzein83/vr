using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.GenericData.Entities
{
    public class DataRecordQuery
    {
        public int DataRecordStorageId { get; set; }

        public DateTime FromTime { get; set; }

        public DateTime ToTime { get; set; }

        public RecordFilterGroup FilterGroup { get; set; }
    }

    public enum RecordQueryLogicalOperator { And = 1, Or = 2 }

    public class RecordFilterGroup : RecordFilter
    {
        public RecordQueryLogicalOperator LogicalOperator { get; set; }

        public List<RecordFilter> Filters { get; set; }
    }

    public abstract class RecordFilter
    {
        public string FieldName { get; set; }
    }
}
