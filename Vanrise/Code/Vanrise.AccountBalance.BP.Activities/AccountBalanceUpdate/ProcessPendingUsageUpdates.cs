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

    public class ProcessPendingUsageUpdatesInput
    {
        public BaseQueue<BalanceUsageQueue> InputQueue { get; set; }
        public AccountBalanceUpdateHandler AcountBalanceUpdateHandler { get; set; }
    }

    #endregion
    
    public sealed class ProcessPendingUsageUpdates : DependentAsyncActivity<ProcessPendingUsageUpdatesInput>
    {
  
        #region Arguments

        [RequiredArgument]
        public InOutArgument<BaseQueue<BalanceUsageQueue>> InputQueue { get; set; }
        [RequiredArgument]
        public InArgument<AccountBalanceUpdateHandler> AcountBalanceUpdateHandler { get; set; }
      
        #endregion
      
        protected override void DoWork(ProcessPendingUsageUpdatesInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
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
        protected override ProcessPendingUsageUpdatesInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new ProcessPendingUsageUpdatesInput()
            {
                InputQueue = this.InputQueue.Get(context),
                AcountBalanceUpdateHandler = this.AcountBalanceUpdateHandler.Get(context),
            };
        }
        private void ProcessPendingUsageUpdatesMethod(BalanceUsageQueue balanceUsageQueue, AccountBalanceUpdateHandler acountBalanceUpdateHandler)
        {
            if(balanceUsageQueue.UsageDetails != null && balanceUsageQueue.UsageDetails.UsageBalanceUpdates != null)
            {
                acountBalanceUpdateHandler.AddAndUpdateLiveBalanceFromBalanceUsageQueue(balanceUsageQueue.BalanceUsageQueueId, balanceUsageQueue.UsageDetails.UsageBalanceUpdates);
            }
        }
    }
}
