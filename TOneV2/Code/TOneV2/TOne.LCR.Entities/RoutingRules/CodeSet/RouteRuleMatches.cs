using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.LCR.Entities
{
    public class RouteRuleMatches
    {
        public RouteRuleMatches()
        {
            RulesByMatchCodes = new Dictionary<string, List<BaseRouteRule>>();
            RulesByMatchCodeAndSubCodes = new Dictionary<string, List<BaseRouteRule>>();
            RulesByMatchZones = new Dictionary<int, List<BaseRouteRule>>();
            RulesMatchingAllZones = new List<BaseRouteRule>();
            RulesByMatchCarrierAccounts = new Dictionary<string, List<BaseRouteRule>>();
            RulesMatchingAllCarrierAccounts = new List<BaseRouteRule>();
        }

        public Dictionary<string, List<BaseRouteRule>> RulesByMatchCodes { get; private set; }

        public Dictionary<string, List<BaseRouteRule>> RulesByMatchCodeAndSubCodes { get; private set; }

        public Dictionary<int, List<BaseRouteRule>> RulesByMatchZones { get; private set; }

        public List<BaseRouteRule> RulesMatchingAllZones { get; private set; }

        public Dictionary<string, List<BaseRouteRule>> RulesByMatchCarrierAccounts { get; private set; }

        public List<BaseRouteRule> RulesMatchingAllCarrierAccounts { get; private set; }
    }
}
