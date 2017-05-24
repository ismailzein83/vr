using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Deal.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;

namespace TOne.WhS.Deal.Business
{
    public class SwapDealBuyRouteRuleManager : Vanrise.Common.Business.VRRuleManager<SwapDealBuyRouteRule, SwapDealBuyRouteRuleDetails, SwapDealBuyRouteRuleSettings>
    {
        public Vanrise.Entities.IDataRetrievalResult<SwapDealBuyRouteRuleDetails> GetFilteredSwapDealBuyRouteRules(Vanrise.Entities.DataRetrievalInput<SwapDealBuyRouteRuleQuery> input)
        {
            var swapDealBuyRouteRules = GetCachedVRRules();

            Func<SwapDealBuyRouteRule, bool> filterExpression = (rule) =>
            {
                if (input.Query != null && input.Query.SwapDealId != rule.Settings.SwapDealId)
                    return false;

                return true;
            };

            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, swapDealBuyRouteRules.ToBigResult(input, filterExpression, VRRuleDetailMapper));
        }

        public IEnumerable<SwapDealBuyRouteRuleExtendedSettingsConfig> GetSwapDealBuyRouteRuleExtendedSettingsConfigs()
        {
            var templateConfigManager = new ExtensionConfigurationManager();
            return templateConfigManager.GetExtensionConfigurations<SwapDealBuyRouteRuleExtendedSettingsConfig>(SwapDealBuyRouteRuleExtendedSettingsConfig.EXTENSION_TYPE);
        }

        protected override Guid GetVRRuleDefinitionId()
        {
            return new ConfigManager().GetSwapDealBuyRouteRuleDefinitionId();
        }

        protected override SwapDealBuyRouteRuleDetails VRRuleDetailMapper(SwapDealBuyRouteRule vrRule)
        {
            return new SwapDealBuyRouteRuleDetails()
            {
                Entity = vrRule
            };
        }
    }
}
