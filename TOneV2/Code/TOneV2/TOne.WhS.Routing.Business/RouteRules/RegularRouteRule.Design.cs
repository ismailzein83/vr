using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Routing.Business.RouteOptionRules;
using TOne.WhS.Routing.Entities;

namespace TOne.WhS.Routing.Business.RouteRules
{
    public partial class RegularRouteRule
    {
        public RouteOptionSettingsGroup OptionsSettingsGroup { get; set; }

        public RouteRuleOptionOrderSettings OptionOrderSettings { get; set; }

        public RouteRuleOptionFilterSettings OptionFilterSettings { get; set; }

        public RouteRuleOptionPercentageSettings OptionPercentageSettings { get; set; }
    }

}
