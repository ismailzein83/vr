using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Vanrise.BusinessProcess;
using Vanrise.Reprocess.Entities;

namespace Vanrise.Reprocess.BP.Activities
{
    public class ExecuteStageInput
    {
        public StageManager StageManager { get; set; }

        public ReprocessStage Stage { get; set; }

        public DateTime From { get; set; }

        public DateTime To { get; set; }

        public List<string> StageNames { get; set; }
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


        protected override ExecuteStageInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new ExecuteStageInput()
            {
                Stage = this.Stage.Get(context),
                StageManager = this.StageManager.Get(context),
                From = this.From.Get(context),
                To = this.To.Get(context),
                StageNames = this.StageNames.Get(context)
            };
        }

        protected override ExecuteStageOutput DoWorkWithResult(ExecuteStageInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            var executionContext = new ReprocessStageActivatorExecutionContext(inputArgument.Stage.StageQueue,
                (actionToDo) => base.DoWhilePreviousRunning(previousActivityStatus, handle, actionToDo),
                (previousActivityStatus_Internal, actionToDo) => base.DoWhilePreviousRunning(previousActivityStatus_Internal, handle, actionToDo),
                () => base.ShouldStop(handle), inputArgument.StageManager.EnqueueBatch, inputArgument.From, inputArgument.To, inputArgument.StageNames);

            inputArgument.Stage.Activator.ExecuteStage(executionContext);
            return new ExecuteStageOutput();
        }

        protected override void OnWorkComplete(AsyncCodeActivityContext context, ExecuteStageOutput result)
        {

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

            public ReprocessStageActivatorExecutionContext(Queueing.BaseQueue<IReprocessBatch> inputQueue, Action<Action> doWhilePreviousRunningAction, Action<AsyncActivityStatus, Action> doWhilePreviousRunningAction2, Func<bool> shouldStopFunc, Action<string, IReprocessBatch> enqueueItem, DateTime from, DateTime to, List<string> stageNames)
            {
                _inputQueue = inputQueue;
                _doWhilePreviousRunningAction = doWhilePreviousRunningAction;
                _doWhilePreviousRunningAction2 = doWhilePreviousRunningAction2;
                _shouldStopFunc = shouldStopFunc;
                _enqueueItem = enqueueItem;
                _from = from;
                _to = to;
                _stageNames = stageNames;
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
        }
    }
}