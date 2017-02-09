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
    public class LoadCorrectUsageBalanceInput
    {
        public Guid AccountTypeId { get; set; }
        public BaseQueue<BalanceUsageQueue<CorrectUsageBalancePayload>> OutputQueue { get; set; }
    }
    #endregion
    public sealed class LoadCorrectUsageBalance : BaseAsyncActivity<LoadCorrectUsageBalanceInput>
    {
        
        #region Arguments
        [RequiredArgument]
        public InArgument<Guid> AccountTypeId { get; set; }

        [RequiredArgument]
        public InOutArgument<BaseQueue<BalanceUsageQueue<CorrectUsageBalancePayload>>> OutputQueue { get; set; }

        #endregion

        protected override void DoWork(LoadCorrectUsageBalanceInput inputArgument, AsyncActivityHandle handle)
        {
            handle.SharedInstanceData.WriteTrackingMessage(Vanrise.Entities.LogEntryType.Information, "Started Loading Correct Usage Balance ...");

            UsageBalanceManager UsageBalanceManager = new UsageBalanceManager();
            UsageBalanceManager.LoadCorrectUsageBalance(inputArgument.AccountTypeId, (balanceUsageQueue) =>
            {
                inputArgument.OutputQueue.Enqueue(balanceUsageQueue);
                if (balanceUsageQueue.UsageDetails.CorrectUsageBalanceItems != null && balanceUsageQueue.UsageDetails.CorrectUsageBalanceItems.Count() > 0)
                handle.SharedInstanceData.WriteTrackingMessage(Vanrise.Entities.LogEntryType.Information, string.Format("{0} New Correct Usage Balance loaded.", balanceUsageQueue.UsageDetails.CorrectUsageBalanceItems.Count()));
            });

        }
        protected override void OnBeforeExecute(AsyncCodeActivityContext context, AsyncActivityHandle handle)
        {
            if (this.OutputQueue.Get(context) == null)
                this.OutputQueue.Set(context, new MemoryQueue<BalanceUsageQueue<CorrectUsageBalancePayload>>());
            base.OnBeforeExecute(context, handle);
        }
        protected override LoadCorrectUsageBalanceInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new LoadCorrectUsageBalanceInput()
            {
                AccountTypeId = this.AccountTypeId.Get(context),
                OutputQueue = this.OutputQueue.Get(context)
            };
        }

    }
}
