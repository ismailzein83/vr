using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class RouteOptionRule : Vanrise.Rules.BaseRule, IRuleCustomerCriteria, IRuleCodeCriteria, IRuleSaleZoneCriteria, IRuleRoutingProductCriteria
    {
        public RouteOptionRuleCriteria Criteria { get; set; }

        public int TypeConfigId { get; set; }

        public RouteOptionRuleSettings Settings { get; set; }

        public DateTime BeginEffectiveDate { get; set; }

        public DateTime? EndEffectiveDate { get; set; }

        public string Description { get; set; }

        public override bool IsAnyCriteriaExcluded(object target)
        {
            RouteIdentifier routeIdentifier = target as RouteIdentifier;
            if (this.Criteria.ExcludedCodes != null && this.Criteria.ExcludedCodes.Contains(routeIdentifier.Code))
                return true;
            return false;
        }

        public CustomerGroupSettings CustomerGroupSettings
        {
            get { return this.Criteria != null ? this.Criteria.CustomerGroupSettings : null; }
        }

        public CodeCriteriaGroupSettings CodeCriteriaGroupSettings
        {
            get { return this.Criteria != null ? this.Criteria.CodeCriteriaGroupSettings : null; }
        }

        public SaleZoneGroupSettings SaleZoneGroupSettings
        {
            get { return this.Criteria != null ? this.Criteria.SaleZoneGroupSettings : null; }
        }

        public int? RoutingProductId
        {
            get { return this.Criteria != null ? this.Criteria.RoutingProductId : null; }
        }
    }
}
