using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Analytic.Entities;
using Vanrise.Entities;
using Vanrise.GenericData.Entities;

namespace Vanrise.Analytic.MainExtensions.AutomatedReport.Queries
{
    public class RecordSearchQuerySettings : VRAutomatedReportQuerySettings
    {
        public List<RecordSearchQueryDataRecordStorage> DataRecordStorages { get; set; }

        public VRTimePeriod TimePeriod { get; set; }

        public List<RecordSearchQueryColumn> Columns { get; set; }

        public RecordFilterGroup FilterGroup { get; set; }

        public int LimitResult { get; set; }

        public OrderDirection Direction { get; set; }

        public List<SortColumn> SortColumns { get; set; }

        public List<DataRecordFilter> Filters { get; set; }

        public override VRAutomatedReportDataResult Execute(IVRAutomatedReportQueryExecuteContext context)
        {
            throw new NotImplementedException();
        }

        public override VRAutomatedReportDataSchema GetSchema(IVRAutomatedReportQueryGetSchemaContext context)
        {
            throw new NotImplementedException();
        }
    }

    public class RecordSearchQueryDataRecordStorage
    {
        public Guid DataRecordStorageId { get; set; }
    }

    public class RecordSearchQueryColumn
    {
        public string ColumnName { get; set; }
    }
}
