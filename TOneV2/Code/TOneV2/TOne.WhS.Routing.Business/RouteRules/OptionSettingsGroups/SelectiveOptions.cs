using System;
using System.Collections.Generic;
using System.Linq;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.Routing.Entities;
using Vanrise.Common;

namespace TOne.WhS.Routing.Business.RouteRules.OptionSettingsGroups
{
    public class SelectiveOptions : RouteOptionSettingsGroup
    {
        public override Guid ConfigId { get { return new Guid("999ad07c-5e78-4838-bc72-f17b5771fcd8"); } }
        public List<RouteOptionSettings> Options { get; set; }

        public Dictionary<int, List<RouteOptionFilterSettings>> Filters { get; set; }

        public override bool AreSuppliersIncluded(IRouteRuleAreSuppliersIncludedContext context)
        {
            if (context.SupplierIds == null || context.SupplierIds.Count == 0)
                return true;

            if (Options == null || Options.Count == 0)
                return false;

            foreach (var supplierId in context.SupplierIds)
                if (Options.Any(item => item.SupplierId == supplierId))
                    return true;

            return false;
        }

        public override string GetDescription()
        {
            if (Options == null || Options.Count == 0)
                return null;

            CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
            IEnumerable<string> supplierNames = Options.Select(item => carrierAccountManager.GetCarrierAccountName(item.SupplierId));

            return string.Join(", ", supplierNames);
        }

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
        public override RouteRuleSettings ExtendSuppliersList(RouteRuleSettings routeRuleSettings, List<RouteOption> routeOptions)
        {
            var regularRouteRule = routeRuleSettings.CastWithValidate<RegularRouteRule>("RegularRouteRule");
            var selectiveOptions = regularRouteRule.OptionsSettingsGroup.CastWithValidate<SelectiveOptions>("SelectiveOptions");
            if (routeOptions == null || routeOptions.Count == 0)
                return regularRouteRule;

            if (selectiveOptions.Options == null)
                selectiveOptions.Options = new List<RouteOptionSettings>();

            foreach (var routeOption in routeOptions)
            {
                if (!selectiveOptions.Options.Any(option => option.SupplierId == routeOption.SupplierId))
                {
                    RouteOptionSettings routeOptionSettings = new RouteOptionSettings()
                       {
                           SupplierId = routeOption.SupplierId
                       };

                    selectiveOptions.Options.Add(routeOptionSettings);
                }
            }
            return regularRouteRule;
        }
    }
}