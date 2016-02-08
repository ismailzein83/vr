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

    public class ApplyStrategyExecutionCancellationInput
    {
        public BaseQueue<CancellingStrategyExecutionBatch> InputQueue { get; set; }
    }

    #endregion

    public class ApplyStrategyExecutionCancellation : DependentAsyncActivity<ApplyStrategyExecutionCancellationInput>
    {

        #region Arguments

        [RequiredArgument]
        public InArgument<BaseQueue<CancellingStrategyExecutionBatch>> InputQueue { get; set; }

        #endregion

        protected override void DoWork(ApplyStrategyExecutionCancellationInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {

            handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, "Started Cancelling Strategy Execution Items and Cases");

            StrategyExecutionItemManager manager = new StrategyExecutionItemManager();

            DoWhilePreviousRunning(previousActivityStatus, handle, () =>
            {
                int index = 0;
                int totalIndex = 0;

                bool hasItem = false;
                do
                {

                    hasItem = inputArgument.InputQueue.TryDequeue(
                        (batch) =>
                        {

                            index++;
                            totalIndex++;
                            if (index == 1000)
                            {
                                Console.WriteLine("{0} Items Dequeued", totalIndex);
                                index = 0;
                            }

                            //foreach (var items in batch.StrategyExecutionItemIds)
                            //manager.AssignAccountCase(items.AccountNumber, items.IMEIs);
                        });
                }
                while (!ShouldStop(handle) && hasItem);
            });

            handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, "Finished Cancelling Strategy Execution Items and Cases");
        }

        protected override ApplyStrategyExecutionCancellationInput GetInputArgument2(System.Activities.AsyncCodeActivityContext context)
        {
            return new ApplyStrategyExecutionCancellationInput
            {
                InputQueue = this.InputQueue.Get(context)
            };
        }

    }
}
