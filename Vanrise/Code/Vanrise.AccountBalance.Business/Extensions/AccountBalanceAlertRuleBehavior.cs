﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;
using Vanrise.Notification.Entities;

namespace Vanrise.AccountBalance.Business.Extensions
{
    public class AccountBalanceAlertRuleBehavior : VRBalanceAlertRuleBehavior
    {
        public override void LoadBalanceInfos(IVRBalanceAlertRuleLoadBalanceInfosContext context)
        {
            throw new NotImplementedException();
        }

        public override GenericData.Entities.GenericRuleTarget CreateRuleTarget(IVRBalanceAlertRuleCreateRuleTargetContext context)
        {
            var ruleTypeSettings = GetRuleTypeSettings(context);
            long accountId = (context.EntityBalanceInfo as Vanrise.AccountBalance.Entities.LiveBalance).AccountId;
            var accountManager = new AccountManager();
            var account = accountManager.GetAccount(ruleTypeSettings.AccountTypeId, accountId);
            GenericRuleTarget ruleTarget = new GenericRuleTarget
            {
                EffectiveOn = DateTime.Now,
                Objects = new Dictionary<string, dynamic> { { "Account", account } }
            };
            return ruleTarget;
        }

        public override void UpdateBalanceRuleInfos(IVRBalanceAlertRuleUpdateBalanceRuleInfosContext context)
        {
            throw new NotImplementedException();
        }

        public override void UpdateBalanceLastAlertInfos(IVRBalanceAlertRuleUpdateBalanceLastAlertInfosContext context)
        {
            throw new NotImplementedException();
        }

        public override void LoadEntitiesToAlert(IVRBalanceAlertRuleLoadEntitiesToAlertContext context)
        {
            throw new NotImplementedException();
        }

        public override void LoadEntitiesToClearAlerts(IVRBalanceAlertRuleLoadEntitiesToClearAlertsContext context)
        {
            throw new NotImplementedException();
        }

        #region Private Methods

        private AccountBalanceAlertRuleTypeSettings GetRuleTypeSettings(IVRBalanceAlertRuleBehaviorContext context)
        {
            if (context.RuleTypeSettings == null)
                throw new ArgumentNullException("context.RuleTypeSettings");
            AccountBalanceAlertRuleTypeSettings ruleTypeSettings = context.RuleTypeSettings as AccountBalanceAlertRuleTypeSettings;
            if (ruleTypeSettings == null)
                throw new Exception(String.Format("context.RuleTypeSettings is not of type AccountBalanceAlertRuleTypeSettings. it is of type '{0}'", context.RuleTypeSettings.GetType()));
            return ruleTypeSettings;
        }

        #endregion
    }
}
