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
        public BaseQueue<AlertRuleThresholdActionBatch> OutputQueue { get; set; }
    }
    #endregion
    public sealed class CalculateNextAlertThresholds : DependentAsyncActivity<CalculateNextAlertThresholdsInput>
    {

        #region Arguments

        [RequiredArgument]
        public InOutArgument<BaseQueue<AccountBalanceBatch>> InputQueue { get; set; }

        [RequiredArgument]
        public InOutArgument<BaseQueue<AlertRuleThresholdActionBatch>> OutputQueue { get; set; }
   
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
                this.OutputQueue.Set(context, new MemoryQueue<AlertRuleThresholdActionBatch>());
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
        private AlertRuleThresholdActionBatch ProcessLiveBalances(List<LiveBalance> accountBalances)
        {
            BalanceAlertRuleManager ruleManager = new BalanceAlertRuleManager();
            List<AlertRuleThresholdAction> alertRuleThresholdActions = new List<AlertRuleThresholdAction>();
            List<AccountBalanceAlertRule> accountBalanceAlertRules = new List<AccountBalanceAlertRule>();

            BalanceAlertThresholdContext context = new BalanceAlertThresholdContext();
            foreach(var balance in accountBalances)
            {
                var rule = ruleManager.GetMatchRule(balance.AccountId);
                if (rule == null)
                    continue;
                if (!accountBalanceAlertRules.Any(x=>x.AlertRuleId == rule.RuleId))
                {
                    for (int i = 0; i < rule.Settings.ThresholdActions.Count; i++)
                    {
                        var thresholdAction = rule.Settings.ThresholdActions[i];
                        decimal threshold = thresholdAction.Threshold.GetThreshold(context);

                        alertRuleThresholdActions.Add(new AlertRuleThresholdAction
                        {
                            RuleId = rule.RuleId,
                            Threshold = threshold,
                            ThresholdActionIndex = i
                        });
                    }
                    accountBalanceAlertRules.Add(new AccountBalanceAlertRule
                    {
                        AccountId = balance.AccountId,
                        AlertRuleId = rule.RuleId,
                    });
                }
            }
            return new AlertRuleThresholdActionBatch
            {
                AccountBalanceAlertRules = accountBalanceAlertRules,
                AlertRuleThresholdActions = alertRuleThresholdActions
            };
        }
    }
}
