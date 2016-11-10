using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Routing.Entities
{
    public class RouteOptionRuleConfiguration
    {
        public CustomerRouteOptionRuleTypeConfiguration CustomerRouteOptionRuleTypeConfiguration { get; set; }

        public ProductRouteOptionRuleTypeConfiguration ProductRouteOptionRuleTypeConfiguration { get; set; }
    }

    public class CustomerRouteOptionRuleTypeConfiguration
    {
        public Dictionary<Guid, RouteOptionRuleTypeConfiguration> RouteOptionRuleTypeConfiguration { get; set; }
    }

    public class ProductRouteOptionRuleTypeConfiguration
    {
        public Dictionary<Guid, RouteOptionRuleTypeConfiguration> RouteOptionRuleTypeConfiguration { get; set; }
    }

    public class RouteOptionRuleTypeConfiguration
    {
        public bool IsExcluded { get; set; }
    }
}
