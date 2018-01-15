using System;
using System.Collections.Generic;
using System.Linq;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Routing.Entities;

namespace TOne.WhS.Routing.Business
{
    public class RPRouteRuleExecutionContext : IRPRouteRuleExecutionContext
    {
        RouteRule _routeRule;
        internal List<RouteOptionRuleTarget> _supplierZoneOptions = new List<RouteOptionRuleTarget>();
        HashSet<int> _filteredSupplierIds;
        Vanrise.Rules.RuleTree[] _ruleTreesForRouteOptions;
        bool _addBlockedOptions;
        List<SupplierCodeMatchWithRate> _validSupplierCodeMatches;

        public RouteRule RouteRule { get { throw new NotImplementedException(); } }
        public HashSet<int> SaleZoneServiceIds { get; set; }
        public RoutingDatabase RoutingDatabase { get; set; }
        internal List<SupplierCodeMatchWithRate> SupplierCodeMatches { private get; set; }
        internal SupplierCodeMatchesWithRateBySupplier SupplierCodeMatchesBySupplier { private get; set; }

        public RPRouteRuleExecutionContext(RouteRule routeRule, Vanrise.Rules.RuleTree[] ruleTreesForRouteOptions)
        {
            _routeRule = routeRule;
            _ruleTreesForRouteOptions = ruleTreesForRouteOptions;
            _addBlockedOptions = new ConfigManager().GetProductRouteBuildAddBlockedOptions();

            SupplierFilterSettings supplierFilterSettings = new SupplierFilterSettings
            {
                RoutingProductId = routeRule.Criteria.GetRoutingProductId()
            };
            _filteredSupplierIds = SupplierGroupContext.GetFilteredSupplierIds(supplierFilterSettings);
        }

        public bool TryAddSupplierZoneOption(RouteOptionRuleTarget optionTarget)
        {
            var routeOptionRule = GetRouteOptionRule(optionTarget);
            if (routeOptionRule != null)
            {
                optionTarget.ExecutedRuleId = routeOptionRule.RuleId;
                RouteOptionRuleExecutionContext routeOptionRuleExecutionContext = new RouteOptionRuleExecutionContext() { SaleZoneServiceIds = string.Join<int>(",", SaleZoneServiceIds.ToList()), RouteRule = _routeRule };
                routeOptionRule.Settings.Execute(routeOptionRuleExecutionContext, optionTarget);

                if ((optionTarget.BlockOption && !_addBlockedOptions) || optionTarget.FilterOption)
                    return false;
            }
            _supplierZoneOptions.Add(optionTarget);
            return true;
        }

        internal List<RouteOptionRuleTarget> GetSupplierZoneOptions()
        {
            return _supplierZoneOptions;
        }

        public List<SupplierCodeMatchWithRate> GetAllSuppliersCodeMatches()
        {
            if (_validSupplierCodeMatches == null)
            {
                if (_filteredSupplierIds == null)
                    _validSupplierCodeMatches = this.SupplierCodeMatches;
                else
                {
                    _validSupplierCodeMatches = new List<SupplierCodeMatchWithRate>();
                    if (this.SupplierCodeMatches != null)
                    {
                        foreach (var supplierCodeMatch in this.SupplierCodeMatches)
                        {
                            if (_filteredSupplierIds.Contains(supplierCodeMatch.CodeMatch.SupplierId))
                            {
                                _validSupplierCodeMatches.Add(supplierCodeMatch);
                            }
                        }
                    }
                }
            }
            return _validSupplierCodeMatches;
        }

        public List<SupplierCodeMatchWithRate> GetSupplierCodeMatches(int supplierId)
        {
            if (_filteredSupplierIds == null || _filteredSupplierIds.Contains(supplierId))
            {
                if (this.SupplierCodeMatchesBySupplier != null)
                {
                    List<SupplierCodeMatchWithRate> supplierCodeMatches;
                    if (this.SupplierCodeMatchesBySupplier.TryGetValue(supplierId, out supplierCodeMatches))
                        return supplierCodeMatches;
                }
            }
            return null;
        }

        private RouteOptionRule GetRouteOptionRule(RouteOptionRuleTarget targetOption)
        {
            if (_ruleTreesForRouteOptions != null)
            {
                foreach (var ruleTree in _ruleTreesForRouteOptions)
                {
                    var matchRule = ruleTree.GetMatchRule(targetOption) as RouteOptionRule;
                    if (matchRule != null)
                        return matchRule;
                }
            }
            return null;
        }
    }
}
