using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Routing.Entities;

namespace TOne.WhS.Routing.Business.RouteRules.OptionSettingsGroups
{
    public class SelectiveOptions : RouteOptionSettingsGroup
    {
        public const int ExtensionConfigId = 13;
        public List<RouteOptionSettings> Options { get; set; }
        
        public override IEnumerable<RouteOptionSettings> GetOptionSettings(IRouteOptionSettingsContext context)
        {
            return this.Options;
        }
    }
}
