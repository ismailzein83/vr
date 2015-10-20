using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class RouteRule : Vanrise.Rules.BaseRule, IRuleCustomerCriteria, IRuleCodeCriteria, IRuleSaleZoneCriteria, IRuleRoutingProductCriteria
    {
        public RouteCriteria RouteCriteria { get; set; }        

        public RouteRuleSettings Settings { get; set; }

        public string Description { get; set; }

        public override bool IsAnyCriteriaExcluded(object target)
        {
            IRuleCodeTarget ruleCodeTarget = target as IRuleCodeTarget;
            if (this.RouteCriteria.ExcludedCodes != null && this.RouteCriteria.ExcludedCodes.Contains(ruleCodeTarget.Code))
                return true;
            return false;
        }

        public IEnumerable<long> SaleZoneIds
        {
            get { return this.RouteCriteria != null && this.RouteCriteria.SaleZoneGroupSettings != null ? this.RouteCriteria.SaleZoneGroupSettings.GetZoneIds(null) : null; }
        }

        public IEnumerable<CodeCriteria> CodeCriterias
        {
            get { return this.RouteCriteria != null && this.RouteCriteria.CodeCriteriaGroupSettings != null ? this.RouteCriteria.CodeCriteriaGroupSettings.GetCodeCriterias(null) : null; }
        }

        public IEnumerable<int> CustomerIds
        {
            get { return this.RouteCriteria != null && this.RouteCriteria.CustomerGroupSettings != null ? this.RouteCriteria.CustomerGroupSettings.GetCustomerIds(null) : null; }
        }

        public IEnumerable<int> RoutingProductIds
        {
            get { return this.RouteCriteria != null && this.RouteCriteria.RoutingProductId.HasValue ? new List<int> { this.RouteCriteria.RoutingProductId.Value} : null; }
        }
    }
}
