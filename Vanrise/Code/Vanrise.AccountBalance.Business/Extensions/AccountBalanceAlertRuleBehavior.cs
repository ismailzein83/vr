using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.AccountBalance.Data;
using Vanrise.AccountBalance.Entities;
using Vanrise.GenericData.Entities;
using Vanrise.Notification.Entities;

namespace Vanrise.AccountBalance.Business.Extensions
{
    public class AccountBalanceAlertRuleBehavior : VRBalanceAlertRuleBehavior
    {
        public override void LoadBalanceInfos(IVRBalanceAlertRuleLoadBalanceInfosContext context)
        {
            ILiveBalanceDataManager dataManager = AccountBalanceDataManagerFactory.GetDataManager<ILiveBalanceDataManager>();
            dataManager.GetLiveBalanceAccounts(context.OnBalanceInfoLoaded);
        }

        public override GenericData.Entities.GenericRuleTarget CreateRuleTarget(IVRBalanceAlertRuleCreateRuleTargetContext context)
        {
            var ruleTypeSettings = GetRuleTypeSettings(context);
            long accountId = (context.EntityBalanceInfo as LiveBalance).AccountId;
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
            List<LiveBalanceNextThresholdUpdateEntity> lstLiveBalanceNextThresholdUpdateEntity = new List<LiveBalanceNextThresholdUpdateEntity>();

            foreach (var balanceRuleInfo in context.BalanceRuleInfosToUpdate)
            {
                LiveBalance entityBalanceInfo = (balanceRuleInfo.EntityBalanceInfo as LiveBalance);
                LiveBalanceNextThresholdUpdateEntity balanceEntity = new LiveBalanceNextThresholdUpdateEntity
                {
                    AccountId = entityBalanceInfo.AccountId,
                    AccountTypeId = entityBalanceInfo.AccountTypeId,
                    AlertRuleId = balanceRuleInfo.AlertRuleId,
                    NextAlertThreshold = balanceRuleInfo.NextAlertThreshold,
                    ThresholdActionIndex = balanceRuleInfo.ThresholdActionIndex
                };
                lstLiveBalanceNextThresholdUpdateEntity.Add(balanceEntity);
            }
            ILiveBalanceDataManager dataManager = AccountBalanceDataManagerFactory.GetDataManager<ILiveBalanceDataManager>();
            dataManager.UpdateBalanceRuleInfos(lstLiveBalanceNextThresholdUpdateEntity);
        }

        public override void UpdateBalanceLastAlertInfos(IVRBalanceAlertRuleUpdateBalanceLastAlertInfosContext context)
        {
            ILiveBalanceDataManager dataManager = AccountBalanceDataManagerFactory.GetDataManager<ILiveBalanceDataManager>();
            List<LiveBalanceLastThresholdUpdateEntity> lstLiveBalanceNextThresholdUpdateEntity = new List<LiveBalanceLastThresholdUpdateEntity>();
            foreach (var infoToUpdate in context.BalanceLastAlertInfosToUpdate)
            {
                LiveBalance entityBalanceInfo = (infoToUpdate.EntityBalanceInfo as LiveBalance);
                LiveBalanceLastThresholdUpdateEntity balanceEntity = new LiveBalanceLastThresholdUpdateEntity
                {
                    AccountId = entityBalanceInfo.AccountId,
                    AccountTypeId = entityBalanceInfo.AccountTypeId,
                    LastExecutedActionThreshold = infoToUpdate.LastExecutedAlertThreshold,
                    ActiveAlertThresholds = infoToUpdate.ActiveAlertThresholds
                };
                lstLiveBalanceNextThresholdUpdateEntity.Add(balanceEntity);
            }
            dataManager.UpdateBalanceLastAlertInfos(lstLiveBalanceNextThresholdUpdateEntity);
        }

        public override void LoadEntitiesToAlert(IVRBalanceAlertRuleLoadEntitiesToAlertContext context)
        {
            ILiveBalanceDataManager dataManager = AccountBalanceDataManagerFactory.GetDataManager<ILiveBalanceDataManager>();
            dataManager.GetLiveBalancesToAlert(context.OnBalanceInfoLoaded);
        }

        public override void LoadEntitiesToClearAlerts(IVRBalanceAlertRuleLoadEntitiesToClearAlertsContext context)
        {
            ILiveBalanceDataManager dataManager = AccountBalanceDataManagerFactory.GetDataManager<ILiveBalanceDataManager>();
            dataManager.GetLiveBalancesToAlert(context.OnBalanceInfoLoaded);
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
