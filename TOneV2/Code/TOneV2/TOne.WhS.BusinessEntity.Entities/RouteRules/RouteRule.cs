using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class RouteRule : Vanrise.Rules.BaseRule, IRuleCustomerCriteria, IRuleCodeCriteria, IRuleSaleZoneCriteria, IRuleRoutingProductCriteria
    {
        public RouteRuleCriteria Criteria { get; set; }        

        public RouteRuleSettings Settings { get; set; }

        public string Description { get; set; }

        public override bool IsAnyCriteriaExcluded(object target)
        {
            IRuleCodeTarget ruleCodeTarget = target as IRuleCodeTarget;
            if (this.Criteria.ExcludedCodes != null && this.Criteria.ExcludedCodes.Contains(ruleCodeTarget.Code))
                return true;
            return false;
        }

        public IEnumerable<long> SaleZoneIds
        {
            get { return this.Criteria != null && this.Criteria.SaleZoneGroupSettings != null ? this.Criteria.SaleZoneGroupSettings.GetZoneIds(null) : null; }
        }

        public IEnumerable<CodeCriteria> CodeCriterias
        {
            get { return this.Criteria != null && this.Criteria.CodeCriteriaGroupSettings != null ? this.Criteria.CodeCriteriaGroupSettings.GetCodeCriterias(null) : null; }
        }

        public IEnumerable<int> CustomerIds
        {
            get { return this.Criteria != null && this.Criteria.CustomerGroupSettings != null ? this.Criteria.CustomerGroupSettings.GetCustomerIds(null) : null; }
        }

        public IEnumerable<int> RoutingProductIds
        {
            get { return this.Criteria != null && this.Criteria.RoutingProductId.HasValue ? new List<int> { this.Criteria.RoutingProductId.Value} : null; }
        }
    }
}
