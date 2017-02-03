using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Mediation.Generic.Entities;
using Vanrise.Queueing;
using Vanrise.BusinessProcess;
using Vanrise.Entities;
using Vanrise.Data;

namespace Mediation.Generic.BP.Activities
{
    #region Arguments

    public class ExecuteOutputInput
    {
        public OutputHandlerExecutionEntity OutputHandlerExecutionEntity { get; set; }
        public MediationDefinition MediationDefinition { get; set; }
    }

    public class ExecuteOutputOutput
    {
    }

    #endregion

    public sealed class ExecuteOutputHandler : DependentAsyncActivity<ExecuteOutputInput, ExecuteOutputOutput>
    {
        [RequiredArgument]
        public InArgument<MediationDefinition> MediationDefinition { get; set; }
        [RequiredArgument]
        public InArgument<OutputHandlerExecutionEntity> OutputHandlerExecutionEntity { get; set; }

        protected override ExecuteOutputOutput DoWorkWithResult(ExecuteOutputInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            var executionContext = new MediationOutputHandlerContext(this, previousActivityStatus, handle, inputArgument.MediationDefinition, inputArgument.OutputHandlerExecutionEntity.InputQueue);
            inputArgument.OutputHandlerExecutionEntity.OutputHandler.Handler.Execute(executionContext);

            return new ExecuteOutputOutput { };
        }


        protected override ExecuteOutputInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new ExecuteOutputInput
            {
                OutputHandlerExecutionEntity = this.OutputHandlerExecutionEntity.Get(context),
                MediationDefinition = this.MediationDefinition.Get(context)
            };
        }
        protected override void OnWorkComplete(AsyncCodeActivityContext context, ExecuteOutputOutput result)
        {

        }

        #region Private Classes

        private class MediationOutputHandlerContext : IMediationOutputHandlerContext
        {
            ExecuteOutputHandler _parentActivity;
            AsyncActivityStatus _previousActivityStatus;
            AsyncActivityHandle _handle;
            MediationDefinition _mediationDefinition;
            BaseQueue<PreparedCdrBatch> _inputQueue;

            public MediationOutputHandlerContext(ExecuteOutputHandler parentActivity, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle,
                MediationDefinition mediationDefinition, BaseQueue<PreparedCdrBatch> inputQueue)
            {
                _parentActivity = parentActivity;
                _previousActivityStatus = previousActivityStatus;
                _handle = handle;
                _mediationDefinition = mediationDefinition;
                _inputQueue = inputQueue;
            }

            public MediationDefinition MediationDefinition
            {
                get
                {
                    return _mediationDefinition;
                }
            }

            public BaseQueue<PreparedCdrBatch> InputQueue
            {
                get { return _inputQueue; }
            }

            public void DoWhilePreviousRunning(Action actionToDo)
            {
                _parentActivity.DoWhilePreviousRunning(_previousActivityStatus, _handle, actionToDo);
            }

            public void DoWhilePreviousRunning(AsyncActivityStatus previousActivityStatus, Action actionToDo)
            {
                _parentActivity.DoWhilePreviousRunning(previousActivityStatus, _handle, actionToDo);
            }

            public void WriteTrackingMessage(LogEntryType severity, string messageFormat, params object[] args)
            {
                _handle.SharedInstanceData.WriteTrackingMessage(severity, messageFormat, args);
            }

            public bool ShouldStop()
            {
                return _parentActivity.ShouldStop(_handle);
            }

            public long ProcessInstanceId
            {
                get { return _handle.SharedInstanceData.InstanceInfo.ProcessInstanceID; }
            }

            public void PrepareDataForDBApply<R, S>(IBulkApplyDataManager<R> dataManager, BaseQueue<S> inputQueue, BaseQueue<object> outputQueue, Func<S, System.Collections.Generic.IEnumerable<R>> GetItems)
            {
                _parentActivity.PrepareDataForDBApply(_previousActivityStatus, _handle, dataManager, inputQueue, outputQueue, GetItems);
            }
        }

        #endregion

        
    }
}
