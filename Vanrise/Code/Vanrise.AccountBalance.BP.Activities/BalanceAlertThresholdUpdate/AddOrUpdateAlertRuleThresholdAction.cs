using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.AccountBalance.Data;
using Vanrise.AccountBalance.Entities;
using Vanrise.BusinessProcess;
using Vanrise.Queueing;

namespace Vanrise.AccountBalance.BP.Activities
{
    
    #region Argument Classes
    public class AddOrUpdateAlertRuleThresholdActionInput
    {
        public BaseQueue<AlertRuleThresholdActionBatch> InputQueue { get; set; }
    }
    #endregion

    public class AddOrUpdateAlertRuleThresholdAction : DependentAsyncActivity<AddOrUpdateAlertRuleThresholdActionInput>
    {
        #region Arguments

        [RequiredArgument]
        public InOutArgument<BaseQueue<AlertRuleThresholdActionBatch>> InputQueue { get; set; }

        #endregion

        protected override void DoWork(AddOrUpdateAlertRuleThresholdActionInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            var counter = 0;
            DoWhilePreviousRunning(previousActivityStatus, handle, () =>
            {
                bool hasItems = false;
                do
                {
                    hasItems = inputArgument.InputQueue.TryDequeue(
                        (alertRuleThresholdActionBatch) =>
                        {
                            counter += alertRuleThresholdActionBatch.AlertRuleThresholdActions.Count();
                            IAlertRuleThresholdActionDataManager dataManager = AccountBalanceDataManagerFactory.GetDataManager<IAlertRuleThresholdActionDataManager>();
                            dataManager.AddOrUpdateAlertRuleThresholdAction(alertRuleThresholdActionBatch.AlertRuleThresholdActions);
                            ILiveBalanceDataManager liveBalanceDataManager = AccountBalanceDataManagerFactory.GetDataManager<ILiveBalanceDataManager>();
                            liveBalanceDataManager.UpdateLiveBalanceAlertRule(alertRuleThresholdActionBatch.AccountBalanceAlertRules);

                        });
                } while (!ShouldStop(handle) && hasItems);
            });
            handle.SharedInstanceData.WriteTrackingMessage(Vanrise.Entities.LogEntryType.Information, "{0} account balances alert rule updated.", counter);
        }
        protected override AddOrUpdateAlertRuleThresholdActionInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new AddOrUpdateAlertRuleThresholdActionInput()
            {
                InputQueue = this.InputQueue.Get(context),
            };
        }

    }
}
