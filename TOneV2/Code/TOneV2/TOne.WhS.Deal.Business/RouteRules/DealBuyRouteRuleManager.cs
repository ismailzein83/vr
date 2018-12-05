using System;
using System.Collections.Generic;
using TOne.WhS.Deal.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;

namespace TOne.WhS.Deal.Business
{
    public class DealBuyRouteRuleManager : Vanrise.Common.Business.VRRuleManager<DealBuyRouteRule, DealBuyRouteRuleDetails, DealBuyRouteRuleSettings>
    {
        public Vanrise.Entities.IDataRetrievalResult<DealBuyRouteRuleDetails> GetFilteredDealBuyRouteRules(Vanrise.Entities.DataRetrievalInput<DealBuyRouteRuleQuery> input)
        {
            var dealBuyRouteRules = GetCachedVRRules();

            Func<DealBuyRouteRule, bool> filterExpression = (rule) =>
            {
                if (input.Query != null && input.Query.DealId != rule.Settings.DealId)
                    return false;

                return true;
            };

            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, dealBuyRouteRules.ToBigResult(input, filterExpression, VRRuleDetailMapper));
        }

        public IEnumerable<DealBuyRouteRuleExtendedSettingsConfig> GetDealBuyRouteRuleExtendedSettingsConfigs()
        {
            var templateConfigManager = new ExtensionConfigurationManager();
            return templateConfigManager.GetExtensionConfigurations<DealBuyRouteRuleExtendedSettingsConfig>(DealBuyRouteRuleExtendedSettingsConfig.EXTENSION_TYPE);
        }

        public int GetCarrierAccountId(int dealId)
        {
            var deal = new DealDefinitionManager().GetDealDefinition(dealId);
            return deal.Settings.GetCarrierAccountId();
        }

        public bool DeleteDealBuyRouteRules (int dealId)
        {
            var dealBuyRouteRules = GetCachedVRRules();

            if (dealBuyRouteRules.Values != null && dealBuyRouteRules.Values.Count > 0)
            {
                List<long> ruleIdsToBeDeleted = new List<long>();
                foreach (DealBuyRouteRule rule in dealBuyRouteRules.Values)
                {
                    if (dealId == rule.Settings.DealId)
                        ruleIdsToBeDeleted.Add(rule.VRRuleId);
                }

                if (ruleIdsToBeDeleted.Count > 0)
                    return base.SetRulesDeleted(ruleIdsToBeDeleted).Result == Vanrise.Entities.DeleteOperationResult.Succeeded;
            }
            return true;
        }
        protected override Guid GetVRRuleDefinitionId()
        {
            return new ConfigManager().GetDealBuyRouteRuleDefinitionId();
        }

        protected override DealBuyRouteRuleDetails VRRuleDetailMapper(DealBuyRouteRule vrRule)
        {
            Dictionary<Guid, DealBuyRouteRuleExtendedSettingsConfig> dealBuyRouteRuleExtendedSettingsConfigsDict = GetDealBuyRouteRuleExtendedSettingsConfigsDict();
            DealBuyRouteRuleExtendedSettingsConfig dealBuyRouteRuleExtendedSettingsConfig = dealBuyRouteRuleExtendedSettingsConfigsDict.GetRecord(vrRule.Settings.ExtendedSettings.ConfigId);

            return new DealBuyRouteRuleDetails()
            {
                VRRuleId = vrRule.VRRuleId,
                DealId = vrRule.Settings.DealId,
                Description = vrRule.Settings.Description,
                RuleType = dealBuyRouteRuleExtendedSettingsConfig.Title,
                Settings = vrRule.Settings.ExtendedSettings.GetDescription(),
                BED = vrRule.Settings.BED,
                EED = vrRule.Settings.EED
            };
        }

        private Dictionary<Guid, DealBuyRouteRuleExtendedSettingsConfig> GetDealBuyRouteRuleExtendedSettingsConfigsDict()
        {
            ExtensionConfigurationManager manager = new ExtensionConfigurationManager();
            return manager.GetExtensionConfigurationsByType<DealBuyRouteRuleExtendedSettingsConfig>(DealBuyRouteRuleExtendedSettingsConfig.EXTENSION_TYPE);
        }
    }
}