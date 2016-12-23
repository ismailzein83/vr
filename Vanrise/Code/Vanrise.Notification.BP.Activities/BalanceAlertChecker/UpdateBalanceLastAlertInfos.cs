using System.Activities;
using Vanrise.BusinessProcess;
using Vanrise.Notification.Entities;
using Vanrise.Queueing;

namespace Vanrise.Notification.BP.Activities.BalanceAlertChecker
{
    public class UpdateBalanceLastAlertInfosInput
    {
        public VRBalanceAlertRuleTypeSettings RuleTypeSettings { get; set; }
        public BaseQueue<VRBalanceUpdateLastAlertInfoPayloadBatch> InputQueue { get; set; }
    }
    public sealed class UpdateBalanceLastAlertInfos : DependentAsyncActivity<UpdateBalanceLastAlertInfosInput>
    {
        [RequiredArgument]
        public InArgument<VRBalanceAlertRuleTypeSettings> RuleTypeSettings { get; set; }
        [RequiredArgument]
        public InArgument<BaseQueue<VRBalanceUpdateLastAlertInfoPayloadBatch>> InputQueue { get; set; }

        protected override void DoWork(UpdateBalanceLastAlertInfosInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {

            DoWhilePreviousRunning(previousActivityStatus, handle, () =>
                {
                    bool hasItems = false;
                    do
                    {
                        hasItems = inputArgument.InputQueue.TryDequeue(
                       (vrBalanceUpdateLastAlertInfoPayloadBatch) =>
                       {
                           VRBalanceAlertRuleUpdateBalanceLastAlertInfosContext context = new VRBalanceAlertRuleUpdateBalanceLastAlertInfosContext
                           {
                               BalanceLastAlertInfosToUpdate = vrBalanceUpdateLastAlertInfoPayloadBatch.Items,
                               RuleTypeSettings = inputArgument.RuleTypeSettings
                           };
                           inputArgument.RuleTypeSettings.Behavior.UpdateBalanceLastAlertInfos(context);
                       });
                    } while (!ShouldStop(handle) && hasItems);
                });
        }

        protected override UpdateBalanceLastAlertInfosInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new UpdateBalanceLastAlertInfosInput
            {
                InputQueue = this.InputQueue.Get(context),
                RuleTypeSettings = this.RuleTypeSettings.Get(context)
            };
        }
    }
}
