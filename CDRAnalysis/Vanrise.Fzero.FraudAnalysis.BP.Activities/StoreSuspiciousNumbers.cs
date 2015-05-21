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
            handle.SharedInstanceData.WriteTrackingMessage(BusinessProcess.Entities.BPTrackingSeverity.Information, "Start StoreSuspiciousNumbers.DoWork.Start {0}", DateTime.Now);
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
                            handle.SharedInstanceData.WriteTrackingMessage(BusinessProcess.Entities.BPTrackingSeverity.Information, "End StoreSuspiciousNumbers.DoWork.DequeuedSaveSuspiciousNumbers {0}", DateTime.Now);
                            dataManager.SaveSuspiciousNumbers(x.suspiciousNumbers);
                            handle.SharedInstanceData.WriteTrackingMessage(BusinessProcess.Entities.BPTrackingSeverity.Information, "End StoreSuspiciousNumbers.DoWork.SavedtoDBSaveSuspiciousNumbers {0}", DateTime.Now);
                        });

                    hasNumberProfiles = inputArgument.InputQueue2.TryDequeue(
                       (y) =>
                       {
                           handle.SharedInstanceData.WriteTrackingMessage(BusinessProcess.Entities.BPTrackingSeverity.Information, "End StoreSuspiciousNumbers.DoWork.DequeuedSaveNumberProfiles {0}", DateTime.Now);
                           dataManager.SaveNumberProfiles(y.numberProfiles);
                           handle.SharedInstanceData.WriteTrackingMessage(BusinessProcess.Entities.BPTrackingSeverity.Information, "End StoreSuspiciousNumbers.DoWork.SavedtoDBSaveNumberProfiles {0}", DateTime.Now);
                       });
                }
                while (!ShouldStop(handle) && hasItemSuspiciousNumbers && hasNumberProfiles);
            });
            handle.SharedInstanceData.WriteTrackingMessage(BusinessProcess.Entities.BPTrackingSeverity.Information, "End StoreSuspiciousNumbers.DoWork.End {0}", DateTime.Now);
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
