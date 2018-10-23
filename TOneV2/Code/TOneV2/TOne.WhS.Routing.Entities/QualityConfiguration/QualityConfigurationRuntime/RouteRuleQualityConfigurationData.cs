using System;
using System.Collections.Generic;

namespace TOne.WhS.Routing.Entities
{
    public abstract class RouteRuleQualityConfigurationData
    {
        public Guid QualityConfigurationId { get; set; }

        public Decimal QualityData { get; set; }

        public int VersionNumber { get; set; }
    }

    public class RouteRuleQualityConfigurationDataBatch
    {
        public List<RouteRuleQualityConfigurationData> RoutingQualityConfigurationDataList { get; set; }
    }
}
