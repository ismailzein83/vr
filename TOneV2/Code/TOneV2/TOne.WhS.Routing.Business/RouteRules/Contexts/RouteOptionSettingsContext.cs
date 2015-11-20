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
    public class RouteOptionSettingsContext : IRouteOptionSettingsContext
    {
        public SupplierFilterSettings SupplierFilterSettings { get; set; }

        public IEnumerable<RouteOptionSettings> GetGroupOptionSettings(RouteOptionSettingsGroup group)
        {
            var allGroupOptions = group.GetOptionSettings(this);
            if (allGroupOptions == null)
                return null;
            HashSet<int> filteredSupplierIds = SupplierGroupContext.GetFilteredSupplierIds(this.SupplierFilterSettings);
            if (filteredSupplierIds != null)
                return allGroupOptions.Where(option => filteredSupplierIds.Contains(option.SupplierId));

            return allGroupOptions;
        }
    }
}
