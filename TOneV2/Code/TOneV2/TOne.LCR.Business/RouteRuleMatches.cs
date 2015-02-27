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
            RulesByMatchCodes = new Dictionary<string, List<BaseRouteRule>>();
            RulesByMatchCodeAndSubCodes = new Dictionary<string, List<BaseRouteRule>>();
            RulesByMatchZones = new Dictionary<int, List<BaseRouteRule>>();
            RulesMatchingAllZones = new List<BaseRouteRule>();
            RulesByMatchCarrierAccounts = new Dictionary<string, List<BaseRouteRule>>();
            RulesMatchingAllCarrierAccounts = new List<BaseRouteRule>();
        }

        public Dictionary<string, List<BaseRouteRule>> RulesByMatchCodes { get; private set; }

        public Dictionary<string, List<BaseRouteRule>> RulesByMatchCodeAndSubCodes { get; private set; }

        public int MinSubCodeLength { get; private set; }

        public void SetMinSubCodeLength()
        {
            this.MinSubCodeLength = this.RulesByMatchCodeAndSubCodes.Keys.Min(itm => itm.Length);
        }

        public Dictionary<int, List<BaseRouteRule>> RulesByMatchZones { get; private set; }

        public List<BaseRouteRule> RulesMatchingAllZones { get; private set; }

        public Dictionary<string, List<BaseRouteRule>> RulesByMatchCarrierAccounts { get; private set; }

        public List<BaseRouteRule> RulesMatchingAllCarrierAccounts { get; private set; }
    }

    public class CustomerRouteRulesMatches
    {
        public CustomerRouteRulesMatches()
        {

        }

        public RouteRuleMatches BeforeLCRRules { get; set; }

        public RouteRuleMatches AfterLCRRules { get; set; }

        public RouteRuleMatches EndRules { get; set; }
    }

    public class SupplierRouteOptionRulesMatches
    {
        public RouteRuleMatches Rules { get; set; }
    }
}
