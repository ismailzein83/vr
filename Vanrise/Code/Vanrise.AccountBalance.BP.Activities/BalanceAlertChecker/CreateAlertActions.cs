using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Vanrise.AccountBalance.Entities;
using Vanrise.Queueing;
using Vanrise.BusinessProcess;
using Vanrise.Notification.Business;
using Vanrise.Notification.Entities;
using Vanrise.AccountBalance.Business;
using Vanrise.AccountBalance.Data;

namespace Vanrise.AccountBalance.BP.Activities
{
    #region Argument Classes
    public class CreateAlertActionsInput
    {
        public BaseQueue<AccountBalanceForAlertRuleBatch> InputQueue { get; set; }
        public int UserId { get; set; }
    }
    #endregion
    public sealed class CreateAlertActions : DependentAsyncActivity<CreateAlertActionsInput>
    {       
        
        #region Arguments

        [RequiredArgument]
        public InOutArgument<BaseQueue<AccountBalanceForAlertRuleBatch>> InputQueue { get; set; }
         [RequiredArgument]
        public InArgument<int> UserId { get; set; }

        #endregion

        protected override void DoWork(CreateAlertActionsInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            DoWhilePreviousRunning(previousActivityStatus, handle, () =>
            {
                bool hasItems = false;
                do
                {
                    hasItems = inputArgument.InputQueue.TryDequeue(
                        (balanceAccounts) =>
                        {

                            if (balanceAccounts.AccountBalancesForAlertRules != null && balanceAccounts.AccountBalancesForAlertRules.Count > 0)
                            {

                                handle.SharedInstanceData.WriteTrackingMessage(Vanrise.Entities.LogEntryType.Information, "Alert Actions Created.");

                                BalanceAlertRuleManager ruleManager = new BalanceAlertRuleManager();
                                VRActionManager vrActionManager = new VRActionManager();
                                IAlertRuleActionExecutionDataManager dataManager = AccountBalanceDataManagerFactory.GetDataManager<IAlertRuleActionExecutionDataManager>();

                                foreach (var accountBalance in balanceAccounts.AccountBalancesForAlertRules)
                                {
                                        var rule = ruleManager.GetGenericRule(accountBalance.AlertRuleId) as BalanceAlertRule;
                                        var thresholdAction = rule.Settings.ThresholdActions[accountBalance.ThresholdActionIndex];

                                        foreach (var action in thresholdAction.Actions)
                                        {
                                            CreateVRActionInput createVRActionInput = new CreateVRActionInput
                                            {
                                                Action = action,
                                                EventPayload = new BalanceAlertEventPayload { AccountId = accountBalance.AccountId, Threshold = accountBalance.Threshold }
                                            };
                                            vrActionManager.CreateAction(createVRActionInput, inputArgument.UserId);
                                        }

                                    long alertRuleActionExecutionId = -1;
                                    AlertRuleActionExecution alertRuleActionExecution = new AlertRuleActionExecution{
                                        AccountID = accountBalance.AccountId,
                                        ExecutionTime = DateTime.Now,
                                        Threshold = accountBalance.Threshold,
                                        ActionExecutionInfo = new ActionExecutionInfo
                                        {
                                            RollBackActions = thresholdAction.RollbackActions
                                        }
                                    };
                                    dataManager.Insert(alertRuleActionExecution, out alertRuleActionExecutionId);
                                }
                                handle.SharedInstanceData.WriteTrackingMessage(Vanrise.Entities.LogEntryType.Information, "Alert Actions Applied.");

                            }
                        });
                } while (!ShouldStop(handle) && hasItems);
            });
        }
        protected override CreateAlertActionsInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new CreateAlertActionsInput()
            {
                InputQueue = this.InputQueue.Get(context),
                UserId = this.UserId.Get(context),
            };
        }
    }
}
