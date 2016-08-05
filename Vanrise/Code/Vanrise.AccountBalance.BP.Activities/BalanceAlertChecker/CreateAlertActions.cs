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

namespace Vanrise.AccountBalance.BP.Activities
{
    #region Argument Classes
    public class CreateAlertActionsInput
    {
        public BaseQueue<AccountBalanceBatch> InputQueue { get; set; }
        public int UserId { get; set; }
    }
    #endregion
    public sealed class CreateAlertActions : DependentAsyncActivity<CreateAlertActionsInput>
    {       
        
        #region Arguments

        [RequiredArgument]
        public InOutArgument<BaseQueue<AccountBalanceBatch>> InputQueue { get; set; }
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

                            if(balanceAccounts.AccountBalances !=null && balanceAccounts.AccountBalances.Count>0)
                            {
                                 BalanceAlertRuleManager ruleManager = new BalanceAlertRuleManager();
                                 VRActionManager vrActionManager = new VRActionManager();
                                
                                foreach(var accountBalance in balanceAccounts.AccountBalances)
                                {
                                    if (accountBalance.NextAlertThreshold.HasValue && accountBalance.NextAlertThreshold.Value > accountBalance.CurrentBalance)
                                    {
                                        if (accountBalance.AlertRuleID.HasValue)
                                        {
                                            var rule = ruleManager.GetGenericRule(accountBalance.AlertRuleID.Value) as BalanceAlertRule;
                                            var thresholdAction = rule.Settings.ThresholdActions[accountBalance.ThresholdActionIndex.Value];

                                            foreach (var action in thresholdAction.Actions)
                                            {
                                                CreateVRActionInput createVRActionInput = new CreateVRActionInput
                                                {
                                                    Action = action,
                                                    EventPayload = new BalanceAlertEventPayload { AccountId = accountBalance.AccountId, Threshold =              accountBalance.NextAlertThreshold.Value }
                                                };
                                                vrActionManager.CreateAction(createVRActionInput, inputArgument.UserId);
                                            }
                                        }
                                    }
                                }
                            }
                            handle.SharedInstanceData.WriteTrackingMessage(Vanrise.Entities.LogEntryType.Information, "Update next alert thresholds.");
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
