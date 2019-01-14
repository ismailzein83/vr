using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestCallAnalysis.Entities;
using Vanrise.BusinessProcess;
using Vanrise.Queueing;

namespace TestCallAnalysis.BP.Activities
{
    
    #region Arguments

    public class PrepareCDRCasesInput
    {
        public MemoryQueue<CDRCorrelationBatch> InputQueue { get; set; }
    }
    public class PrepareCDRCasesOutput
    {
    }

    #endregion
    public class PrepareCDRCases : DependentAsyncActivity<PrepareCDRCasesInput, PrepareCDRCasesOutput>
    {
        [RequiredArgument]
        public InOutArgument<MemoryQueue<CDRCorrelationBatch>> InputQueue { get; set; }
        protected override PrepareCDRCasesOutput DoWorkWithResult(PrepareCDRCasesInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
          
            DoWhilePreviousRunning(previousActivityStatus, handle, () =>
            {
                bool hasItem = false;
                do
                {
                    hasItem = inputArgument.InputQueue.TryDequeue((recordBatch) =>
                    {
                     
                    });
                } while (!ShouldStop(handle) && hasItem);
            });
            return new PrepareCDRCasesOutput();
        }

        protected override PrepareCDRCasesInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new PrepareCDRCasesInput()
            {
                InputQueue = this.InputQueue.Get(context),
            };
        }

        protected override void OnWorkComplete(AsyncCodeActivityContext context, PrepareCDRCasesOutput result)
        {
        }
    }
}
