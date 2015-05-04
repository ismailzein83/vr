using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess;
using Vanrise.Fzero.FraudAnalysis.Data;
using Vanrise.Fzero.FraudAnalysis.Entities;
using Vanrise.Queueing;


namespace Vanrise.Fzero.FraudAnalysis.BP.Activities
{
    #region Arguments Classes

    public class GetStoreSuspiciousNumbersInput
    {
        public BaseQueue<SuspiciousNumberBatch> InputQueue { get; set; }

        public Strategy strategy { get; set; }

    }

    #endregion

    public class StoreSuspiciousNumbers : DependentAsyncActivity<GetStoreSuspiciousNumbersInput>
    {
        #region Arguments

        [RequiredArgument]
        public InArgument<BaseQueue<SuspiciousNumberBatch>> InputQueue { get; set; }


        [RequiredArgument]
        public InArgument<Strategy> strategy { get; set; }

        #endregion


        protected override void DoWork(GetStoreSuspiciousNumbersInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            handle.SharedInstanceData.WriteTrackingMessage(BusinessProcess.Entities.BPTrackingSeverity.Information, "Start StoreSuspiciousNumbers.DoWork.Start {0}", DateTime.Now);
            ISuspiciousNumberDataManager dataManager = FraudDataManagerFactory.GetDataManager<ISuspiciousNumberDataManager>();
            
            DoWhilePreviousRunning(previousActivityStatus, handle, () =>
            {
                bool hasItem = false;
                do
                {
                    hasItem = inputArgument.InputQueue.TryDequeue(
                        (item) =>
                        {
                            handle.SharedInstanceData.WriteTrackingMessage(BusinessProcess.Entities.BPTrackingSeverity.Information, "End StoreSuspiciousNumbers.DoWork.Dequeued {0}", DateTime.Now);
                            dataManager.SaveSuspiciousNumbers(item.suspiciousNumbers, inputArgument.strategy);
                            handle.SharedInstanceData.WriteTrackingMessage(BusinessProcess.Entities.BPTrackingSeverity.Information, "End StoreSuspiciousNumbers.DoWork.SavedtoDB {0}", DateTime.Now);
                        });
                }
                while (!ShouldStop(handle) && hasItem);
            });
            handle.SharedInstanceData.WriteTrackingMessage(BusinessProcess.Entities.BPTrackingSeverity.Information, "End StoreSuspiciousNumbers.DoWork.End {0}", DateTime.Now);
        }

        protected override GetStoreSuspiciousNumbersInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new GetStoreSuspiciousNumbersInput
            {
                InputQueue = this.InputQueue.Get(context),
                strategy = this.strategy.Get(context)
            };
        }
    }
}
