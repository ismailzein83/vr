using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Vanrise.BusinessProcess;
using Vanrise.Reprocess.Entities;
using Vanrise.Entities;
using Vanrise.Common;

namespace Vanrise.Reprocess.BP.Activities
{
    public class ExecuteStageInput
    {
        public StageManager StageManager { get; set; }

        public ReprocessStage Stage { get; set; }

        public DateTime From { get; set; }

        public DateTime To { get; set; }

        public List<string> StageNames { get; set; }

        public long CurrentProcessId { get; set; }

        public Dictionary<string, object> InitializationOutputByStage { get; set; }
    }

    public class ExecuteStageOutput
    {
    }

    public sealed class ExecuteStage : DependentAsyncActivity<ExecuteStageInput, ExecuteStageOutput>
    {
        [RequiredArgument]
        public InArgument<StageManager> StageManager { get; set; }

        [RequiredArgument]
        public InArgument<ReprocessStage> Stage { get; set; }

        [RequiredArgument]
        public InArgument<DateTime> From { get; set; }

        [RequiredArgument]
        public InArgument<DateTime> To { get; set; }

        [RequiredArgument]
        public InArgument<List<string>> StageNames { get; set; }

        [RequiredArgument]
        public InArgument<Dictionary<string, object>> InitializationOutputByStage { get; set; }


        protected override ExecuteStageInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new ExecuteStageInput()
            {
                Stage = this.Stage.Get(context),
                StageManager = this.StageManager.Get(context),
                From = this.From.Get(context),
                To = this.To.Get(context),
                StageNames = this.StageNames.Get(context),
                CurrentProcessId = context.GetSharedInstanceData().InstanceInfo.ParentProcessID.HasValue ? context.GetSharedInstanceData().InstanceInfo.ParentProcessID.Value : context.GetSharedInstanceData().InstanceInfo.ProcessInstanceID,
                InitializationOutputByStage = this.InitializationOutputByStage.Get(context)
            };
        }

        protected override ExecuteStageOutput DoWorkWithResult(ExecuteStageInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            Dictionary<string, object> initializationOutputByStage = inputArgument.InitializationOutputByStage;

            object initializationStageOutput = initializationOutputByStage.GetRecord(inputArgument.Stage.StageName);

            var executionContext = new ReprocessStageActivatorExecutionContext(inputArgument.Stage.StageQueue,
                (actionToDo) => base.DoWhilePreviousRunning(previousActivityStatus, handle, actionToDo),
                (previousActivityStatus_Internal, actionToDo) => base.DoWhilePreviousRunning(previousActivityStatus_Internal, handle, actionToDo),
                () => base.ShouldStop(handle), inputArgument.StageManager.EnqueueBatch, inputArgument.From, inputArgument.To, inputArgument.StageNames,
                inputArgument.Stage.StageName, inputArgument.CurrentProcessId, (logEntryType, message) => handle.SharedInstanceData.WriteTrackingMessage(logEntryType, message, null), initializationStageOutput);

            inputArgument.Stage.Activator.ExecuteStage(executionContext);
            return new ExecuteStageOutput();
        }

        protected override void OnWorkComplete(AsyncCodeActivityContext context, ExecuteStageOutput result)
        {
            context.GetSharedInstanceData().WriteTrackingMessage(LogEntryType.Information, string.Format("{0} execution is done.", this.Stage.Get(context).StageName), null);
        }

        private class ReprocessStageActivatorExecutionContext : IReprocessStageActivatorExecutionContext
        {
            Queueing.BaseQueue<IReprocessBatch> _inputQueue;
            Action<Action> _doWhilePreviousRunningAction;
            Action<AsyncActivityStatus, Action> _doWhilePreviousRunningAction2;
            Func<bool> _shouldStopFunc;
            Action<string, IReprocessBatch> _enqueueItem;
            DateTime _from;
            DateTime _to;
            List<string> _stageNames;
            string _currentStageName;
            long _processInstanceId;
            Action<LogEntryType, string> _writeTrackingMessage;
            object _initializationStageOutput;


            public ReprocessStageActivatorExecutionContext(Queueing.BaseQueue<IReprocessBatch> inputQueue, Action<Action> doWhilePreviousRunningAction, Action<AsyncActivityStatus,
                Action> doWhilePreviousRunningAction2, Func<bool> shouldStopFunc, Action<string, IReprocessBatch> enqueueItem, DateTime from, DateTime to, List<string> stageNames,
                string currentStageName, long processInstanceId, Action<LogEntryType, string> writeTrackingMessage, object initializationStageOutput)
            {
                _inputQueue = inputQueue;
                _doWhilePreviousRunningAction = doWhilePreviousRunningAction;
                _doWhilePreviousRunningAction2 = doWhilePreviousRunningAction2;
                _shouldStopFunc = shouldStopFunc;
                _enqueueItem = enqueueItem;
                _from = from;
                _to = to;
                _stageNames = stageNames;
                _processInstanceId = processInstanceId;
                _currentStageName = currentStageName;
                _writeTrackingMessage = writeTrackingMessage;
                _initializationStageOutput = initializationStageOutput;
            }

            public Queueing.BaseQueue<IReprocessBatch> InputQueue
            {
                get { return _inputQueue; }
            }

            public void DoWhilePreviousRunning(Action actionToDo)
            {
                _doWhilePreviousRunningAction(actionToDo);
            }

            public void DoWhilePreviousRunning(AsyncActivityStatus previousActivityStatus, Action actionToDo)
            {
                _doWhilePreviousRunningAction2(previousActivityStatus, actionToDo);
            }

            public void WriteTrackingMessage(LogEntryType severity, string messageFormat)
            {
                _writeTrackingMessage(severity, messageFormat);
            }

            public bool ShouldStop()
            {
                return _shouldStopFunc();
            }

            public void EnqueueBatch(string stageName, IReprocessBatch batch)
            {
                _enqueueItem(stageName, batch);
            }

            public DateTime From
            {
                get { return _from; }
            }

            public DateTime To
            {
                get { return _to; }
            }

            public List<string> StageNames
            {
                get { return _stageNames; }
            }

            public string CurrentStageName
            {
                get { return _currentStageName; }
            }

            public long ProcessInstanceId
            {
                get { return _processInstanceId; }
            }

            public object InitializationStageOutput
            {
                get { return _initializationStageOutput; }
            }
        }
    }
}