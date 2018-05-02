using System;
using System.Collections.Generic;
using System.Activities;
using Vanrise.BusinessProcess;
using Vanrise.Reprocess.Entities;
using Vanrise.Common;
using Vanrise.Entities;
using Vanrise.GenericData.Entities;

namespace Vanrise.Reprocess.BP.Activities
{
    public class CommitChangesInput
    {
        public ReprocessStage Stage { get; set; }

        public Dictionary<string, object> InitializationOutputByStage { get; set; }

        public DateTime From { get; set; }

        public DateTime To { get; set; }

        public int RecordCountPerTransaction { get; set; }

        public ReprocessDefinition ReprocessDefinition { get; set; }

        public ReprocessFilter ReprocessFilter { get; set; }
    }

    public class CommitChangesOutput
    {
    }

    public sealed class CommitChanges : BaseAsyncActivity<CommitChangesInput, CommitChangesOutput>
    {
        [RequiredArgument]
        public InArgument<ReprocessStage> Stage { get; set; }

        [RequiredArgument]
        public InArgument<Dictionary<string, object>> InitializationOutputByStage { get; set; }

        [RequiredArgument]
        public InArgument<DateTime> From { get; set; }

        [RequiredArgument]
        public InArgument<DateTime> To { get; set; }

        [RequiredArgument]
        public InArgument<int> RecordCountPerTransaction { get; set; }

        [RequiredArgument]
        public InArgument<ReprocessDefinition> ReprocessDefinition { get; set; }

        [RequiredArgument]
        public InArgument<ReprocessFilter> ReprocessFilter { get; set; }

        protected override CommitChangesInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new CommitChangesInput()
            {
                Stage = this.Stage.Get(context),
                InitializationOutputByStage = this.InitializationOutputByStage.Get(context),
                From = this.From.Get(context),
                To = this.To.Get(context),
                RecordCountPerTransaction = this.RecordCountPerTransaction.Get(context),
                ReprocessFilter = this.ReprocessFilter.Get(context),
                ReprocessDefinition = this.ReprocessDefinition.Get(context)
            };
        }

        protected override CommitChangesOutput DoWorkWithResult(CommitChangesInput inputArgument, AsyncActivityHandle handle)
        {
            handle.SharedInstanceData.WriteTrackingMessage(Vanrise.Entities.LogEntryType.Information, "Starting Commit Changes for stage {0}", inputArgument.Stage.StageName);

            object initializationStageOutput = inputArgument.InitializationOutputByStage.GetRecord(inputArgument.Stage.StageName);

            var getStorageRowCountContext = new ReprocessStageActivatorGetStorageRowCountContext(initializationStageOutput);
            int? storageRowCount = inputArgument.Stage.Activator.GetStorageRowCount(getStorageRowCountContext);

            if (storageRowCount.HasValue)
            {
                int batchCount = (storageRowCount.Value > 0) ? ((int)Math.Ceiling((double)storageRowCount.Value / inputArgument.RecordCountPerTransaction)) : 1;
                TimeSpan reprocessDuration = inputArgument.To.Subtract(inputArgument.From);
                TimeSpan batchDuration = TimeSpan.FromMinutes(reprocessDuration.TotalMinutes / batchCount);

                //batchDuration = new TimeSpan(batchDuration.Ticks - (batchDuration.Ticks % 10000000)); //To Remove Milliseconds and Ticks
                batchDuration = new TimeSpan(batchDuration.Days, batchDuration.Hours, batchDuration.Minutes, batchDuration.Seconds); //To Remove Milliseconds and Ticks

                IEnumerable<DateTimeRange> dateTimeRanges = Vanrise.Common.Utilities.GenerateDateTimeRanges(inputArgument.From, inputArgument.To, batchDuration);

                foreach (var dateTimeRange in dateTimeRanges)
                {
                    if (ShouldStop(handle))
                        break;

                    //handle.SharedInstanceData.WriteTrackingMessage(Vanrise.Entities.LogEntryType.Information, "Starting Commit Changes for stage {0} Batch Start: {1}, Batch End : {2}",
                    //    inputArgument.Stage.StageName, dateTimeRange.From.ToString("yyyy-MM-dd HH:mm:ss"), dateTimeRange.To.ToString("yyyy-MM-dd HH:mm:ss"));

                    var commitChangesContext = new ReprocessStageActivatorCommitChangesContext(initializationStageOutput, dateTimeRange.From, dateTimeRange.To, inputArgument.ReprocessFilter, inputArgument.ReprocessDefinition);
                    inputArgument.Stage.Activator.CommitChanges(commitChangesContext);

                    //handle.SharedInstanceData.WriteTrackingMessage(Vanrise.Entities.LogEntryType.Information, "Finishing Commit Changes for stage {0} Batch Start: {1}, Batch End : {2}",
                    //    inputArgument.Stage.StageName, dateTimeRange.From.ToString("yyyy-MM-dd HH:mm:ss"), dateTimeRange.To.ToString("yyyy-MM-dd HH:mm:ss"));
                }

                var initializatingContext = new ReprocessStageActivatorDropStorageContext(initializationStageOutput);
                inputArgument.Stage.Activator.DropStorage(initializatingContext);
            }

            handle.SharedInstanceData.WriteTrackingMessage(Vanrise.Entities.LogEntryType.Information, "Finished Commit Changes for stage {0} ", inputArgument.Stage.StageName);
            return new CommitChangesOutput();
        }

