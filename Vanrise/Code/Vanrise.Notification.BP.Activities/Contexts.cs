using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Notification.Entities;

namespace Vanrise.Notification.BP.Activities
{
    public class VRBalanceAlertThresholdContext : IVRBalanceAlertThresholdContext
    {
        public IVREntityBalanceInfo EntityBalanceInfo
        {
            get;
            set;
        }
    }

    public class VRBalanceAlertRuleCreateRuleTargetContext : IVRBalanceAlertRuleCreateRuleTargetContext
    {

        public IVREntityBalanceInfo EntityBalanceInfo
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
