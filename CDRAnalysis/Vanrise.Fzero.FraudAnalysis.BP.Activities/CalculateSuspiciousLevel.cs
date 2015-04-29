using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess;
using Vanrise.Fzero.FraudAnalysis.Business;
using Vanrise.Fzero.FraudAnalysis.Entities;
using Vanrise.Queueing;

namespace Vanrise.Fzero.FraudAnalysis.BP.Activities
{
    #region Arguments Classes

    public class CalculateSuspiciousLevelInput
    {
        public BaseQueue<NumberCriteriaBatch> InputQueue { get; set; }

        public BaseQueue<SuspiciousNumberBatch> OutputQueue { get; set; }
    }

    #endregion

    public class CalculateSuspiciousLevel : DependentAsyncActivity<CalculateSuspiciousLevelInput>
    {

        #region Arguments

        [RequiredArgument]
        public InArgument<BaseQueue<NumberCriteriaBatch>> InputQueue { get; set; }

        public InOutArgument<BaseQueue<SuspiciousNumberBatch>> OutputQueue { get; set; }

        #endregion

        protected override void DoWork(CalculateSuspiciousLevelInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
           // FraudManager manager = new FraudManager();

            DoWhilePreviousRunning(previousActivityStatus, handle, () =>
            {
                bool hasItem = false;
                do
                {
                    hasItem = inputArgument.InputQueue.TryDequeue(
                        (item) =>
                        {
                            
                        });
                }
                while (!ShouldStop(handle) && hasItem);
            });
        }

        protected override CalculateSuspiciousLevelInput GetInputArgument2(System.Activities.AsyncCodeActivityContext context)
        {
            return new CalculateSuspiciousLevelInput()
            {
                InputQueue = this.InputQueue.Get(context),
                OutputQueue = this.OutputQueue.Get(context)
            };
        }
    }
}
