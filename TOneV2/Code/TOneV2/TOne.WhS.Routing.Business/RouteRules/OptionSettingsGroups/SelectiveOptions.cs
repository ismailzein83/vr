using System;
using System.Collections.Generic;
using TOne.WhS.Routing.Entities;
using Vanrise.Common;

namespace TOne.WhS.Routing.Business.RouteRules.OptionSettingsGroups
{
    public class SelectiveOptions : RouteOptionSettingsGroup
    {
        public override Guid ConfigId { get { return new Guid("999ad07c-5e78-4838-bc72-f17b5771fcd8"); } }
        public List<RouteOptionSettings> Options { get; set; }

        public Dictionary<int, List<RouteOptionFilterSettings>> Filters { get; set; }

        public override IEnumerable<RouteOptionSettings> GetOptionSettings(IRouteOptionSettingsContext context)
        {
            return this.Options;
        }

        public override bool IsOptionFiltered(IRouteOptionFilterExecutionContext context)
        {
            if (Filters == null || Filters.Count == 0)
                return false;

            List<RouteOptionFilterSettings> supplierFilters = Filters.GetRecord(context.SupplierId);
            if (supplierFilters == null || supplierFilters.Count == 0)
                return false;

            foreach (var filter in supplierFilters)
            {
                filter.Execute(context);
                if (context.FilterOption)
                    return true;
            }

            return false;
        }
    }
}