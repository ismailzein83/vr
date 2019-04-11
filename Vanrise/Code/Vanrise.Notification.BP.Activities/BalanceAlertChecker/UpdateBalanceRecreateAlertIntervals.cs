using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess;
using Vanrise.Notification.Entities;
using Vanrise.Queueing;

namespace Vanrise.Notification.BP.Activities.BalanceAlertChecker
{
    public class UpdateBalanceRecreateAlertIntervalsInput
    {
        public VRBalanceAlertRuleTypeSettings RuleTypeSettings { get; set; }
        public BaseQueue<VRBalanceUpdateRecreateAlertIntervalPayloadBatch> InputQueue { get; set; }
    }

    public class UpdateBalanceRecreateAlertIntervals : DependentAsyncActivity<UpdateBalanceRecreateAlertIntervalsInput>
    {
        [RequiredArgument]
        public InArgument<VRBalanceAlertRuleTypeSettings> RuleTypeSettings { get; set; }
        [RequiredArgument]
        public InArgument<BaseQueue<VRBalanceUpdateRecreateAlertIntervalPayloadBatch>> InputQueue { get; set; }

        protected override void DoWork(UpdateBalanceRecreateAlertIntervalsInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {

            DoWhilePreviousRunning(previousActivityStatus, handle, () =>
            {
                bool hasItems = false;
                do
                {
                    hasItems = inputArgument.InputQueue.TryDequeue(
                   (vrBalanceUpdateLastAlertInfoPayloadBatch) =>
                   {
                       VRBalanceAlertRuleUpdateBalanceRecreateAlertIntervalContext context = new VRBalanceAlertRuleUpdateBalanceRecreateAlertIntervalContext
                       {
                           BalanceRecreateAlertIntervalsToUpdate = vrBalanceUpdateLastAlertInfoPayloadBatch.Items,
                           RuleTypeSettings = inputArgument.RuleTypeSettings
                       };
                       inputArgument.RuleTypeSettings.Behavior.UpdateBalanceRecreateAlertInterval(context);
                   });
                } while (!ShouldStop(handle) && hasItems);
            });
        }

        protected override UpdateBalanceRecreateAlertIntervalsInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new UpdateBalanceRecreateAlertIntervalsInput
            {
                InputQueue = this.InputQueue.Get(context),
                RuleTypeSettings = this.RuleTypeSettings.Get(context)
            };
        }


        private class VRBalanceAlertRuleUpdateBalanceRecreateAlertIntervalContext : IVRBalanceAlertRuleUpdateBalanceRecreateAlertIntervalContext
        {
            public List<VRBalanceUpdateRecreateAlertIntervalPayload> BalanceRecreateAlertIntervalsToUpdate
            {
                get;
                set;
            }

            public VRBalanceAlertRuleTypeSettings RuleTypeSettings
            {
                get;
                set;
            }
        }

    }
}
