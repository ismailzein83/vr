using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestCallAnalysis.Entities;
using Vanrise.BusinessProcess;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;
using Vanrise.Queueing;

namespace TestCallAnalysis.BP.Activities
{
    public class AssignCasesProcessInput
    {
        public MemoryQueue<CDRCaseBatch> InputQueue { get; set; }
    }

    public class AssignCasesProcessOutput
    {

    }
    public class AssignCasesProcess : BaseAsyncActivity<AssignCasesProcessInput, AssignCasesProcessOutput>
    {
        [RequiredArgument]
        public InArgument<MemoryQueue<CDRCaseBatch>> InputQueue { get; set; }

        protected override AssignCasesProcessOutput DoWorkWithResult(AssignCasesProcessInput inputArgument, AsyncActivityHandle handle)
        {
            bool hasItems = false;
            do
            {
                if (inputArgument.InputQueue != null && inputArgument.InputQueue.Count > 0)
                {
                    hasItems = inputArgument.InputQueue.TryDequeue((recordBatch) =>
                    {

                    });
                };
            } while (!ShouldStop(handle) && hasItems);
            return new AssignCasesProcessOutput();
        }

        protected override AssignCasesProcessInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new AssignCasesProcessInput()
            {
                InputQueue = this.InputQueue.Get(context)
            };
        }

        protected override void OnWorkComplete(AsyncCodeActivityContext context, AssignCasesProcessOutput result)
        {

        }
    }
}
