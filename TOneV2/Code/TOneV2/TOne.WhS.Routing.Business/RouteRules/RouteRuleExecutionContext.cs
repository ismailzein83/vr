﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Routing.Business.RouteOptionRules;
using TOne.WhS.Routing.Entities;

namespace TOne.WhS.Routing.Business.RouteRules
{
    public class RouteRuleExecutionContext : IRouteRuleExecutionContext
    {
        RouteRule _routeRule;
        internal List<RouteOptionRuleTarget> _options = new List<RouteOptionRuleTarget>();
        HashSet<int> _filteredSupplierIds;
        public RouteRuleExecutionContext(RouteRule routeRule)
        {
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

        public List<SupplierCodeMatchWithRate> GetSupplierCodeMatches(int supplierId)
        {
            if (_filteredSupplierIds == null || _filteredSupplierIds.Contains(supplierId))
            {
                if (this.SupplierCodeMatchBySupplier != null)
                {
                    List<SupplierCodeMatchWithRate> supplierCodeMatches;
                    if (this.SupplierCodeMatchBySupplier.TryGetValue(supplierId, out supplierCodeMatches))
                        return supplierCodeMatches;
                }
            }
            return null;
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
    }
}
