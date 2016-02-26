using System;
using System.Activities;
using Vanrise.BusinessProcess;
using Vanrise.Common;
using Vanrise.Fzero.FraudAnalysis.Business;
using Vanrise.Fzero.FraudAnalysis.Entities;
using Vanrise.Queueing;

namespace Vanrise.Fzero.FraudAnalysis.BP.Activities
{

    #region Arguments Classes

    public class AssignCasesInput
    {
        public BaseQueue<StrategyExecutionItemSummaryBatch> InputQueue { get; set; }
    }

    #endregion

    public class AssignCases : DependentAsyncActivity<AssignCasesInput>
    {

        #region Arguments

        [RequiredArgument]
        public InArgument<BaseQueue<StrategyExecutionItemSummaryBatch>> InputQueue { get; set; }

        #endregion

        protected override void DoWork(AssignCasesInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {

            handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, "Started Assigning Cases ");

            AccountCaseManager manager = new AccountCaseManager();

            DoWhilePreviousRunning(previousActivityStatus, handle, () =>
            {
                int index = 0;
                int totalIndex = 0;

                bool hasItem = false;
                do
                {

                    hasItem = inputArgument.InputQueue.TryDequeue(
                        (strategyExecutionItemSummaryBatch) =>
                        {

                            index++;
                            totalIndex++;
                            if (index == 1000)
                            {
                                Console.WriteLine("{0} Accounts Dequeued", totalIndex);
                                index = 0;
                            }

                            foreach (var strategyExecutionItemSummary in strategyExecutionItemSummaryBatch.StrategyExecutionItemSummaries)
                                manager.AssignAccountCase(strategyExecutionItemSummary.AccountNumber, strategyExecutionItemSummary.IMEIs);
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
