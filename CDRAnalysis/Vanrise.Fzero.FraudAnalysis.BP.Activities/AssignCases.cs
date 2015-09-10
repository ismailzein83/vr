using System;
using System.Activities;
using System.Collections.Generic;
using Vanrise.BusinessProcess;
using Vanrise.Common;
using Vanrise.Fzero.CDRImport.Entities;
using Vanrise.Fzero.FraudAnalysis.Business;
using Vanrise.Fzero.FraudAnalysis.Data;
using Vanrise.Fzero.FraudAnalysis.Entities;
using Vanrise.Queueing;

namespace Vanrise.Fzero.FraudAnalysis.BP.Activities
{

    #region Arguments Classes

    public class AssignCasesInput
    {
        public BaseQueue<AccountNumberBatch> InputQueue { get; set; }
    }

    #endregion

    public class AssignCases : DependentAsyncActivity<AssignCasesInput>
    {

        #region Arguments

        [RequiredArgument]
        public InArgument<BaseQueue<AccountNumberBatch>> InputQueue { get; set; }

        #endregion

        protected override void DoWork(AssignCasesInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {

            handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, "Started Assigning Cases ");

            ICaseManagementDataManager dataManager = FraudDataManagerFactory.GetDataManager<ICaseManagementDataManager>();

            DoWhilePreviousRunning(previousActivityStatus, handle, () =>
            {
                int index = 0;
                int totalIndex = 0;

                bool hasItem = false;
                do
                {

                    hasItem = inputArgument.InputQueue.TryDequeue(
                        (accountNumberBatch) =>
                        {
                            var serializedNumbers = Vanrise.Common.Compressor.Decompress(System.IO.File.ReadAllBytes(accountNumberBatch.AccountNumberBatchFilePath));
                            System.IO.File.Delete(accountNumberBatch.AccountNumberBatchFilePath);
                            var number = Vanrise.Common.ProtoBufSerializer.Deserialize<string>(serializedNumbers);

                            index++;
                            totalIndex++;
                            if (index == 1000)
                            {
                                Console.WriteLine("{0} Accounts Dequeued", totalIndex);
                                index = 0;
                            }


                            dataManager.UpdateAccountCase(number, CaseStatus.Open, null, false);
                        });
                }
                while (!ShouldStop(handle) && hasItem);
            });

            handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, "Finshed Assigning Cases ");
        }

        protected override AssignCasesInput GetInputArgument2(System.Activities.AsyncCodeActivityContext context)
        {
            return new AssignCasesInput
            {
                InputQueue = this.InputQueue.Get(context)
            };
        }

    }
}
