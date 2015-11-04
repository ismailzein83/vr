using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Routing.Business.RouteOptionRules;
using TOne.WhS.Routing.Entities;

namespace TOne.WhS.Routing.Business.RouteRules
{
    public partial class RegularRouteRule : RouteRuleSettings
    {
        public override void Execute(object contextObj, RouteRuleTarget target)
        {
            IRouteRuleExecutionContext context = (IRouteRuleExecutionContext)contextObj;
            var options = CreateOptions(context, target);
            if (options != null)
            {
                if (this.OptionOrderSettings != null)
                    options = this.OptionOrderSettings.Execute(context, options);

                int optionsAdded = 0;
                foreach (RouteOptionWrapper option in options)
                {
                    if (option.OptionFilter != null)
                    {
                        RouteOptionRuleExecutionContext routeOptionRuleExecutionContext = new RouteOptionRuleExecutionContext();
                        option.OptionFilter.Execute(routeOptionRuleExecutionContext, option);
                    }
                    if (!option.FilterOption)
                    {
                        if (context.TryAddOption(option))
                        {
                            optionsAdded++;
                            if (context.NumberOfOptions.HasValue && context.NumberOfOptions.Value == optionsAdded)
                                break;
                        }
                    }
                }

                if (this.OptionPercentageSettings != null)
                    this.OptionPercentageSettings.Execute(context, target);
            }


        }

        private IEnumerable<RouteOptionRuleTarget> CreateOptions(IRouteRuleExecutionContext context, RouteRuleTarget target)
        {            
            var options = new List<RouteOptionRuleTarget>();
            if (this.OptionsSettingsGroup != null)
            {
                IRouteOptionSettingsContext routeOptionSettingsContext = new RouteOptionSettingsContext
                {
                    FilterSettings = new SupplierFilterSettings
                    {
                        RoutingProductId = target.RoutingProductId
                    }
                };
                var optionsSettings = routeOptionSettingsContext.GetGroupOptionSettings(this.OptionsSettingsGroup);
                if (optionsSettings != null)
                {
                    foreach (var optionSettings in optionsSettings)
                    {
                        List<SupplierCodeMatch> optionSupplierCodeMatches = context.GetSupplierCodeMatches(optionSettings.SupplierId);
                        if (optionSupplierCodeMatches != null)
                        {
                            foreach (var supplierCodeMatch in optionSupplierCodeMatches)
                            {
                                TryCreateOption(context, options, target, supplierCodeMatch, optionSettings.Percentage, optionSettings.Filter != null ? optionSettings.Filter : this.OptionFilterSettings);
                            }
                        }
                    }
                }
            }
            else
            {
                var allSuppliersCodeMatches = context.GetAllSuppliersCodeMatches();
                if(allSuppliersCodeMatches != null)
                {
                    foreach (var supplierCodeMatch in allSuppliersCodeMatches)
                    {
                        TryCreateOption(context, options, target, supplierCodeMatch, null, this.OptionFilterSettings);
                    }
                }
            }
            return options;
        }

        private void TryCreateOption(IRouteRuleExecutionContext context, List<RouteOptionRuleTarget> options, RouteRuleTarget routeRuleTarget, SupplierCodeMatch supplierCodeMatch, Decimal? percentage, RouteRuleOptionFilterSettings optionFilter)
        {
            SupplierZoneRate supplierZoneRate = context.GetSupplierZoneRate(supplierCodeMatch.SupplierZoneId);
            if (supplierZoneRate != null)
            {
                var option = new RouteOptionWrapper
                {
                    RouteTarget = routeRuleTarget,
                    SupplierId = supplierCodeMatch.SupplierId,
                    SupplierCode = supplierCodeMatch.SupplierCode,
                    SupplierZoneId = supplierCodeMatch.SupplierZoneId,
                    SupplierRate = supplierZoneRate.Rate,
                    EffectiveOn = routeRuleTarget.EffectiveOn,
                    OptionFilter = optionFilter
                };
                if (percentage.HasValue)
                    option.Percentage = percentage.Value;
                options.Add(option);
            }
        }


        #region Private Classes

        private class RouteOptionWrapper : RouteOptionRuleTarget
        {
            public RouteRuleOptionFilterSettings OptionFilter { get; set; }
        }

        #endregion
    }
}
