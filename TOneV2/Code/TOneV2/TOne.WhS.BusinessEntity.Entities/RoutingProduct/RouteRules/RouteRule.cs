using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class RouteRule : Vanrise.Rules.BaseRule, IRuleCustomerCriteria, IRuleCodeCriteria, IRuleSaleZoneCriteria, IRuleRoutingProductCriteria
    {
        public int RouteRuleId { get; set; }

        public RouteCriteria RouteCriteria { get; set; }

        public int TypeConfigId { get; set; }

        public RouteRuleSettings Settings { get; set; }

        public DateTime BeginEffectiveDate { get; set; }

        public DateTime? EndEffectiveDate { get; set; }

        public string Description { get; set; }

        public override bool IsAnyCriteriaExcluded(object target)
        {
            RouteIdentifier routeIdentifier = target as RouteIdentifier;
            if (this.RouteCriteria.ExcludedCodes != null && this.RouteCriteria.ExcludedCodes.Contains(routeIdentifier.Code))
                return true;
            return false;
        }


        public CustomerGroupSettings CustomerGroupSettings
        {
            get { return this.RouteCriteria != null ? this.RouteCriteria.CustomerGroupSettings : null; }
        }

        public CodeCriteriaGroupSettings CodeCriteriaGroupSettings
        {
            get { return this.RouteCriteria != null ? this.RouteCriteria.CodeCriteriaGroupSettings : null; }
        }

        public SaleZoneGroupSettings SaleZoneGroupSettings
        {
            get { return this.RouteCriteria != null ? this.RouteCriteria.SaleZoneGroupSettings : null; }
        }

        public int? RoutingProductId
        {
            get { return this.RouteCriteria != null ? this.RouteCriteria.RoutingProductId : null; }
        }
    }
}
