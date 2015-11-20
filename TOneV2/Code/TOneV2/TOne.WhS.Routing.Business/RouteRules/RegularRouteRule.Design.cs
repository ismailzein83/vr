using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Routing.Entities;

namespace TOne.WhS.Routing.Business
{
    public partial class RegularRouteRule
    {
        public RouteOptionSettingsGroup OptionsSettingsGroup { get; set; }

        public RouteOptionOrderSettings OptionOrderSettings { get; set; }

        public List<RouteOptionFilterSettings> OptionFilters { get; set; }

        public RouteOptionPercentageSettings OptionPercentageSettings { get; set; }
    }

}
