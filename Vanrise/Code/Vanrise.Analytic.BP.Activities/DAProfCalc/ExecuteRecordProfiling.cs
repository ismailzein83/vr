using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Vanrise.BusinessProcess;
using Vanrise.Queueing;
using Vanrise.GenericData.Entities;
using Vanrise.Analytic.Entities;

namespace Vanrise.Analytic.BP.Activities.DAProfCalc
{
    #region Arguments

    public class ExecuteRecordProfilingInput
    {
        public Guid DAProfCalcDefinitionId { get; set; }

        public List<DAProfCalcExecInput> DAProfCalcExecInputs { get; set; }

        public BaseQueue<RecordBatch> InputQueue { get; set; }

        public List<BaseQueue<DAProfCalcOutputRecordBatch>> OutputQueues { get; set; }

        public IDAProfCalcOutputRecordProcessor OutputRecordProcessor { get; set; }
    }

    public class ExecuteRecordProfilingOutput
    {

    }

    #endregion

    public sealed class ExecuteRecordProfiling : DependentAsyncActivity<ExecuteRecordProfilingInput, ExecuteRecordProfilingOutput>
    {
        [RequiredArgument]
        public InArgument<Guid> DAProfCalcDefinitionId { get; set; }

        [RequiredArgument]
        public InArgument<List<DAProfCalcExecInput>> DAProfCalcExecInputs { get; set; }

        [RequiredArgument]
        public InArgument<BaseQueue<RecordBatch>> InputQueue { get; set; }

        [RequiredArgument]
        public InArgument<List<BaseQueue<DAProfCalcOutputRecordBatch>>> OutputQueues { get; set; }

        public InArgument<IDAProfCalcOutputRecordProcessor> OutputRecordProcessor { get; set; }

        protected override ExecuteRecordProfilingOutput DoWorkWithResult(ExecuteRecordProfilingInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            ExecuteRecordProfilingOutput output = new ExecuteRecordProfilingOutput();
            bool hasItem = false;
            DoWhilePreviousRunning(previousActivityStatus, handle, () =>
            {
                do
                {
                    hasItem = inputArgument.InputQueue.TryDequeue((recordBatch) =>
                    {
                        
                    });
                }
                while (!ShouldStop(handle) && hasItem);
            });
            return output;
        }

        protected override ExecuteRecordProfilingInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new ExecuteRecordProfilingInput()
            {
                DAProfCalcDefinitionId = DAProfCalcDefinitionId.Get(context),
                DAProfCalcExecInputs = DAProfCalcExecInputs.Get(context),
                InputQueue = InputQueue.Get(context),
                OutputQueues = OutputQueues.Get(context),
                OutputRecordProcessor = OutputRecordProcessor.Get(context)
            };
        }

        protected override void OnWorkComplete(AsyncCodeActivityContext context, ExecuteRecordProfilingOutput result)
        {
            //throw new NotImplementedException();
        }
    }
}
