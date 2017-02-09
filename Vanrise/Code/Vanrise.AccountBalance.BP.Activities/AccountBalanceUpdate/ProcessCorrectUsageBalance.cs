using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.AccountBalance.Business;
using Vanrise.AccountBalance.Entities;
using Vanrise.BusinessProcess;
using Vanrise.Queueing;

namespace Vanrise.AccountBalance.BP.Activities
{
    #region Argument Classes

    public class ProcessCorrectUsageBalanceInput
    {
        public BaseQueue<BalanceUsageQueue<CorrectUsageBalancePayload>> InputQueue { get; set; }
        public AccountBalanceUpdateHandler AcountBalanceUpdateHandler { get; set; }
    }

    #endregion


    public sealed class ProcessCorrectUsageBalance : DependentAsyncActivity<ProcessCorrectUsageBalanceInput>
    {

        #region Arguments

        [RequiredArgument]
        public InOutArgument<BaseQueue<BalanceUsageQueue<CorrectUsageBalancePayload>>> InputQueue { get; set; }
        [RequiredArgument]
        public InArgument<AccountBalanceUpdateHandler> AcountBalanceUpdateHandler { get; set; }

        #endregion

        protected override void DoWork(ProcessCorrectUsageBalanceInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            var counter = 0;
            DoWhilePreviousRunning(previousActivityStatus, handle, () =>
            {
                bool hasItems = false;
                do
                {
                    hasItems = inputArgument.InputQueue.TryDequeue(
                        (balanceUsageQueue) =>
                        {
                            counter++;
                            ProcessCorrectUsageBalanceMethod(balanceUsageQueue, inputArgument.AcountBalanceUpdateHandler);
                        });
                } while (!ShouldStop(handle) && hasItems);
            });
            handle.SharedInstanceData.WriteTrackingMessage(Vanrise.Entities.LogEntryType.Information, "{0} Pending Usage Queues Processed", counter);

        }
        protected override ProcessCorrectUsageBalanceInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new ProcessCorrectUsageBalanceInput()
            {
                InputQueue = this.InputQueue.Get(context),
                AcountBalanceUpdateHandler = this.AcountBalanceUpdateHandler.Get(context),
            };
        }
        private void ProcessCorrectUsageBalanceMethod(BalanceUsageQueue<CorrectUsageBalancePayload> balanceUsageQueue, AccountBalanceUpdateHandler acountBalanceUpdateHandler)
        {
            if (balanceUsageQueue.UsageDetails != null)
            {
                acountBalanceUpdateHandler.CorrectBalanceFromBalanceUsageQueue(balanceUsageQueue.BalanceUsageQueueId, balanceUsageQueue.UsageDetails.TransactionTypeId, balanceUsageQueue.UsageDetails.CorrectUsageBalanceItems, balanceUsageQueue.UsageDetails.PeriodDate, balanceUsageQueue.UsageDetails.CorrectionProcessId, balanceUsageQueue.UsageDetails.IsLastBatch);
            }
        }
    }
}
