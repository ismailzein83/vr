using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.AccountBalance.Business;
using Vanrise.AccountBalance.Data;
using Vanrise.AccountBalance.Entities;
using Vanrise.BusinessProcess;
using Vanrise.Notification.Business;
using Vanrise.Notification.Entities;
using Vanrise.Queueing;

namespace Vanrise.AccountBalance.BP.Activities
{

    #region Argument Classes
    public class RollbackActionsAndDeleteAlertRuleActionExecutionInput
    {
        public Vanrise.Queueing.BaseQueue<AlertRuleActionExecution> InputQueue { get; set; }
        public int UserId { get; set; }
    }
    #endregion
    public class RollbackActionsAndDeleteAlertRuleActionExecution : DependentAsyncActivity<RollbackActionsAndDeleteAlertRuleActionExecutionInput>
    {
        #region Arguments

        [RequiredArgument]
        public InOutArgument<BaseQueue<AlertRuleActionExecution>> InputQueue { get; set; }
        [RequiredArgument]
        public InArgument<int> UserId { get; set; }

        #endregion

        protected override void DoWork(RollbackActionsAndDeleteAlertRuleActionExecutionInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {

            DoWhilePreviousRunning(previousActivityStatus, handle, () =>
            {
                bool hasItems = false;
                do
                {
                    hasItems = inputArgument.InputQueue.TryDequeue(
                        (alertRuleActionExecution) =>
                        {
                            IAlertRuleActionExecutionDataManager dataManager = AccountBalanceDataManagerFactory.GetDataManager<IAlertRuleActionExecutionDataManager>();
                            if (alertRuleActionExecution.ActionExecutionInfo != null && alertRuleActionExecution.ActionExecutionInfo.RollBackActions != null && alertRuleActionExecution.ActionExecutionInfo.RollBackActions.Count > 0)
                            {
                                VRActionManager vrActionManager = new VRActionManager();


                                foreach (var action in alertRuleActionExecution.ActionExecutionInfo.RollBackActions)
                                {
                                   
                                    CreateVRActionInput createVRActionInput = new CreateVRActionInput
                                    {
                                        Action = action,
                                        EventPayload = new BalanceAlertEventPayload { AccountId = alertRuleActionExecution.AccountID, Threshold = alertRuleActionExecution.Threshold }
                                    };
                                    vrActionManager.CreateAction(createVRActionInput, inputArgument.UserId);
                                }
                            }
                            dataManager.Delete(alertRuleActionExecution.AlertRuleActionExecutionId);
                            handle.SharedInstanceData.WriteTrackingMessage(Vanrise.Entities.LogEntryType.Information, "Expired Actions Deleted.");
                        });
                } while (!ShouldStop(handle) && hasItems);
            });

        }

        protected override RollbackActionsAndDeleteAlertRuleActionExecutionInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new RollbackActionsAndDeleteAlertRuleActionExecutionInput()
            {
                InputQueue = this.InputQueue.Get(context),
                UserId = this.UserId.Get(context),
            };
        }

    }

}
