using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess;
using Vanrise.Notification.Entities;
using Vanrise.Queueing;

namespace Vanrise.Notification.BP.Activities.BalanceAlertThresholdUpdate
{
    public class UpdateBalanceRuleInfosInput
    {
        public VRBalanceAlertRuleTypeSettings RuleTypeSettings { get; set; }
        public BaseQueue<VRBalanceUpdateRuleInfoPayloadBatch> InputQueue { get; set; }
    }
    public sealed class UpdateBalanceRuleInfos : DependentAsyncActivity<UpdateBalanceRuleInfosInput>
    {
        [RequiredArgument]
        public InArgument<VRBalanceAlertRuleTypeSettings> RuleTypeSettings { get; set; }

        [RequiredArgument]
        public InArgument<BaseQueue<VRBalanceUpdateRuleInfoPayloadBatch>> InputQueue { get; set; }

        protected override void DoWork(UpdateBalanceRuleInfosInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {

            DoWhilePreviousRunning(previousActivityStatus, handle, () =>
            {
                bool hasItem = false;
                do
                {
                    hasItem = inputArgument.InputQueue.TryDequeue(
                        (vrBalanceUpdateRuleInfoPayloadBatch) =>
                        {
                            VRBalanceAlertRuleUpdateBalanceRuleInfosContext context = new VRBalanceAlertRuleUpdateBalanceRuleInfosContext
                                {
                                    BalanceRuleInfosToUpdate = vrBalanceUpdateRuleInfoPayloadBatch.Items,
                                    RuleTypeSettings = inputArgument.RuleTypeSettings
                                };
                            inputArgument.RuleTypeSettings.Behavior.UpdateBalanceRuleInfos(context);
                        });

                } while (!ShouldStop(handle) && hasItem);
            });
        }

        protected override UpdateBalanceRuleInfosInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new UpdateBalanceRuleInfosInput
            {
                InputQueue = this.InputQueue.Get(context),
                RuleTypeSettings = this.RuleTypeSettings.Get(context)
            };
        }

     }
}
