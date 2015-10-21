using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Routing.Business.RouteOptionRules;

namespace TOne.WhS.Routing.Business.RouteRules
{
    public class RouteRuleExecutionContext : IRouteRuleExecutionContext
    {
        RouteRule _routeRule;
        internal List<RouteOptionRuleTarget> _options = new List<RouteOptionRuleTarget>();
        public RouteRuleExecutionContext(RouteRule routeRule)
        {
            _routeRule = routeRule;
        }

        internal List<SupplierCodeMatch> SupplierCodeMatches { private get; set; }

        internal SupplierCodeMatchBySupplier SupplierCodeMatchBySupplier { private get; set; }

        internal SupplierZoneRatesByZone SupplierZoneRates { private get; set; }

        public RouteRule RouteRule
        {
            get
            {
                return _routeRule;
            }
        }

        public int? NumberOfOptions { get; internal set; }

        public bool TryAddOption(RouteOptionRuleTarget optionTarget)
        {
            RouteOptionRuleManager routeOptionRuleManager = new RouteOptionRuleManager();
            var routeOptionRule = routeOptionRuleManager.GetMatchRule(optionTarget);
            if (routeOptionRule != null)
            {
                optionTarget.ExecutedRuleId = routeOptionRule.RuleId;
                RouteOptionRuleExecutionContext routeOptionRuleExecutionContext = new RouteOptionRuleExecutionContext();
                routeOptionRule.Settings.Execute(routeOptionRuleExecutionContext, optionTarget);
                if (optionTarget.BlockOption)
                    return false;
            }
            _options.Add(optionTarget);
            return true;
        }

        public ReadOnlyCollection<RouteOptionRuleTarget> GetOptions()
        {
            return _options.AsReadOnly();
        }

        public List<SupplierCodeMatch> GetSupplierCodeMatches(int supplierId)
        {            
            if(this.SupplierCodeMatchBySupplier != null)
            {
                List<SupplierCodeMatch> supplierCodeMatches;
                if (this.SupplierCodeMatchBySupplier.TryGetValue(supplierId, out supplierCodeMatches))
                    return supplierCodeMatches;
            }
            return null;
        }

        public List<SupplierCodeMatch> GetAllSuppliersCodeMatches()
        {
            return this.SupplierCodeMatches;
        }

        public SupplierZoneRate GetSupplierZoneRate(long supplierZoneId)
        {
            if(this.SupplierZoneRates != null )
            {
                SupplierZoneRate supplierZoneRate;
                if (this.SupplierZoneRates.TryGetValue(supplierZoneId, out supplierZoneRate))
                    return supplierZoneRate;
            }
            return null;
        }
    }
}
