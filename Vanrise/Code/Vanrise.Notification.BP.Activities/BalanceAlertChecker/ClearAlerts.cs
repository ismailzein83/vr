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
    public class ClearAlertsInput
    {
        public VRBalanceAlertRuleTypeSettings RuleTypeSettings { get; set; }
        public BaseQueue<VREntityBalanceInfoBatch> InputQueue { get; set; }
    }
    public sealed class ClearAlerts : DependentAsyncActivity<ClearAlertsInput>
    {
        [RequiredArgument]
        public InArgument<VRBalanceAlertRuleTypeSettings> RuleTypeSettings { get; set; }

        [RequiredArgument]
        public InArgument<BaseQueue<VREntityBalanceInfoBatch>> InputQueue { get; set; }

        protected override void DoWork(ClearAlertsInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            
        }

        protected override ClearAlertsInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new ClearAlertsInput
            {
                InputQueue = this.InputQueue.Get(context),
                RuleTypeSettings = this.RuleTypeSettings.Get(context)
            };
        }
    }
}
