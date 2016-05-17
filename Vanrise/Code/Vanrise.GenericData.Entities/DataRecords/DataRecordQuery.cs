using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.GenericData.Entities
{
    public enum OrderDirection { Ascending = 0, Descending = 1 }
    public class DataRecordQuery
    {
        public List<int> DataRecordStorageIds { get; set; }

        public DateTime FromTime { get; set; }

        public DateTime ToTime { get; set; }

        public List<string> Columns { get; set; }

        public RecordFilterGroup FilterGroup { get; set; }

        public int LimitResult { get; set; }

        public OrderDirection Direction { get; set; }
    }
}
