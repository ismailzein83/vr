using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class RouteRule : Vanrise.Rules.BaseRule, IRouteCriteria
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

    }
}
