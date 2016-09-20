using System;
using System.Collections.Generic;
using System.Linq;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.Routing.Entities;

namespace TOne.WhS.Routing.Business.RouteRules.OptionSettingsGroups
{
    public class ExcludedOptions : RouteOptionSettingsGroup
    {
        public override Guid ConfigId { get { return new Guid("49938afb-0296-40f1-9ef7-9d790cd1eba4"); } }
        public List<RouteOptionSettings> Options { get; set; }

        public override IEnumerable<RouteOptionSettings> GetOptionSettings(IRouteOptionSettingsContext context)
        {
            CarrierAccountManager manager = new CarrierAccountManager();
            var activeSuppliers = manager.GetRoutingActiveSuppliers();
            
            if (activeSuppliers == null || activeSuppliers.Count() == 0)
                return null;

            if (this.Options == null || this.Options.Count == 0)
                return activeSuppliers.Select(itm => new RouteOptionSettings() { SupplierId = itm.SupplierId });

            return activeSuppliers.Where(itm => Options.FirstOrDefault(item => item.SupplierId == itm.SupplierId) == null)
                .Select(itm => new RouteOptionSettings() { SupplierId = itm.SupplierId });
        }
    }
}
