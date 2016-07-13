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
    { public StageManager StageManager { get; set; }
                
        public ReprocessStage Stage { get; set; }
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

        protected override ExecuteStageInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            throw new NotImplementedException();
        }

        protected override ExecuteStageOutput DoWorkWithResult(ExecuteStageInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            var executionContext = new ReprocessStageActivatorExecutionContext(inputArgument.Stage.StageQueue,
                (actionToDo) => base.DoWhilePreviousRunning(previousActivityStatus, handle, actionToDo),
                (previousActivityStatus_Internal, actionToDo) => base.DoWhilePreviousRunning(previousActivityStatus_Internal, handle, actionToDo),
                () => base.ShouldStop(handle));

            inputArgument.Stage.Activator.ExecuteStage(executionContext);
            return new ExecuteStageOutput();
        }

        protected override void OnWorkComplete(AsyncCodeActivityContext context, ExecuteStageOutput result)
        {
            throw new NotImplementedException();
        }

        private class ReprocessStageActivatorExecutionContext : IReprocessStageActivatorExecutionContext
        {
            Queueing.BaseQueue<IReprocessBatch> _inputQueue;
            Action <Action> _doWhilePreviousRunningAction;
            Action<AsyncActivityStatus, Action> _doWhilePreviousRunningAction2;
            Func<bool> _shouldStopFunc;

            public ReprocessStageActivatorExecutionContext(Queueing.BaseQueue<IReprocessBatch> inputQueue, Action<Action> doWhilePreviousRunningAction, Action<AsyncActivityStatus, Action> doWhilePreviousRunningAction2, Func<bool> shouldStopFunc)
            {
                _inputQueue = inputQueue;
                _doWhilePreviousRunningAction = doWhilePreviousRunningAction;
                _doWhilePreviousRunningAction2 = doWhilePreviousRunningAction2;
                _shouldStopFunc = shouldStopFunc;
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
        }

    }

    
}
