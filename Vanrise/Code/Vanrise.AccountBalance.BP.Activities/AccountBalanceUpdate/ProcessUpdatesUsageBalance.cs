using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Vanrise.BusinessProcess;
using Vanrise.Queueing;
using Vanrise.AccountBalance.Entities;
using Vanrise.AccountBalance.Data;
using Vanrise.Common.Business;
using Vanrise.AccountBalance.Business;

namespace Vanrise.AccountBalance.BP.Activities
{
  
    #region Argument Classes

    public class ProcessUpdatesUsageBalanceInput
    {
        public BaseQueue<BalanceUsageQueue<UpdateUsageBalancePayload>> InputQueue { get; set; }
        public AccountBalanceUpdateHandler AcountBalanceUpdateHandler { get; set; }
    }

    #endregion

    public sealed class ProcessUpdatesUsageBalance : DependentAsyncActivity<ProcessUpdatesUsageBalanceInput>
    {
  
        #region Arguments

        [RequiredArgument]
        public InOutArgument<BaseQueue<BalanceUsageQueue<UpdateUsageBalancePayload>>> InputQueue { get; set; }
        [RequiredArgument]
        public InArgument<AccountBalanceUpdateHandler> AcountBalanceUpdateHandler { get; set; }
      
        #endregion

        protected override void DoWork(ProcessUpdatesUsageBalanceInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
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
                            ProcessPendingUsageUpdatesMethod(balanceUsageQueue, inputArgument.AcountBalanceUpdateHandler);
                        });
                } while (!ShouldStop(handle) && hasItems);
            });
            handle.SharedInstanceData.WriteTrackingMessage(Vanrise.Entities.LogEntryType.Information, "{0} Pending Usage Queues Processed", counter);

        }
        protected override ProcessUpdatesUsageBalanceInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new ProcessUpdatesUsageBalanceInput()
            {
                InputQueue = this.InputQueue.Get(context),
                AcountBalanceUpdateHandler = this.AcountBalanceUpdateHandler.Get(context),
            };
        }
        private void ProcessPendingUsageUpdatesMethod(BalanceUsageQueue<UpdateUsageBalancePayload> balanceUsageQueue, AccountBalanceUpdateHandler acountBalanceUpdateHandler)
        {
            if(balanceUsageQueue.UsageDetails != null && balanceUsageQueue.UsageDetails.UpdateUsageBalanceItems != null)
            {
                acountBalanceUpdateHandler.AddAndUpdateLiveBalanceFromBalanceUsageQueue(balanceUsageQueue.BalanceUsageQueueId, balanceUsageQueue.UsageDetails.TransactionTypeId,balanceUsageQueue.UsageDetails.UpdateUsageBalanceItems);
            }
        }
    }
}
