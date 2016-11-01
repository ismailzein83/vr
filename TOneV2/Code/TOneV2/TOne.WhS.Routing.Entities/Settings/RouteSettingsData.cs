using System;
using System.Collections.Generic;
using Vanrise.Entities;

namespace TOne.WhS.Routing.Entities
{
    public class RouteSettingsData : SettingData
    {
        public RouteDatabasesToKeep RouteDatabasesToKeep { get; set; }

        public SubProcessSettings SubProcessSettings { get; set; }

        public RouteBuildConfiguration RouteBuildConfiguration { get; set; }

        public RouteOptionRuleConfiguration RouteOptionRuleConfiguration { get; set; }
    }


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
        public bool IsExcluded  { get; set; }
    }

}
 