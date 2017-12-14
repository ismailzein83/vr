using System;
using System.Collections.Generic;
using Vanrise.Entities;

namespace TOne.WhS.Routing.Entities
{
    public class QualityConfiguration
    {
        public List<RouteRuleQualityConfiguration> RouteRuleQualityConfigurationList { get; set; }
    }

    public class RouteRuleQualityConfiguration
    {
        public Guid QualityConfigurationId { get; set; }

        public VRTimePeriod TimePeriod { get; set; }

        public string Name { get; set; }

        public bool IsActive { get; set; }

        public bool IsDefault { get; set; }

        public string Expression { get; set; }
    }
}