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
        public override Guid ConfigId { get { return new Guid("999ad07c-5e78-4838-bc72-f17b5771fcd8"); } }
        public List<RouteOptionSettings> Options { get; set; }
        
        public override IEnumerable<RouteOptionSettings> GetOptionSettings(IRouteOptionSettingsContext context)
        {
            return this.Options;
        }
    }
}
