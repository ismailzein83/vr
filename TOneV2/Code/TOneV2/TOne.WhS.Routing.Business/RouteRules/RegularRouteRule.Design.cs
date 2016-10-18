using System;
using System.Collections.Generic;
using TOne.WhS.Routing.Entities;

namespace TOne.WhS.Routing.Business
{
    public enum OrderType { Percentage = 0, Sequential = 1 }
    public partial class RegularRouteRule
    {
        public override Guid ConfigId { get { return new Guid("5a492aa2-9642-453c-8b18-967d745ad812"); } }
        
        public OrderType OrderType { get; set; }
        
        public RouteOptionSettingsGroup OptionsSettingsGroup { get; set; }

        public List<RouteOptionOrderSettings> OptionOrderSettings { get; set; }

        public List<RouteOptionFilterSettings> OptionFilters { get; set; }

        public RouteOptionPercentageSettings OptionPercentageSettings { get; set; }
    }
}