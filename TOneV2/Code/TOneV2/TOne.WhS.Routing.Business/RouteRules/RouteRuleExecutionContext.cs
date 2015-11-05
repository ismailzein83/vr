using System;
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

        internal List<SupplierCodeMatch> SupplierCodeMatches { private get; set; }

        internal SupplierCodeMatchBySupplier SupplierCodeMatchBySupplier { private get; set; }

        internal SupplierZoneDetailByZone SupplierZoneDetails { private get; set; }

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
            if (_filteredSupplierIds == null || _filteredSupplierIds.Contains(supplierId))
            {
                if (this.SupplierCodeMatchBySupplier != null)
                {
                    List<SupplierCodeMatch> supplierCodeMatches;
                    if (this.SupplierCodeMatchBySupplier.TryGetValue(supplierId, out supplierCodeMatches))
                        return supplierCodeMatches;
                }
            }
            return null;
        }


        List<SupplierCodeMatch> _validSupplierCodeMatches;
        public List<SupplierCodeMatch> GetAllSuppliersCodeMatches()
        {
            if (_validSupplierCodeMatches == null)
            {
                if (_filteredSupplierIds == null)
                    _validSupplierCodeMatches = this.SupplierCodeMatches;
                else
                {
                    _validSupplierCodeMatches = new List<SupplierCodeMatch>();
                    if (this.SupplierCodeMatches != null)
                    {
                        foreach (var supplierCodeMatch in this.SupplierCodeMatches)
                        {
                            if (_filteredSupplierIds.Contains(supplierCodeMatch.SupplierId))
                            {
                                _validSupplierCodeMatches.Add(supplierCodeMatch);
                            }
                        }
                    }
                }                
            }
            return _validSupplierCodeMatches;
        }

        public SupplierZoneDetail GetSupplierZoneDetail(long supplierZoneId)
        {
            if(this.SupplierZoneDetails != null )
            {
                SupplierZoneDetail supplierZoneDetail;
                if (this.SupplierZoneDetails.TryGetValue(supplierZoneId, out supplierZoneDetail))
                    return supplierZoneDetail;
            }
            return null;
        }
    }
}
