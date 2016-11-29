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
            //List<LiveBalanceNextThresholdUpdateEntity> liveBalances = context.BalanceRuleInfosToUpdate.Select(s => new LiveBalanceNextThresholdUpdateEntity
            //{
            //    AccountId = long.Parse(s.EntityBalanceInfo.EntityId),
            //    AccountTypeId = (s.EntityBalanceInfo as LiveBalance).AccountTypeId,
            //    AlertRuleId = s.AlertRuleId != null && s.AlertRuleId.HasValue ? s.AlertRuleId.Value : 0,
            //    NextAlertThreshold = s.NextAlertThreshold != null && s.NextAlertThreshold.HasValue ? s.NextAlertThreshold.Value : 0

            //}).ToList();

            List<LiveBalanceNextThresholdUpdateEntity> liveBalances = GenerateLiveBalances(context.BalanceRuleInfosToUpdate);

            ILiveBalanceDataManager dataManager = AccountBalanceDataManagerFactory.GetDataManager<ILiveBalanceDataManager>();
            dataManager.UpdateBalanceRuleInfos(liveBalances);
        }

        List<LiveBalanceNextThresholdUpdateEntity> GenerateLiveBalances(List<VRBalanceUpdateRuleInfoPayload> balanceRuleInfosToUpdate)
        {
            List<LiveBalanceNextThresholdUpdateEntity> lstLiveBalanceNextThresholdUpdateEntity = new List<LiveBalanceNextThresholdUpdateEntity>();

            foreach (var balanceRuleInfo in balanceRuleInfosToUpdate)
            {
                LiveBalanceNextThresholdUpdateEntity balanceEntity = new LiveBalanceNextThresholdUpdateEntity
                {
                    AccountId = long.Parse(balanceRuleInfo.EntityBalanceInfo.EntityId),
                    AccountTypeId = (balanceRuleInfo.EntityBalanceInfo as LiveBalance).AccountTypeId,
                    AlertRuleId = balanceRuleInfo.AlertRuleId != null && balanceRuleInfo.AlertRuleId.HasValue ? balanceRuleInfo.AlertRuleId.Value : 0,
                    NextAlertThreshold = balanceRuleInfo.NextAlertThreshold != null && balanceRuleInfo.NextAlertThreshold.HasValue ? balanceRuleInfo.NextAlertThreshold.Value : 0
                };
                lstLiveBalanceNextThresholdUpdateEntity.Add(balanceEntity);
            }

            return lstLiveBalanceNextThresholdUpdateEntity;
        }

        public override void UpdateBalanceLastAlertInfos(IVRBalanceAlertRuleUpdateBalanceLastAlertInfosContext context)
        {
            throw new NotImplementedException();
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
