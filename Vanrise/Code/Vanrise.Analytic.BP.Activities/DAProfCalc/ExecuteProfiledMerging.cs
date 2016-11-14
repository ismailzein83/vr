using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Vanrise.Queueing;
using Vanrise.Analytic.Entities;
using Vanrise.BusinessProcess;

namespace Vanrise.Analytic.BP.Activities.DAProfCalc
{
    #region Arguments

    public class ExecuteProfiledMergingInput
    {

    }

    public class ExecuteProfiledMergingOutput
    {

    }

    #endregion

    public sealed class ExecuteProfiledMerging : DependentAsyncActivity<ExecuteProfiledMergingInput, ExecuteProfiledMergingOutput>
    {
        [RequiredArgument]
        public InArgument<Guid> DAProfCalcDefinitionId { get; set; }

        [RequiredArgument]
        public InArgument<List<DAProfCalcExecInput>> DAProfCalcExecInputs { get; set; }

        [RequiredArgument]
        public InArgument<BaseQueue<DAProfCalcOutputRecordBatch>> InputQueue { get; set; }

        [RequiredArgument]
        public InOutArgument<BaseQueue<DAProfCalcOutputRecordBatch>> OutputQueue { get; set; }

        protected override ExecuteProfiledMergingOutput DoWorkWithResult(ExecuteProfiledMergingInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            throw new NotImplementedException();
        }

        protected override ExecuteProfiledMergingInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            throw new NotImplementedException();
        }

        protected override void OnWorkComplete(AsyncCodeActivityContext context, ExecuteProfiledMergingOutput result)
        {
            throw new NotImplementedException();
        }
    }
}
