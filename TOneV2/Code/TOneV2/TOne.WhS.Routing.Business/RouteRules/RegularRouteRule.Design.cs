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
        public override Guid ConfigId { get { return new Guid("5a492aa2-9642-453c-8b18-967d745ad812"); } }

        public RouteOptionSettingsGroup OptionsSettingsGroup { get; set; }

        public List<RouteOptionOrderSettings> OptionOrderSettings { get; set; }

        public List<RouteOptionFilterSettings> OptionFilters { get; set; }

        public RouteOptionPercentageSettings OptionPercentageSettings { get; set; }
    }

}
