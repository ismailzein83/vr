using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Vanrise.BusinessProcess;
using Vanrise.Queueing;
using Vanrise.AccountBalance.Entities;
using Vanrise.AccountBalance.Business;

namespace Vanrise.AccountBalance.BP.Activities
{

    #region Argument Classes
    public class CalculateNextAlertThresholdsInput
    {
        public BaseQueue<AccountBalanceBatch> InputQueue { get; set; }
        public BaseQueue<List<BalanceAccountThreshold>> OutputQueue { get; set; }
    }
    #endregion
    public sealed class CalculateNextAlertThresholds : DependentAsyncActivity<CalculateNextAlertThresholdsInput>
    {

        #region Arguments

        [RequiredArgument]
        public InOutArgument<BaseQueue<AccountBalanceBatch>> InputQueue { get; set; }

        [RequiredArgument]
        public InOutArgument<BaseQueue<List<BalanceAccountThreshold>>> OutputQueue { get; set; }
        #endregion

        protected override void DoWork(CalculateNextAlertThresholdsInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            var counter = 0;
            DoWhilePreviousRunning(previousActivityStatus, handle, () =>
            {
                bool hasItems = false;
                do
                {
                    hasItems = inputArgument.InputQueue.TryDequeue(
                        (liveBalanceBatch) =>
                        {
                            counter += liveBalanceBatch.AccountBalances.Count();
                            var balanceAccountThresholds = ProcessLiveBalances(liveBalanceBatch.AccountBalances);
                            inputArgument.OutputQueue.Enqueue(balanceAccountThresholds);
                        });
                } while (!ShouldStop(handle) && hasItems);
            });
            handle.SharedInstanceData.WriteTrackingMessage(Vanrise.Entities.LogEntryType.Information, "Next alert threshold has been calculated for {0} accounts ", counter);
        }

        protected override void OnBeforeExecute(AsyncCodeActivityContext context, AsyncActivityHandle handle)
        {
            if (this.OutputQueue.Get(context) == null)
                this.OutputQueue.Set(context, new MemoryQueue<List<BalanceAccountThreshold>>());
            base.OnBeforeExecute(context, handle);
        }

        protected override CalculateNextAlertThresholdsInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new CalculateNextAlertThresholdsInput()
            {
                InputQueue = this.InputQueue.Get(context),
                OutputQueue = this.OutputQueue.Get(context),
            };
        }
        private List<BalanceAccountThreshold> ProcessLiveBalances(List<LiveBalance> accountBalances)
        {
            BalanceAlertRuleManager ruleManager = new BalanceAlertRuleManager();
            List<BalanceAccountThreshold> balanceAccountThresholds = new List<BalanceAccountThreshold>();
            foreach(var balance in accountBalances)
            {
                var rule = ruleManager.GetMatchRule(balance.AccountId);
                if (rule == null)
                    continue;
                var balanceAccountThreshold = GetBalanceAccountThreshold(rule.Settings.ThresholdActions, balance.AccountId, balance.CurrentBalance, rule.RuleId);
                if (balanceAccountThreshold != null)
                    balanceAccountThresholds.Add(balanceAccountThreshold);
            }
            return balanceAccountThresholds;
        }


        private BalanceAccountThreshold GetBalanceAccountThreshold(List<BalanceAlertThresholdAction>  thresholdActions, long accountId,decimal currentBalance,int alertRuleId)
        {
            BalanceAlertThresholdContext context = new BalanceAlertThresholdContext();
            decimal? minThreshold = null;
            int thresholdActionIndex = 0;
            List<decimal> thresholds = new List<decimal>();
            for(int i = 0 ; i < thresholdActions.Count ;i++)
            {

                var thresholdAction = thresholdActions[i];
                decimal threshold = thresholdAction.Threshold.GetThreshold(context);
                if (!minThreshold.HasValue && threshold < currentBalance)
                {
                      minThreshold = threshold;
                      thresholdActionIndex = i;
                }
                else if (minThreshold.HasValue && threshold > minThreshold.Value && threshold < currentBalance)
                {
                    thresholdActionIndex = i;
                     minThreshold = threshold;
                }
            }
            if (!minThreshold.HasValue)
                return null;
            return new BalanceAccountThreshold
            {
                Threshold = minThreshold.Value,
                ThresholdActionIndex = thresholdActionIndex,
                AccountId = accountId,
                AlertRuleId = alertRuleId
            };
        }
    }
}
