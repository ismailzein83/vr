using System.Activities;
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
            int totalEntityBalanceInfoUpdated = 0;
            int totalEntityBalanceInfoBatchesUpdated = 0;
            DoWhilePreviousRunning(previousActivityStatus, handle, () =>
            {
                bool hasItem = false;
                do
                {
                    hasItem = inputArgument.InputQueue.TryDequeue(
                        (vrBalanceUpdateRuleInfoPayloadBatch) =>
                        {
                            totalEntityBalanceInfoBatchesUpdated++;
                            totalEntityBalanceInfoUpdated += vrBalanceUpdateRuleInfoPayloadBatch.Items.Count;
                            VRBalanceAlertRuleUpdateBalanceRuleInfosContext context = new VRBalanceAlertRuleUpdateBalanceRuleInfosContext
                                {
                                    BalanceRuleInfosToUpdate = vrBalanceUpdateRuleInfoPayloadBatch.Items,
                                    RuleTypeSettings = inputArgument.RuleTypeSettings
                                };
                            inputArgument.RuleTypeSettings.Behavior.UpdateBalanceRuleInfos(context);
                        });

                } while (!ShouldStop(handle) && hasItem);
            });
            handle.SharedInstanceData.WriteTrackingMessage(Vanrise.Entities.LogEntryType.Information, "{0} Total Balance Info Updated.", totalEntityBalanceInfoUpdated);
            handle.SharedInstanceData.WriteTrackingMessage(Vanrise.Entities.LogEntryType.Information, "{0} Total Balance Info Batches Updated.", totalEntityBalanceInfoBatchesUpdated);
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
