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

        RoutRuleConfiguration _routRuleConfiguration;
        public RoutRuleConfiguration RoutRuleConfiguration
        {
            get
            {
                if (_routRuleConfiguration == null)
                {
                    _routRuleConfiguration = new RoutRuleConfiguration
                    {
                        DefaultFixedOptionLossValue = false,
                        FixedOptionLossType = FixedOptionLossType.RemoveLoss
                    };
                }
                return _routRuleConfiguration;
            }
            set
            {
                _routRuleConfiguration = value;
            }
        }

        public QualityConfiguration QualityConfiguration { get; set; }

        public override bool IsValid(ISettingDataValidationContext context)
        {
            int nbOfDefaults = 0;
            foreach (var routeRuleQualityConfiguration in this.QualityConfiguration.RouteRuleQualityConfigurationList)
            {
                if (routeRuleQualityConfiguration.IsDefault)
                {
                    nbOfDefaults++;
                    if (nbOfDefaults > 1)
                    {
                        context.ErrorMessage = "Only one default route rule quality configuration is permitted.";
                        return false;
                    }
                    if (!routeRuleQualityConfiguration.IsActive)
                    {
                        context.ErrorMessage = "Default route rule quality configuration should be active.";
                        return false;
                    }
                }
            }
            if (nbOfDefaults == 0)
            {
                context.ErrorMessage = "At least one default route rule quality configuration should be added.";
                return false;
            }
            return true;
        }
    }
}
