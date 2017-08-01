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

        public QualityConfiguration QualityConfiguration { get; set; }

        public override bool IsValid(ISettingDataValidationContext context)
        {
            return base.IsValid(context);
        }    
    }
}
