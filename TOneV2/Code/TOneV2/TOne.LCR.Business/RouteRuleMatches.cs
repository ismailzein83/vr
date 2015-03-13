using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.LCR.Entities;

namespace TOne.LCR.Business
{
    public class RouteRuleMatches
    {
        public RouteRuleMatches()
        {
            RulesByMatchCodes = new Dictionary<string, List<RouteRule>>();
            RulesByMatchCodeAndSubCodes = new Dictionary<string, List<RouteRule>>();
            RulesByMatchZones = new Dictionary<int, List<RouteRule>>();
            RulesMatchingAllZones = new List<RouteRule>();
        }

        public Dictionary<string, List<RouteRule>> RulesByMatchCodes { get; private set; }

        public Dictionary<string, List<RouteRule>> RulesByMatchCodeAndSubCodes { get; private set; }

        public int MinSubCodeLength { get; private set; }

        public void SetMinSubCodeLength()
        {
            if (this.RulesByMatchCodeAndSubCodes.Count > 0)
                this.MinSubCodeLength = this.RulesByMatchCodeAndSubCodes.Keys.Min(itm => itm.Length);
            else
                this.MinSubCodeLength = 0;
        }

        public Dictionary<int, List<RouteRule>> RulesByMatchZones { get; private set; }

        public List<RouteRule> RulesMatchingAllZones { get; private set; }
    }
}
