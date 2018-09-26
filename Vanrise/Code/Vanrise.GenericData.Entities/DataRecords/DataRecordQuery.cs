using System;
using System.Collections.Generic;

namespace Vanrise.GenericData.Entities
{
    public enum OrderDirection { Ascending = 0, Descending = 1 }
    public class DataRecordQuery
    {
        public string ReportName { get; set; }
        public List<Guid> DataRecordStorageIds { get; set; }

        public DateTime FromTime { get; set; }

        public DateTime ToTime { get; set; }

        public List<string> Columns { get; set; }

        public List<string> ColumnTitles { get; set; }

        public RecordFilterGroup FilterGroup { get; set; }

        public int? LimitResult { get; set; }

        public OrderDirection Direction { get; set; }

        public List<SortColumn> SortColumns { get; set; }

        public List<DataRecordFilter> Filters { get; set; }
    }

    public class DataRecordFilter
    {
        public string FieldName { get; set; }

        public List<Object> FilterValues { get; set; }
    }

    public class SortColumn
    {
        public string FieldName { get; set; }

        public bool IsDescending { get; set; }
    }
}