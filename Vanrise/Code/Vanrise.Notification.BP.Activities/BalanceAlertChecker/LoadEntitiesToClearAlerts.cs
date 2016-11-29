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
