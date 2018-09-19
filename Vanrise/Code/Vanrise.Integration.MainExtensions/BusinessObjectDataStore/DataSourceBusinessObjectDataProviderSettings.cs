using System;
using System.Collections.Generic;
using Vanrise.Common;
using Vanrise.GenericData.Business;
using Vanrise.Integration.Business;
using Vanrise.Integration.Entities;

namespace Vanrise.Integration.MainExtensions.BusinessObjectDataStore
{
    public class DataSourceBusinessObjectDataProviderSettings : BusinessObjectDataProviderExtendedSettings
    {
        public override Guid ConfigId { get { return new Guid("345363B4-3D89-45AB-85D1-687674C77DFC"); } }

        public override bool DoesSupportFilterOnAllFields { get { return false; } }

        private const string LastHourPrefix = "LastHour";
        private const string Last24HoursPrefix = "Last24Hours";
        private const int TotalIntervalsCount = 2;

        public override void LoadRecords(IBusinessObjectDataProviderLoadRecordsContext context)
        {
            var enabledDataSourcesIds = new DataSourceManager().GetEnabledDataSourcesIds();
            if (enabledDataSourcesIds == null || enabledDataSourcesIds.Count == 0)
                return;

            DateTime now = DateTime.Now;

            HashSet<DSAnalysisInterval> dsAnalysisIntervalHashSet = new HashSet<DSAnalysisInterval>();
            var manager = new DataSourceImportedBatchManager();

            foreach (var field in context.Fields)
            {
                if (field.StartsWith(LastHourPrefix))
                    dsAnalysisIntervalHashSet.Add(new DSAnalysisInterval() { Prefix = LastHourPrefix, NbOfHours = 1 });

                else if (field.StartsWith(Last24HoursPrefix))
                    dsAnalysisIntervalHashSet.Add(new DSAnalysisInterval() { Prefix = Last24HoursPrefix, NbOfHours = 24 });

                if (dsAnalysisIntervalHashSet.Count == TotalIntervalsCount)
                    break;
            }

            var dataSourceSummaryByIdAndInterval = new Dictionary<Guid, Dictionary<DSAnalysisInterval, DataSourceSummary>>();
            foreach (Guid datasourceId in enabledDataSourcesIds)
                dataSourceSummaryByIdAndInterval.Add(datasourceId, new Dictionary<DSAnalysisInterval, DataSourceSummary>());

            foreach (var dsAnalysisInterval in dsAnalysisIntervalHashSet)
            {
                DateTime loadRecordsFromTime = now.AddHours(-dsAnalysisInterval.NbOfHours);

                var dataSourcesSummary = manager.GetDataSourcesSummary(loadRecordsFromTime, enabledDataSourcesIds);

                if (dataSourcesSummary != null && dataSourcesSummary.Count > 0)
                {
                    foreach (var dataSourceSummary in dataSourcesSummary)
                    {
                        dataSourceSummaryByIdAndInterval.GetOrCreateItem(dataSourceSummary.DataSourceId).Add(dsAnalysisInterval, dataSourceSummary);
                    }
                }
            }

            if (dataSourceSummaryByIdAndInterval != null && dataSourceSummaryByIdAndInterval.Count > 0)
                foreach (var dataSourceSummaryByIntervalKvp in dataSourceSummaryByIdAndInterval)
                {
                    Guid dataSourceId = dataSourceSummaryByIntervalKvp.Key;
                    Dictionary<DSAnalysisInterval, DataSourceSummary> dataSourceSummaryByInterval = dataSourceSummaryByIntervalKvp.Value;

                    context.OnRecordLoaded(DataRecordObjectMapper(dataSourceId, dataSourceSummaryByInterval, now), DateTime.Now);
                }
        }

        private DataRecordObject DataRecordObjectMapper(Guid datasourceId, Dictionary<DSAnalysisInterval, DataSourceSummary> dataSourceSummaryByInterval, DateTime now)
        {
            var dataSourceSummaryObject = new Dictionary<string, object>();
            dataSourceSummaryObject.Add("DataSource", datasourceId);

            foreach (var dataSourceSummaryKvp in dataSourceSummaryByInterval)
            {
                DSAnalysisInterval interval = dataSourceSummaryKvp.Key;
                DataSourceSummary dataSourceSummary = dataSourceSummaryKvp.Value;

                dataSourceSummaryObject.Add(string.Concat(interval.Prefix, "MaxBatchTime"), dataSourceSummary.LastImportedBatchTime);
                dataSourceSummaryObject.Add(string.Concat(interval.Prefix, "NbBatches"), dataSourceSummary.NbImportedBatch);
                dataSourceSummaryObject.Add(string.Concat(interval.Prefix, "TotalRecordCount"), dataSourceSummary.TotalRecordCount);
                dataSourceSummaryObject.Add(string.Concat(interval.Prefix, "MaxBatchRecordCount"), dataSourceSummary.MaxRecordCount);
                dataSourceSummaryObject.Add(string.Concat(interval.Prefix, "MinBatchRecordCount"), dataSourceSummary.MinRecordCount);
                dataSourceSummaryObject.Add(string.Concat(interval.Prefix, "MaxBatchSize"), dataSourceSummary.MaxBatchSize);
                dataSourceSummaryObject.Add(string.Concat(interval.Prefix, "MinBatchSize"), dataSourceSummary.MinBatchSize);
                dataSourceSummaryObject.Add(string.Concat(interval.Prefix, "NbInvalidBatches"), dataSourceSummary.NbInvalidBatch);
                dataSourceSummaryObject.Add(string.Concat(interval.Prefix, "NbEmptyBatches"), dataSourceSummary.NbEmptyBatch);
                dataSourceSummaryObject.Add(string.Concat(interval.Prefix, "NbOfMinutesSinceLastBatch"), now.Subtract(dataSourceSummary.LastImportedBatchTime).TotalMinutes);

            }

            return new DataRecordObject(new Guid("3BD1FED8-44C7-4F33-93FD-D8276FBE07AD"), dataSourceSummaryObject);
        }

        private struct DSAnalysisInterval
        {
            public string Prefix { get; set; }
            public int NbOfHours { get; set; }
        }
    }
}