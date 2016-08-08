using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Vanrise.BusinessProcess;
using Vanrise.Queueing;
using Vanrise.AccountBalance.Entities;
using Vanrise.AccountBalance.Data;

namespace Vanrise.AccountBalance.BP.Activities
{
    #region Argument Classes
    public class LoadBalancesToAlertInput
    {
        public BaseQueue<AccountBalanceForAlertRuleBatch> OutputQueue { get; set; }
    }
    #endregion
    public sealed class LoadBalancesToAlert : BaseAsyncActivity<LoadBalancesToAlertInput>
    {
        #region Arguments

        [RequiredArgument]
        public InOutArgument<BaseQueue<AccountBalanceForAlertRuleBatch>> OutputQueue { get; set; }
        #endregion
        protected override void DoWork(LoadBalancesToAlertInput inputArgument, AsyncActivityHandle handle)
        {
            handle.SharedInstanceData.WriteTrackingMessage(Vanrise.Entities.LogEntryType.Information, "Started Loading Account Balances.");

            IBalanceAlertRuleDataManager dataManager = AccountBalanceDataManagerFactory.GetDataManager<IBalanceAlertRuleDataManager>();
            var batchSize = 100;
            var list = new List<AccountBalanceForAlertRule>();
            var counter = 0;
            dataManager.GetLiveBalancesToAlert((liveBalance) =>
            {
                counter++;
                if (counter == batchSize)
                {
                    var liveBalanceBatch = new AccountBalanceForAlertRuleBatch() { AccountBalancesForAlertRules = list };
                    inputArgument.OutputQueue.Enqueue(liveBalanceBatch);
                    list = new List<AccountBalanceForAlertRule>();
                    counter = 0;
                }
                list.Add(liveBalance);
            });

            if (list.Count > 0)
            {
                var liveBalanceBatch = new AccountBalanceForAlertRuleBatch() { AccountBalancesForAlertRules = list };
                inputArgument.OutputQueue.Enqueue(liveBalanceBatch);
            }
            handle.SharedInstanceData.WriteTrackingMessage(Vanrise.Entities.LogEntryType.Information, "Finish Loading Account Balances.");
        }
        protected override void OnBeforeExecute(AsyncCodeActivityContext context, AsyncActivityHandle handle)
        {
            if (this.OutputQueue.Get(context) == null)
                this.OutputQueue.Set(context, new MemoryQueue<AccountBalanceForAlertRuleBatch>());
            base.OnBeforeExecute(context, handle);
        }
        protected override LoadBalancesToAlertInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new LoadBalancesToAlertInput()
            {
                OutputQueue = this.OutputQueue.Get(context)
            };
        }
    }
}
