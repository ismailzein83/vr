using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestCallAnalysis.Entities;
using Vanrise.Entities;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;

namespace TestCallAnalysis.BP.Activities
{
    public sealed class BuildCDRCorrelationFilterGroup : CodeActivity
    {

        [RequiredArgument]
        public InArgument<TimeSpan> DateTimeMargin { get; set; }

        [RequiredArgument]
        public InArgument<TestCallAnalysis.Entities.CDRCorrelationProcessState> CDRCorrelationProcessState { get; set; }

        [RequiredArgument]
        public OutArgument<List<TestCallAnalysis.Entities.CDRCorrelationFilterGroup>> CDRCorrelationFilterGroups { get; set; }

        [RequiredArgument]
        public OutArgument<bool> NewDataImported { get; set; }

        [RequiredArgument]
        public OutArgument<long?> LastImportedId { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            TimeSpan dateTimeMargin = this.DateTimeMargin.Get(context);
            TestCallAnalysis.Entities.CDRCorrelationProcessState cdrCorrelationProcessState = this.CDRCorrelationProcessState.Get(context);
            DataRecordStorageManager manager = new DataRecordStorageManager();
            long? lastImportedId = null;
            bool newDataImported = false;
            List<TestCallAnalysis.Entities.CDRCorrelationFilterGroup> cdrCorrelationFilterGroups = new List<TestCallAnalysis.Entities.CDRCorrelationFilterGroup>();

            DateTime? overallMinDate;
            DateTime? overallMaxDate;
            DataRecordTypeManager _dataRecordTypeManager = new DataRecordTypeManager();
            var inputDataRecordStorageId = new Guid("f8c165ad-21c3-4e29-9e3a-7d3e332fdc42");
            var dataRecordStorage = manager.GetDataRecordStorage(inputDataRecordStorageId);
            
            var dataRecordType = _dataRecordTypeManager.GetDataRecordType(dataRecordStorage.DataRecordTypeId);
            var idFieldName = dataRecordType.Settings.IdField;
            var datetimeFieldName = dataRecordType.Settings.DateTimeField;

            long? overallMaxId = manager.GetMaxId(inputDataRecordStorageId, idFieldName, datetimeFieldName, out overallMaxDate, out overallMinDate);

            if (cdrCorrelationProcessState != null && cdrCorrelationProcessState.LastImportedId.HasValue)
            {
                long? maxId;
                DateTime? minDate = manager.GetMinDateTimeWithMaxIdAfterId(inputDataRecordStorageId, cdrCorrelationProcessState.LastImportedId.Value, idFieldName, datetimeFieldName, out maxId);

                if (minDate.HasValue)
                {
                    newDataImported = true;
                    lastImportedId = maxId;
                    DateTime minDateMinusMargin = minDate.Value.AddSeconds(-1 * dateTimeMargin.TotalSeconds);
                    cdrCorrelationFilterGroups = BuildRecordFilterGroups(minDateMinusMargin, overallMaxDate.Value, datetimeFieldName, maxId.Value, idFieldName);
                }
            }
            else
            {
                if (overallMaxId.HasValue) // no data exist in the table
                {
                    newDataImported = true;
                    lastImportedId = overallMaxId;
                    cdrCorrelationFilterGroups = BuildRecordFilterGroups(overallMinDate.Value, overallMaxDate.Value, datetimeFieldName, overallMaxId.Value, idFieldName);
                }
            }

            this.CDRCorrelationFilterGroups.Set(context, cdrCorrelationFilterGroups);
            this.NewDataImported.Set(context, newDataImported);
            this.LastImportedId.Set(context, lastImportedId);
        }

        private List<TestCallAnalysis.Entities.CDRCorrelationFilterGroup> BuildRecordFilterGroups(DateTime minDate, DateTime maxDate, string datetimeFieldName, long maxId, string idFieldName)
        {
            List<TestCallAnalysis.Entities.CDRCorrelationFilterGroup> cdrCorrelationFilterGroups = new List<TestCallAnalysis.Entities.CDRCorrelationFilterGroup>();
            RecordFilterGroup recordFilter = new RecordFilterGroup { LogicalOperator = RecordQueryLogicalOperator.And, Filters = new List<RecordFilter>() };
            recordFilter.Filters.Add(new DateTimeRecordFilter { FieldName = datetimeFieldName, CompareOperator = DateTimeRecordFilterOperator.GreaterOrEquals, Value = minDate });
            recordFilter.Filters.Add(new DateTimeRecordFilter { FieldName = datetimeFieldName, CompareOperator = DateTimeRecordFilterOperator.Less, Value = maxDate });
            recordFilter.Filters.Add(new NumberRecordFilter { FieldName = idFieldName, CompareOperator = NumberRecordFilterOperator.LessOrEquals, Value = maxId });
            recordFilter.Filters.Add(new NumberListRecordFilter { FieldName = "CDRType", CompareOperator = ListRecordFilterOperator.In, Values = new List<decimal>() { 1, 2 } });
            cdrCorrelationFilterGroups.Add(new TestCallAnalysis.Entities.CDRCorrelationFilterGroup() { RecordFilterGroup = recordFilter, From = minDate, To = maxDate });
            return cdrCorrelationFilterGroups;
        }
    }
}
