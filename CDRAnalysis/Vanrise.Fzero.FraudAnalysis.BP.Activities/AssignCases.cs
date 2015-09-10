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

        protected override void OnBeforeExecute(AsyncCodeActivityContext context, AsyncActivityHandle handle)
        {
            base.OnBeforeExecute(context, handle);
        }

        protected override void DoWork(AssignCasesInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            ICaseManagementDataManager dataManager = FraudDataManagerFactory.GetDataManager<ICaseManagementDataManager>();

                int cdrsCount = 0;
                DoWhilePreviousRunning(previousActivityStatus, handle, () =>
                {
                    bool hasItem = false;
                    do
                    {

                        hasItem = inputArgument.InputQueue.TryDequeue(
                            (accountNumberBatch) =>
                            {
                                var serializedNumbers = Vanrise.Common.Compressor.Decompress(System.IO.File.ReadAllBytes(accountNumberBatch.AccountNumberBatchFilePath));
                                System.IO.File.Delete(accountNumberBatch.AccountNumberBatchFilePath);
                                var numbers = Vanrise.Common.ProtoBufSerializer.Deserialize<List<string>>(serializedNumbers);
                                foreach (var number in numbers)
                                {
                                    dataManager.UpdateAccountCase(number, CaseStatus.Open, null);
                                }
                                cdrsCount += numbers.Count;
                                handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Verbose, "{0} Cases assigned", cdrsCount);

                            });
                    }
                    while (!ShouldStop(handle) && hasItem);
                });
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
