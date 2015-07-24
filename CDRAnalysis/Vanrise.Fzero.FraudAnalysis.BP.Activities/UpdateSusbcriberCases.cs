using System;
using System.Activities;
using System.Collections.Generic;
using Vanrise.BusinessProcess;
using Vanrise.Fzero.FraudAnalysis.Data;
using Vanrise.Fzero.FraudAnalysis.Entities;
using Vanrise.Queueing;


namespace Vanrise.Fzero.FraudAnalysis.BP.Activities
{
    #region Arguments Classes

    public class GetStoreSuspiciousNumbersforCasesInput
    {
        public BaseQueue<SuspiciousNumberBatch> InputQueue { get; set; }

        public BaseQueue<NumberProfileBatch> InputQueue2 { get; set; }

        public List<Strategy> Strategies { get; set; }

    }

    #endregion

    public class UpdateSusbcriberCases : DependentAsyncActivity<GetStoreSuspiciousNumbersforCasesInput>
    {
        #region Arguments

        [RequiredArgument]
        public InArgument<BaseQueue<SuspiciousNumberBatch>> InputQueue { get; set; }

        #endregion


        protected override void DoWork(GetStoreSuspiciousNumbersforCasesInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            handle.SharedInstanceData.WriteTrackingMessage(BusinessProcess.Entities.BPTrackingSeverity.Information, "Started Storing Suspicious Numbers to Database");
            ISuspiciousNumberDataManager dataManager = FraudDataManagerFactory.GetDataManager<ISuspiciousNumberDataManager>();
            
            DoWhilePreviousRunning(previousActivityStatus, handle, () =>
            {
                bool hasItemSuspiciousNumbers = false;
                do
                {
                    hasItemSuspiciousNumbers = inputArgument.InputQueue.TryDequeue(
                        (x) =>
                        {
                            dataManager.UpdateSusbcriberCases(x.suspiciousNumbers);
                        });
                }
                while (!ShouldStop(handle) && hasItemSuspiciousNumbers);
            });
            handle.SharedInstanceData.WriteTrackingMessage(BusinessProcess.Entities.BPTrackingSeverity.Information, "Finished Storing Suspicious Numbers to Database");
        }

        protected override GetStoreSuspiciousNumbersforCasesInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new GetStoreSuspiciousNumbersforCasesInput
            {
                InputQueue = this.InputQueue.Get(context)
            };
        }
    }
}
