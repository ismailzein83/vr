using System;
using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;
using Vanrise.Common;

namespace TOne.WhS.BusinessEntity.Business
{
    public class MarginRuleManager : GenericRuleManager<MarginRule>
    {
        public MarginRuleSettings GetMarginRuleSettings(Guid ruleDefinitionId, GenericRuleTarget target)
        {
            var marginRule = GetMatchRule(ruleDefinitionId, target);
            if (marginRule != null)
                return marginRule.Settings;

            return null;
        }

        public void ApplyMarginRuleSettings(MarginRuleSettings settings, IApplyMarginRuleContext context)
        {
            if (settings == null)
                return;

            settings.ApplyMarginRule(context);
        }
    }
}