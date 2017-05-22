using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Deal.Entities;
using Vanrise.Common;

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

        protected override Guid GetVRRuleDefinitionId()
        {
            Guid vrRuleDefinitionId = new Guid("3baf7ff7-2c85-4297-92de-7d333c27dea5");
            return vrRuleDefinitionId;
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
