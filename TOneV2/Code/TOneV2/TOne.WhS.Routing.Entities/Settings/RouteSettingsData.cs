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

        RouteRuleConfiguration _routeRuleConfiguration;
        public RouteRuleConfiguration RouteRuleConfiguration
        {
            get
            {
                if (_routeRuleConfiguration == null)
                {
                    _routeRuleConfiguration = new RouteRuleConfiguration
                    {
                        FixedOptionLossDefaultValue = false,
                        FixedOptionLossType = FixedOptionLossType.RemoveLoss
                    };
                }
                return _routeRuleConfiguration;
            }
            set
            {
                _routeRuleConfiguration = value;
            }
        }

        public RouteOptionRuleConfiguration RouteOptionRuleConfiguration { get; set; }

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