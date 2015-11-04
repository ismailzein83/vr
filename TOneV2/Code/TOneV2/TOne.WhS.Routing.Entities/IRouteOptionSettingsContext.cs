using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.Routing.Entities
{
    public interface IRouteOptionSettingsContext
    {
        SupplierFilterSettings FilterSettings { get; set; }

        IEnumerable<RouteOptionSettings> GetGroupOptionSettings(RouteOptionSettingsGroup group);
    }
}
