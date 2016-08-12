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
        public InArgument<List<Guid>> IncludedDAProfCalcItemDefinitionIds { get; set; }

        [RequiredArgument]
        public InArgument<BaseQueue<RecordBatch>> InputQueue { get; set; }

        [RequiredArgument]
        public InArgument<List<BaseQueue<DAProfCalcOutputRecordBatch>>> OutputQueues { get; set; }
        
        protected override ExecuteRecordProfilingOutput DoWorkWithResult(ExecuteRecordProfilingInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            throw new NotImplementedException();
        }

        protected override ExecuteRecordProfilingInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            throw new NotImplementedException();
        }

        protected override void OnWorkComplete(AsyncCodeActivityContext context, ExecuteRecordProfilingOutput result)
        {
            throw new NotImplementedException();
        }
    }
}
