using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Routing.Entities
{
    public abstract class RouteOptionSettingsGroup
    {
        public abstract Guid ConfigId { get; }

        public abstract IEnumerable<RouteOptionSettings> GetOptionSettings(IRouteOptionSettingsContext context);
    }
}
