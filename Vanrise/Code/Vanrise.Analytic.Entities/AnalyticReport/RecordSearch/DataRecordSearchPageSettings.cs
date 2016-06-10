using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Analytic.Entities
{
    public class DataRecordSearchPageSettings : AnalyticReportSettings
    {
        public List<DRSearchPageStorageSource> Sources { get; set; }
        public int? MaxNumberOfRecords { get; set; }
        public int NumberOfRecords { get; set; }
    }

    public class DRSearchPageStorageSource
    {
        public string Title { get; set; }
        public string Name { get; set; }
        public int DataRecordTypeId { get; set; }

        public List<int> RecordStorageIds { get; set; }

        public List<DRSearchPageGridColumn> GridColumns { get; set; }

        public List<DRSearchPageItemDetail> ItemDetails { get; set; }
    }

    public class DRSearchPageGridColumn
    {
        public string FieldName { get; set; }

        public string FieldTitle { get; set; }

        public string Width { get; set; }
    }

    public class DRSearchPageItemDetail
    {
        public string FieldName { get; set; }

        public string FieldTitle { get; set; }

        public int ColumnWidth { get; set; }
    }
}
