using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.Routing.Entities
{
    public class AutomaticRouteRuleSettings : RouteRuleSettings
    {
        public RouteRuleOptionFilterSettings OptionFilterSettings { get; set; }

        public RouteRuleOptionOrderSettings OptionOrderSettings { get; set; }

        public RouteRuleOptionPercentageSettings OptionPercentageSettings { get; set; }
    }
}
