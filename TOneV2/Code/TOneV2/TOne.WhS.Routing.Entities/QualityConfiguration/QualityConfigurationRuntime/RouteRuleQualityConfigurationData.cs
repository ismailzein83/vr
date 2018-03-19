using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Routing.Entities
{
    public abstract class RouteRuleQualityConfigurationData
    {
        public Guid QualityConfigurationId { get; set; }

        public Decimal QualityData { get; set; }
    }

    public class RouteRuleQualityConfigurationDataBatch
    {
        public List<RouteRuleQualityConfigurationData> RoutingQualityConfigurationDataList { get; set; }
    }
}