        protected override void OnWorkComplete(AsyncCodeActivityContext context, CommitChangesOutput result)
        {
        }

        #region Private Classes

        private class ReprocessStageActivatorGetStorageRowCountContext : IReprocessStageActivatorGetStorageRowCountContext
        {
            object _initializationStageOutput;

            public ReprocessStageActivatorGetStorageRowCountContext(object initializationStageOutput)
            {
                _initializationStageOutput = initializationStageOutput;
            }

            public object InitializationStageOutput { get { return _initializationStageOutput; } }
        }

        private class ReprocessStageActivatorCommitChangesContext : IReprocessStageActivatorCommitChangesContext
        {
            object _initializationStageOutput;
            DateTime _from;
            DateTime _to;

            ReprocessFilter _reprocessFilter;
            ReprocessDefinition _reprocessDefinition;

            public ReprocessStageActivatorCommitChangesContext(object initializationStageOutput, DateTime from, DateTime to, ReprocessFilter reprocessFilter, ReprocessDefinition reprocessDefinition)
            {
                _initializationStageOutput = initializationStageOutput;
                _from = from;
                _to = to;
                _reprocessFilter = reprocessFilter;
                _reprocessDefinition = reprocessDefinition;
            }

            public object InitializationStageOutput { get { return _initializationStageOutput; } }

            public DateTime From { get { return _from; } }

            public DateTime To { get { return _to; } }

            public RecordFilterGroup GetRecordFilterGroup(Guid? dataRecordTypeId)
            {
                RecordFilterGroup recordFilterGroup = null;
                if (_reprocessFilter != null && _reprocessDefinition.Settings.FilterDefinition != null)
                    recordFilterGroup = _reprocessDefinition.Settings.FilterDefinition.GetFilterGroup(new ReprocessFilterGetFilterGroupContext() { ReprocessFilter = _reprocessFilter, TargetDataRecordTypeId = dataRecordTypeId });

                return recordFilterGroup;
            }
        }

        private class ReprocessStageActivatorDropStorageContext : IReprocessStageActivatorDropStorageContext
        {
            object _initializationStageOutput;

            public ReprocessStageActivatorDropStorageContext(object initializationStageOutput)
            {
                _initializationStageOutput = initializationStageOutput;
            }

            public object InitializationStageOutput { get { return _initializationStageOutput; } }
        }

        #endregion
    }
}