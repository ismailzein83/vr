using System;
using System.Collections.Generic;
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

    public class VRBalanceAlertRuleUpdateBalanceRuleInfosContext : IVRBalanceAlertRuleUpdateBalanceRuleInfosContext
    {
        public List<VRBalanceUpdateRuleInfoPayload> BalanceRuleInfosToUpdate
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

    public class VRBalanceAlertRuleUpdateBalanceLastAlertInfosContext : IVRBalanceAlertRuleUpdateBalanceLastAlertInfosContext
    {
        public List<VRBalanceUpdateLastAlertInfoPayload> BalanceLastAlertInfosToUpdate
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
