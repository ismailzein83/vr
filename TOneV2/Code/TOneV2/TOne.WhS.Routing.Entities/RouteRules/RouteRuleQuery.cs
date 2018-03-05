using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Routing.Entities
{
    public class RouteRuleQuery
    {
        public string Name { get; set; }

        public int? RoutingProductId { get; set; }

        public string Code { get; set; }

        public IEnumerable<int> CustomerIds { get; set; }

        public IEnumerable<long> SaleZoneIds { get; set; }

        public IEnumerable<int> CountryIds { get; set; }

        public DateTime? EffectiveOn { get; set; }

        public List<Guid> RouteRuleSettingsConfigIds { get; set; }

        public List<int> LinkedRouteRuleIds { get; set; }

        public List<IRouteRuleFilter> Filters { get; set; }

        public bool IsManagementScreen { get; set; }
    }

    public interface IRouteRuleFilter
    {
        bool IsMatched(IRouteRuleFilterContext context);
    }

    public interface IRouteRuleFilterContext
    {
        RouteRule RouteRule { get; }
    }

    public class RouteRuleFilterContext : IRouteRuleFilterContext
    {
        public RouteRule RouteRule { get; set; }
    }
}
