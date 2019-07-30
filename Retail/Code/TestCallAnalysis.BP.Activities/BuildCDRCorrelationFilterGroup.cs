using System;
using System.Activities;
using System.Collections.Generic;
using TestCallAnalysis.Business;
using TestCallAnalysis.Entities;
using Vanrise.Common.Business;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;

namespace TestCallAnalysis.BP.Activities
{
    public sealed class BuildCDRCorrelationFilterGroup : CodeActivity
    {
        [RequiredArgument]
        public InArgument<TestCallAnalysis.Entities.CDRCorrelationProcessState> CDRCorrelationProcessState { get; set; }
        [RequiredArgument]
        public OutArgument<List<TestCallAnalysis.Entities.CDRCorrelationFilterGroup>> CDRCorrelationFilterGroups { get; set; }
        [RequiredArgument]
        public OutArgument<bool> NewDataImported { get; set; }
        [RequiredArgument]
        public OutArgument<long?> LastImportedId { get; set; }
        [RequiredArgument]
        public OutArgument<TCAnalSettingsData> TCAnalSettingsData { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            TestCallAnalysis.Entities.CDRCorrelationProcessState cdrCorrelationProcessState = this.CDRCorrelationProcessState.Get(context);

            var TCanalComponentSettingsId = new Guid("6627cae7-7653-482b-a67f-6d44cfa00096");
            SettingManager settingManager = new SettingManager();
            var tcanalComponentSettings = settingManager.GetSetting(TCanalComponentSettingsId);
            var tcanalSettingsData = tcanalComponentSettings.Data as TCAnalSettingsData;

            DataRecordStorageManager dataRecordStorageManager = new DataRecordStorageManager();
            long? lastImportedId = null;
            bool newDataImported = false;
            List<TestCallAnalysis.Entities.CDRCorrelationFilterGroup> cdrCorrelationFilterGroups = new List<TestCallAnalysis.Entities.CDRCorrelationFilterGroup>();

            DateTime? overallMinDate;
            DateTime? overallMaxDate;
            DataRecordTypeManager dataRecordTypeManager = new DataRecordTypeManager();
            var inputDataRecordStorageId = MappedCDRManager.dataRecordStorage;
            var dataRecordStorage = dataRecordStorageManager.GetDataRecordStorage(inputDataRecordStorageId);
            
            var dataRecordType = dataRecordTypeManager.GetDataRecordType(dataRecordStorage.DataRecordTypeId);
            var idFieldName = dataRecordType.Settings.IdField;
            var datetimeFieldName = dataRecordType.Settings.DateTimeField;

            long? overallMaxId = dataRecordStorageManager.GetMaxId(inputDataRecordStorageId, out overallMaxDate, out overallMinDate);

            if (cdrCorrelationProcessState != null && cdrCorrelationProcessState.LastImportedId.HasValue)
            {
                RecordFilterGroup recordFilter = new RecordFilterGroup { LogicalOperator = RecordQueryLogicalOperator.And, Filters = new List<RecordFilter>() };
                recordFilter.Filters.Add(new BooleanRecordFilter { FieldName = "IsCorrelated", IsTrue = false});
                long? maxId;
                DateTime? minDate = dataRecordStorageManager.GetMinDateTimeWithMaxIdByFilter(inputDataRecordStorageId, recordFilter, out maxId);

                if (minDate.HasValue)
                {
                    newDataImported = true;
                    lastImportedId = maxId;
                    DateTime minDateMinusMargin = minDate.Value.AddSeconds(-1 * tcanalSettingsData.TimeMargin.TotalSeconds);
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
            this.TCAnalSettingsData.Set(context, tcanalSettingsData);
        }

        private List<TestCallAnalysis.Entities.CDRCorrelationFilterGroup> BuildRecordFilterGroups(DateTime minDate, DateTime maxDate, string datetimeFieldName, long maxId, string idFieldName)
        {
            TimeSpan oneSecond = new TimeSpan(0, 0, 0, 1);
            maxDate = maxDate + oneSecond;
            List<TestCallAnalysis.Entities.CDRCorrelationFilterGroup> cdrCorrelationFilterGroups = new List<TestCallAnalysis.Entities.CDRCorrelationFilterGroup>();
            RecordFilterGroup recordFilter = new RecordFilterGroup { LogicalOperator = RecordQueryLogicalOperator.And, Filters = new List<RecordFilter>() };
            recordFilter.Filters.Add(new DateTimeRecordFilter { FieldName = datetimeFieldName, CompareOperator = DateTimeRecordFilterOperator.GreaterOrEquals, Value = minDate });
            recordFilter.Filters.Add(new DateTimeRecordFilter { FieldName = datetimeFieldName, CompareOperator = DateTimeRecordFilterOperator.Less, Value = maxDate });
            recordFilter.Filters.Add(new NumberRecordFilter { FieldName = idFieldName, CompareOperator = NumberRecordFilterOperator.LessOrEquals, Value = maxId });
            recordFilter.Filters.Add(new NumberListRecordFilter { FieldName = "CDRType", CompareOperator = ListRecordFilterOperator.In, Values = new List<decimal>() { 1, 2 } });
            recordFilter.Filters.Add(new BooleanRecordFilter { FieldName = "IsCorrelated", IsTrue = false });
            cdrCorrelationFilterGroups.Add(new TestCallAnalysis.Entities.CDRCorrelationFilterGroup() { RecordFilterGroup = recordFilter, From = minDate, To = maxDate });
            return cdrCorrelationFilterGroups;
        }
    }
}
