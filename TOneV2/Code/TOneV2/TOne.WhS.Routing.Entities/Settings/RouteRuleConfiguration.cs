using System;
using System.Collections.Generic;

namespace TOne.WhS.Routing.Entities
{
    public enum FixedOptionLossType { RemoveLoss = 0, AcceptLoss = 1 }
    public class RouteRuleConfiguration
    {
        public FixedOptionLossType FixedOptionLossType { get; set; }

        public bool FixedOptionLossDefaultValue { get; set; }

        public List<RouteRuleCriteriaPriority> RuleCriteriasPriority { get; set; }
    }
}