using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;

namespace Vanrise.Notification.Entities
{
    public abstract class VRBalanceAlertRuleBehavior
    {
        public abstract void LoadBalanceInfos(IVRBalanceAlertRuleLoadBalanceInfosContext context);

        public abstract GenericRuleTarget CreateRuleTarget(IVRBalanceAlertRuleCreateRuleTargetContext context);

        public abstract void UpdateBalanceRuleInfos(IVRBalanceAlertRuleUpdateBalanceRuleInfosContext context);

        public abstract void UpdateBalanceLastAlertInfos(IVRBalanceAlertRuleUpdateBalanceLastAlertInfosContext context);

        public abstract void LoadEntitiesToAlert(IVRBalanceAlertRuleLoadEntitiesToAlertContext context);

        public abstract void LoadEntitiesToClearAlerts(IVRBalanceAlertRuleLoadEntitiesToClearAlertsContext context);
    }

    public interface IVRBalanceAlertRuleBehaviorContext
    {
        VRBalanceAlertRuleTypeSettings RuleTypeSettings { get; }
    }

    public interface IVRBalanceAlertRuleLoadBalanceInfosContext : IVRBalanceAlertRuleBehaviorContext
    {
        void OnBalanceInfoLoaded(IVREntityBalanceInfo balanceInfo);
    }

    public interface IVRBalanceAlertRuleCreateRuleTargetContext : IVRBalanceAlertRuleBehaviorContext
    {
        IVREntityBalanceInfo EntityBalanceInfo { get; }
    }

    public interface IVRBalanceAlertRuleUpdateBalanceRuleInfosContext : IVRBalanceAlertRuleBehaviorContext
    {
        List<VRBalanceUpdateRuleInfoPayload> BalanceRuleInfosToUpdate { get; }
    }

    public interface IVRBalanceAlertRuleUpdateBalanceLastAlertInfosContext : IVRBalanceAlertRuleBehaviorContext
    {
        List<VRBalanceUpdateLastAlertInfoPayload> BalanceLastAlertInfosToUpdate { get; }
    }

    public interface IVRBalanceAlertRuleLoadEntitiesToAlertContext : IVRBalanceAlertRuleBehaviorContext
    {
        void OnBalanceInfoLoaded(IVREntityBalanceInfo balanceInfo);
    }

    public interface IVRBalanceAlertRuleLoadEntitiesToClearAlertsContext : IVRBalanceAlertRuleBehaviorContext
    {
        void OnBalanceInfoLoaded(IVREntityBalanceInfo balanceInfo);
    }
}