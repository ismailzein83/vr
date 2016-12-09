using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Vanrise.Notification.Entities;
using Vanrise.Queueing;
using Vanrise.BusinessProcess;

namespace Vanrise.Notification.BP.Activities.BalanceAlertChecker
{
    public class LoadEntitiesToClearAlertsInput
    {
        public VRBalanceAlertRuleTypeSettings RuleTypeSettings { get; set; }
        public BaseQueue<VREntityBalanceInfoBatch> OutputQueue { get; set; }
    }

    public sealed class LoadEntitiesToClearAlerts : DependentAsyncActivity<LoadEntitiesToClearAlertsInput>
    {
        [RequiredArgument]
        public InArgument<VRBalanceAlertRuleTypeSettings> RuleTypeSettings { get; set; }

        [RequiredArgument]
        public InOutArgument<BaseQueue<VREntityBalanceInfoBatch>> OutputQueue { get; set; }

        protected override void DoWork(LoadEntitiesToClearAlertsInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            handle.SharedInstanceData.WriteTrackingMessage(Vanrise.Entities.LogEntryType.Information, "Start Loading Account Balances for Clear Alert.");
            int batchCount = 100;
            List<IVREntityBalanceInfo> lstBalanceInfoToAlert = new List<IVREntityBalanceInfo>();
            Action<IVREntityBalanceInfo> onBalanceInfoLoaded = (vrEntityBalanceInfo) =>
            {
                if (lstBalanceInfoToAlert.Count >= batchCount)
                {
                    inputArgument.OutputQueue.Enqueue(new VREntityBalanceInfoBatch
                    {
                        BalanceInfos = new List<IVREntityBalanceInfo>(lstBalanceInfoToAlert)
                    });
                    lstBalanceInfoToAlert = new List<IVREntityBalanceInfo>();
                }
                lstBalanceInfoToAlert.Add(vrEntityBalanceInfo);
            };
            VRBalanceAlertRuleLoadEntitiesToClearAlertsContext context = new VRBalanceAlertRuleLoadEntitiesToClearAlertsContext(onBalanceInfoLoaded);
            context.RuleTypeSettings = inputArgument.RuleTypeSettings;
            inputArgument.RuleTypeSettings.Behavior.LoadEntitiesToClearAlerts(context);
            if (lstBalanceInfoToAlert.Count > 0)
            {
                inputArgument.OutputQueue.Enqueue(new VREntityBalanceInfoBatch
                {
                    BalanceInfos = new List<IVREntityBalanceInfo>(lstBalanceInfoToAlert)
                });
            }
        }

        protected override LoadEntitiesToClearAlertsInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new LoadEntitiesToClearAlertsInput
            {
                RuleTypeSettings = this.RuleTypeSettings.Get(context),
                OutputQueue = this.OutputQueue.Get(context)
            };
        }

        protected override void OnBeforeExecute(AsyncCodeActivityContext context, AsyncActivityHandle handle)
        {
            if (this.OutputQueue.Get(context) == null)
                this.OutputQueue.Set(context, new MemoryQueue<VREntityBalanceInfoBatch>());
            base.OnBeforeExecute(context, handle);
        }

        public class VRBalanceAlertRuleLoadEntitiesToClearAlertsContext : IVRBalanceAlertRuleLoadEntitiesToClearAlertsContext
        {
            Action<IVREntityBalanceInfo> _onBalanceInfoLoaded;
            public VRBalanceAlertRuleLoadEntitiesToClearAlertsContext(Action<IVREntityBalanceInfo> onBalanceInfoLoaded)
            {
                _onBalanceInfoLoaded = onBalanceInfoLoaded;
            }
            public void OnBalanceInfoLoaded(IVREntityBalanceInfo balanceInfo)
            {
                _onBalanceInfoLoaded(balanceInfo);
            }

            public VRBalanceAlertRuleTypeSettings RuleTypeSettings
            {
                get;
                set;
            }
        }
    }
}
