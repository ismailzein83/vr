using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Vanrise.Queueing;
using Vanrise.AccountBalance.Entities;
using Vanrise.BusinessProcess;
using Vanrise.AccountBalance.Data;
using Vanrise.AccountBalance.Business;

namespace Vanrise.AccountBalance.BP.Activities
{

    #region Argument Classes
    public class LoadUpdatesUsageBalanceInput
    {
        public Guid AccountTypeId { get; set; }
        public BaseQueue<BalanceUsageQueue<UpdateUsageBalancePayload>> OutputQueue { get; set; }
    }
    #endregion

    public sealed class LoadUpdatesUsageBalance : BaseAsyncActivity<LoadUpdatesUsageBalanceInput>
    {

        #region Arguments
        [RequiredArgument]
        public InArgument<Guid> AccountTypeId { get; set; }

        [RequiredArgument]
        public InOutArgument<BaseQueue<BalanceUsageQueue<UpdateUsageBalancePayload>>> OutputQueue { get; set; }

        #endregion

        protected override void DoWork(LoadUpdatesUsageBalanceInput inputArgument, AsyncActivityHandle handle)
        {
            handle.SharedInstanceData.WriteTrackingMessage(Vanrise.Entities.LogEntryType.Information, "Started Loading Pending Usage Updates ...");

            UsageBalanceManager UsageBalanceManager = new UsageBalanceManager();
            UsageBalanceManager.LoadUpdatesUsageBalance(inputArgument.AccountTypeId, (balanceUsageQueue) =>
            {
                inputArgument.OutputQueue.Enqueue(balanceUsageQueue);
                handle.SharedInstanceData.WriteTrackingMessage(Vanrise.Entities.LogEntryType.Information, string.Format("{0} New Pending Usage loaded.", balanceUsageQueue.UsageDetails.UpdateUsageBalanceItems.Count()));
            });

        }
        protected override void OnBeforeExecute(AsyncCodeActivityContext context, AsyncActivityHandle handle)
        {
            if (this.OutputQueue.Get(context) == null)
                this.OutputQueue.Set(context, new MemoryQueue<BalanceUsageQueue<UpdateUsageBalancePayload>>());
            base.OnBeforeExecute(context, handle);
        }
        protected override LoadUpdatesUsageBalanceInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new LoadUpdatesUsageBalanceInput()
            {
                AccountTypeId = this.AccountTypeId.Get(context),
                OutputQueue = this.OutputQueue.Get(context)
            };
        }

    }
}
