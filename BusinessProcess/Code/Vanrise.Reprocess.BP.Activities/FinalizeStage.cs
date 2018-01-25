using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Vanrise.BusinessProcess;
using Vanrise.Reprocess.Entities;
using Vanrise.Entities;
using Vanrise.Common;
using Vanrise.GenericData.Entities;

namespace Vanrise.Reprocess.BP.Activities
{
    public class FinalizeStageInput
    {
        public IReprocessStageActivator StageActivator { get; set; }

        public string StageName { get; set; }

        public BatchRecord BatchRecord { get; set; }

        public long CurrentProcessId { get; set; }

        public Dictionary<string, object> InitializationOutputByStage { get; set; }

        public ReprocessDefinition ReprocessDefinition { get; set; }

        public ReprocessFilter ReprocessFilter { get; set; }
    }

    public class FinalizeStageOutput
    {
    }

    public sealed class FinalizeStage : DependentAsyncActivity<FinalizeStageInput, FinalizeStageOutput>
    {
        [RequiredArgument]
        public InArgument<IReprocessStageActivator> StageActivator { get; set; }

        [RequiredArgument]
        public InArgument<string> StageName { get; set; }

        [RequiredArgument]
        public InArgument<BatchRecord> BatchRecord { get; set; }

        [RequiredArgument]
        public InArgument<Dictionary<string, object>> InitializationOutputByStage { get; set; }

        [RequiredArgument]
        public InArgument<ReprocessDefinition> ReprocessDefinition { get; set; }

        [RequiredArgument]
        public InArgument<ReprocessFilter> ReprocessFilter { get; set; }

        protected override FinalizeStageInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new FinalizeStageInput()
            {
                StageActivator = this.StageActivator.Get(context),
                StageName = this.StageName.Get(context),
                BatchRecord = this.BatchRecord.Get(context),
                CurrentProcessId = context.GetSharedInstanceData().InstanceInfo.ParentProcessID.HasValue ? context.GetSharedInstanceData().InstanceInfo.ParentProcessID.Value : context.GetSharedInstanceData().InstanceInfo.ProcessInstanceID,
                InitializationOutputByStage = this.InitializationOutputByStage.Get(context),
                ReprocessFilter = this.ReprocessFilter.Get(context),
                ReprocessDefinition = this.ReprocessDefinition.Get(context)
            };
        }

        protected override FinalizeStageOutput DoWorkWithResult(FinalizeStageInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            Dictionary<string, object> initializationOutputByStage = inputArgument.InitializationOutputByStage;

            object initializationStageOutput = initializationOutputByStage.GetRecord(inputArgument.StageName);

            var executionContext = new ReprocessStageActivatorFinalizingContext(inputArgument.StageName, inputArgument.CurrentProcessId, inputArgument.BatchRecord,
                (logEntryType, message) => { handle.SharedInstanceData.WriteTrackingMessage(logEntryType, message, null); }, (previousActivityStatus_Internal, actionToDo) => base.DoWhilePreviousRunning(previousActivityStatus_Internal, handle, actionToDo), initializationStageOutput,
                inputArgument.ReprocessFilter, inputArgument.ReprocessDefinition);

            inputArgument.StageActivator.FinalizeStage(executionContext);
            return new FinalizeStageOutput();
        }

        protected override void OnWorkComplete(AsyncCodeActivityContext context, FinalizeStageOutput result)
        {

        }

        private class ReprocessStageActivatorFinalizingContext : IReprocessStageActivatorFinalizingContext
        {
            string _currentStageName;
            long _processInstanceId;
            BatchRecord _batchRecord;
            Action<LogEntryType, string> _writeTrackingMessage;
            Action<AsyncActivityStatus, Action> _doWhilePreviousRunningAction;
            ReprocessFilter _reprocessFilter;
            ReprocessDefinition _reprocessDefinition;
            object _initializationStageOutput;

            public ReprocessStageActivatorFinalizingContext(string currentStageName, long processInstanceId, BatchRecord batchRecord, Action<LogEntryType, string> writeTrackingMessage,
                Action<AsyncActivityStatus, Action> doWhilePreviousRunningAction, object initializationStageOutput, ReprocessFilter reprocessFilter, ReprocessDefinition reprocessDefinition)
            {
                _currentStageName = currentStageName;
                _processInstanceId = processInstanceId;
                _batchRecord = batchRecord;
                _writeTrackingMessage = writeTrackingMessage;
                _doWhilePreviousRunningAction = doWhilePreviousRunningAction;
                _initializationStageOutput = initializationStageOutput;
                _reprocessFilter = reprocessFilter;
                _reprocessDefinition = reprocessDefinition;
            }

            public long ProcessInstanceId
            {
                get { return _processInstanceId; }
            }

            public string CurrentStageName
            {
                get { return _currentStageName; }
            }

            public void WriteTrackingMessage(LogEntryType severity, string messageFormat)
            {
                _writeTrackingMessage(severity, messageFormat);
            }

            public void DoWhilePreviousRunning(AsyncActivityStatus previousActivityStatus, Action actionToDo)
            {
                _doWhilePreviousRunningAction(previousActivityStatus, actionToDo);
            }

            public BatchRecord BatchRecord
            {
                get { return _batchRecord; }
            }

            public object InitializationStageOutput
            {
                get { return _initializationStageOutput; }
            }

            public RecordFilterGroup GetRecordFilterGroup(Guid? dataRecordTypeId)
            {
                RecordFilterGroup recordFilterGroup = null;
                if (_reprocessFilter != null && _reprocessDefinition.Settings.FilterDefinition != null)
                    recordFilterGroup = _reprocessDefinition.Settings.FilterDefinition.GetFilterGroup(new ReprocessFilterGetFilterGroupContext() { ReprocessFilter = _reprocessFilter, TargetDataRecordTypeId = dataRecordTypeId });

                return recordFilterGroup;
            }
        }
    }
}