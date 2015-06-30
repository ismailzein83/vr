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

    public class GetStoreSuspiciousNumbersInput
    {
        public BaseQueue<SuspiciousNumberBatch> InputQueue { get; set; }

        public BaseQueue<NumberProfileBatch> InputQueue2 { get; set; }

        public List<Strategy> Strategies { get; set; }

    }

    #endregion

    public class StoreSuspiciousNumbers : DependentAsyncActivity<GetStoreSuspiciousNumbersInput>
    {
        #region Arguments

        [RequiredArgument]
        public InArgument<BaseQueue<SuspiciousNumberBatch>> InputQueue { get; set; }
        public InArgument<BaseQueue<NumberProfileBatch>> InputQueue2 { get; set; }


        [RequiredArgument]
        public InArgument<List<Strategy>> Strategies { get; set; }

        #endregion


        protected override void DoWork(GetStoreSuspiciousNumbersInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            handle.SharedInstanceData.WriteTrackingMessage(BusinessProcess.Entities.BPTrackingSeverity.Information, "Started Storing Suspicious Numbers to Database");
            ISuspiciousNumberDataManager dataManager = FraudDataManagerFactory.GetDataManager<ISuspiciousNumberDataManager>();
            
            DoWhilePreviousRunning(previousActivityStatus, handle, () =>
            {
                bool hasItemSuspiciousNumbers = false;
                bool hasNumberProfiles = false;
                do
                {
                    hasItemSuspiciousNumbers = inputArgument.InputQueue.TryDequeue(
                        (x) =>
                        {
                            dataManager.SaveSuspiciousNumbers(x.suspiciousNumbers);
                        });

                    hasNumberProfiles = inputArgument.InputQueue2.TryDequeue(
                       (y) =>
                       {
                           dataManager.SaveNumberProfiles(y.numberProfiles);
                       });
                }
                while (!ShouldStop(handle) && hasItemSuspiciousNumbers && hasNumberProfiles);
            });
            handle.SharedInstanceData.WriteTrackingMessage(BusinessProcess.Entities.BPTrackingSeverity.Information, "Finished Storing Suspicious Numbers to Database");
        }

        protected override GetStoreSuspiciousNumbersInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new GetStoreSuspiciousNumbersInput
            {
                InputQueue = this.InputQueue.Get(context),
                InputQueue2 = this.InputQueue2.Get(context),
                Strategies = this.Strategies.Get(context)
            };
        }
    }
}
