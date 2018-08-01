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
        public OutArgument<List<CDRCorrelationFilterGroup>> CDRCorrelationFilterGroups { get; set; }

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

            DataRecordStorageManager manager = new DataRecordStorageManager();
            CDRCorrelationDefinitionSettings settings = cdrCorrelationDefinition.Settings;

            long? lastImportedId = null;
            bool newDataImported = false;
            List<CDRCorrelationFilterGroup> cdrCorrelationFilterGroups = new List<CDRCorrelationFilterGroup>();

            DateTime? overallMinDate;
            DateTime? overallMaxDate;
            long? overallMaxId = manager.GetMaxId(settings.InputDataRecordStorageId, settings.IdFieldName, settings.DatetimeFieldName, out overallMaxDate, out overallMinDate);

            if (cdrCorrelationProcessState != null && cdrCorrelationProcessState.LastImportedId.HasValue)
            {
                long? maxId;
                DateTime? minDate = manager.GetMinDateTimeWithMaxIdAfterId(settings.InputDataRecordStorageId, cdrCorrelationProcessState.LastImportedId.Value, settings.IdFieldName, settings.DatetimeFieldName, out maxId);

                if (minDate.HasValue)
                {
                    newDataImported = true;
                    lastImportedId = maxId;
                    DateTime minDateMinusMargin = minDate.Value.AddSeconds(-1 * dateTimeMargin.TotalSeconds);
                    cdrCorrelationFilterGroups = BuildRecordFilterGroups(minDateMinusMargin, overallMaxDate.Value, batchIntervalTime, settings.DatetimeFieldName, maxId.Value, settings.IdFieldName);
                }
            }
            else
            {
                if (overallMaxId.HasValue) // no data exist in the table
                {
                    newDataImported = true;
                    lastImportedId = overallMaxId;
                    cdrCorrelationFilterGroups = BuildRecordFilterGroups(overallMinDate.Value, overallMaxDate.Value, batchIntervalTime, settings.DatetimeFieldName, overallMaxId.Value, settings.IdFieldName);
                }
            }

            this.CDRCorrelationFilterGroups.Set(context, cdrCorrelationFilterGroups);
            this.NewDataImported.Set(context, newDataImported);
            this.LastImportedId.Set(context, lastImportedId);
        }

        private List<CDRCorrelationFilterGroup> BuildRecordFilterGroups(DateTime minDate, DateTime maxDate, TimeSpan batchIntervalTime, string datetimeFieldName, long maxId, string idFieldName)
        {
            List<CDRCorrelationFilterGroup> cdrCorrelationFilterGroups = new List<CDRCorrelationFilterGroup>();
            List<DateTimeRange> dateTimeRanges = Vanrise.Common.Utilities.GenerateDateTimeRanges(minDate, maxDate, batchIntervalTime).ToList();

            for (int i = 0; i < dateTimeRanges.Count; i++)
            {
                DateTimeRange dateTimeRange = dateTimeRanges[i];
                DateTime from = dateTimeRange.From;
                DateTime? to = null;

                RecordFilterGroup recordFilter = new RecordFilterGroup { LogicalOperator = RecordQueryLogicalOperator.And, Filters = new List<RecordFilter>() };
                recordFilter.Filters.Add(new DateTimeRecordFilter { FieldName = datetimeFieldName, CompareOperator = DateTimeRecordFilterOperator.GreaterOrEquals, Value = from });

                if (i != dateTimeRanges.Count - 1)
                {
                    to = dateTimeRange.To;
                    recordFilter.Filters.Add(new DateTimeRecordFilter { FieldName = datetimeFieldName, CompareOperator = DateTimeRecordFilterOperator.Less, Value = to });
                }

                recordFilter.Filters.Add(new NumberRecordFilter { FieldName = idFieldName, CompareOperator = NumberRecordFilterOperator.LessOrEquals, Value = maxId });
                recordFilter.Filters.Add(new NumberListRecordFilter { FieldName = "RecordType", CompareOperator = ListRecordFilterOperator.In, Values = new List<decimal>() { 0, 1 } });

                cdrCorrelationFilterGroups.Add(new CDRCorrelationFilterGroup() { RecordFilterGroup = recordFilter, From = from, To = to });
            }

            return cdrCorrelationFilterGroups;
        }
    }
}