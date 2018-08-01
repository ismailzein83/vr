using System;
using System.Activities;
using System.Collections.Generic;
using Vanrise.Entities;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;
using System.Linq;

namespace Vanrise.GenericData.BP.Activities
{
    public sealed class BuildCDRCorrelationFilterGroup : CodeActivity
    {
        [RequiredArgument]
        public InArgument<CDRCorrelationDefinition> CDRCorrelationDefinition { get; set; }

        [RequiredArgument]
        public InArgument<TimeSpan> DateTimeMargin { get; set; }

        [RequiredArgument]
        public InArgument<TimeSpan> BatchIntervalTime { get; set; }

        [RequiredArgument]
        public InArgument<CDRCorrelationProcessState> CDRCorrelationProcessState { get; set; }

        [RequiredArgument]
        public OutArgument<List<RecordFilterGroup>> RecordFilterGroups { get; set; }

        [RequiredArgument]
        public OutArgument<bool> NewDataImported { get; set; }

        [RequiredArgument]
        public OutArgument<long?> LastImportedId { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            CDRCorrelationDefinition cdrCorrelationDefinition = this.CDRCorrelationDefinition.Get(context);
            TimeSpan dateTimeMargin = this.DateTimeMargin.Get(context);
            TimeSpan batchIntervalTime = this.BatchIntervalTime.Get(context);

            CDRCorrelationProcessState cdrCorrelationProcessState = this.CDRCorrelationProcessState.Get(context);

            CDRCorrelationDefinitionSettings settings = cdrCorrelationDefinition.Settings;

            bool newDataImported = false;
            List<RecordFilterGroup> recordFilterGroups = new List<RecordFilterGroup>();


            long? lastImportedId = null;

            DataRecordStorageManager manager = new DataRecordStorageManager();
            DateTime? overallMinDate;
            DateTime? overallMaxDate;
            long? overallMaxId = manager.GetMaxId(settings.InputDataRecordStorageId, settings.IdFieldName, settings.DatetimeFieldName, out overallMaxDate, out overallMinDate);

            if (cdrCorrelationProcessState != null && cdrCorrelationProcessState.LastImportedId.HasValue)
            {
                long? maxId;
                DateTime? minDate = manager.GetMinDateTimeWithMaxIdAfterId(settings.InputDataRecordStorageId, cdrCorrelationProcessState.LastImportedId.Value, settings.IdFieldName, settings.DatetimeFieldName, out maxId);

                if (minDate.HasValue)
                {
                    lastImportedId = maxId;

                    newDataImported = true;
                    DateTime minDateMinusMargin = minDate.Value.AddSeconds(-1 * dateTimeMargin.TotalSeconds);
                    recordFilterGroups = BuildRecordFilterGroups(minDateMinusMargin, overallMaxDate.Value, batchIntervalTime, settings.DatetimeFieldName, maxId.Value, settings.IdFieldName);
                }
            }
            else
            {
                if (overallMaxId.HasValue) // no data exist in the table
                {
                    lastImportedId = overallMaxId;
                    newDataImported = true;
                    recordFilterGroups = BuildRecordFilterGroups(overallMinDate.Value, overallMaxDate.Value, batchIntervalTime, settings.DatetimeFieldName, overallMaxId.Value, settings.IdFieldName);
                }
            }

            this.RecordFilterGroups.Set(context, recordFilterGroups);
            this.NewDataImported.Set(context, newDataImported);
            this.LastImportedId.Set(context, lastImportedId);
        }

        private List<RecordFilterGroup> BuildRecordFilterGroups(DateTime minDate, DateTime maxDate, TimeSpan batchIntervalTime, string datetimeFieldName, long maxId, string idFieldName)
        {
            List<RecordFilterGroup> recordFilterGroups = new List<RecordFilterGroup>();
            List<DateTimeRange> dateTimeRanges = Vanrise.Common.Utilities.GenerateDateTimeRanges(minDate, maxDate, batchIntervalTime).ToList();

            for (int i = 0; i < dateTimeRanges.Count; i++)
            {
                DateTimeRange dateTimeRange = dateTimeRanges[i];

                RecordFilterGroup recordFilter = new RecordFilterGroup { LogicalOperator = RecordQueryLogicalOperator.And, Filters = new List<RecordFilter>() };
                recordFilter.Filters.Add(new DateTimeRecordFilter { FieldName = datetimeFieldName, CompareOperator = DateTimeRecordFilterOperator.GreaterOrEquals, Value = dateTimeRange.From });

                if (i != dateTimeRanges.Count - 1)
                    recordFilter.Filters.Add(new DateTimeRecordFilter { FieldName = datetimeFieldName, CompareOperator = DateTimeRecordFilterOperator.Less, Value = dateTimeRange.To });

                recordFilter.Filters.Add(new NumberRecordFilter { FieldName = idFieldName, CompareOperator = NumberRecordFilterOperator.LessOrEquals, Value = maxId });
                recordFilter.Filters.Add(new NumberListRecordFilter { FieldName = "RecordType", CompareOperator = ListRecordFilterOperator.In, Values = new List<decimal>() { 0, 1 } });

                recordFilterGroups.Add(recordFilter);
            }
            return recordFilterGroups;
        }
    }
}