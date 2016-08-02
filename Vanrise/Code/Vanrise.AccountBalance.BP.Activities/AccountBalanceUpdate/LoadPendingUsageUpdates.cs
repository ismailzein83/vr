using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Vanrise.Queueing;
using Vanrise.AccountBalance.Entities;
using Vanrise.BusinessProcess;
using Vanrise.AccountBalance.Data;

namespace Vanrise.AccountBalance.BP.Activities
{
   
    #region Argument Classes
    public class LoadPendingUsageUpdatesInput
    {
        public BaseQueue<BalanceUsageQueue> OutputQueue { get; set; }
    }
    #endregion
    
    public sealed class LoadPendingUsageUpdates : BaseAsyncActivity<LoadPendingUsageUpdatesInput>
    {

        #region Arguments
        [RequiredArgument]
        public InOutArgument<BaseQueue<BalanceUsageQueue>> OutputQueue { get; set; }
        
        #endregion

        protected override void DoWork(LoadPendingUsageUpdatesInput inputArgument, AsyncActivityHandle handle)
        {
            handle.SharedInstanceData.WriteTrackingMessage(Vanrise.Entities.LogEntryType.Information, "Started Loading Pending Usage Updates ...");

            IBalanceUsageQueueDataManager dataManager = AccountBalanceDataManagerFactory.GetDataManager<IBalanceUsageQueueDataManager>();

            dataManager.LoadUsageBalanceUpdate((balanceUsageQueue) =>
            {
                inputArgument.OutputQueue.Enqueue(balanceUsageQueue);
                handle.SharedInstanceData.WriteTrackingMessage(Vanrise.Entities.LogEntryType.Information, "New Billing Transactions loaded.");
            });

        }
        protected override void OnBeforeExecute(AsyncCodeActivityContext context, AsyncActivityHandle handle)
        {
            if (this.OutputQueue.Get(context) == null)
                this.OutputQueue.Set(context, new MemoryQueue<BalanceUsageQueue>());
            base.OnBeforeExecute(context, handle);
        }
        protected override LoadPendingUsageUpdatesInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new LoadPendingUsageUpdatesInput()
            {
                OutputQueue = this.OutputQueue.Get(context)
            };
        }

    }
}
