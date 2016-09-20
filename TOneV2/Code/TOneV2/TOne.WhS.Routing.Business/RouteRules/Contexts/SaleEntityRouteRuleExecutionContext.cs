﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Routing.Entities;

namespace TOne.WhS.Routing.Business
{
    public class SaleEntityRouteRuleExecutionContext : ISaleEntityRouteRuleExecutionContext
    {
        RouteRule _routeRule;
        internal List<RouteOptionRuleTarget> _options = new List<RouteOptionRuleTarget>();
        HashSet<int> _filteredSupplierIds;
        Vanrise.Rules.RuleTree[] _ruleTreesForRouteOptions;
        public SaleEntityRouteRuleExecutionContext(RouteRule routeRule, Vanrise.Rules.RuleTree[] ruleTreesForRouteOptions)
        {
            _ruleTreesForRouteOptions = ruleTreesForRouteOptions;
            _routeRule = routeRule;
            SupplierFilterSettings supplierFilterSettings = new SupplierFilterSettings
            {
                RoutingProductId = routeRule.Criteria.RoutingProductId
            };
            _filteredSupplierIds = SupplierGroupContext.GetFilteredSupplierIds(supplierFilterSettings);
        }

        internal List<SupplierCodeMatchWithRate> SupplierCodeMatches { private get; set; }

        internal SupplierCodeMatchWithRateBySupplier SupplierCodeMatchBySupplier { private get; set; }

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
            var routeOptionRule = GetRouteOptionRule(optionTarget);
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

        internal RouteOption CreateOptionFromTarget(RouteOptionRuleTarget targetOption)
        {
            var routeOptionRule = GetRouteOptionRule(targetOption);
            
            if (routeOptionRule != null)
            {
                targetOption.ExecutedRuleId = routeOptionRule.RuleId;
                RouteOptionRuleExecutionContext routeOptionRuleExecutionContext = new RouteOptionRuleExecutionContext();
                routeOptionRule.Settings.Execute(routeOptionRuleExecutionContext, targetOption);                
            }
            RouteOption routeOption = new RouteOption
            {
                SupplierId = targetOption.SupplierId,
                SupplierCode = targetOption.SupplierCode,
                SupplierZoneId = targetOption.SupplierZoneId,
                SupplierRate = targetOption.SupplierRate,
                Percentage = targetOption.Percentage,
                IsBlocked = targetOption.BlockOption,
                ExecutedRuleId = targetOption.ExecutedRuleId
            };
            targetOption.ExecutedRuleId = null;
            targetOption.BlockOption = false;
            return routeOption;
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

        public ReadOnlyCollection<RouteOptionRuleTarget> GetOptions()
        {
            return _options.AsReadOnly();
        }


        List<SupplierCodeMatchWithRate> _validSupplierCodeMatches;
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


        public SupplierCodeMatchWithRate GetSupplierCodeMatch(int supplierId)
        {
            if (_filteredSupplierIds == null || _filteredSupplierIds.Contains(supplierId))
            {
                if (this.SupplierCodeMatchBySupplier != null)
                {
                    SupplierCodeMatchWithRate supplierCodeMatch;
                    if (this.SupplierCodeMatchBySupplier.TryGetValue(supplierId, out supplierCodeMatch))
                        return supplierCodeMatch;
                }
            }
            return null;
        }
    }
}
