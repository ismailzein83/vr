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
        public override void Execute(IRouteRuleExecutionContext context, RouteRuleTarget target)
        {
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
            if (context.SupplierCodeMatches == null || context.SupplierZoneRates == null)
                return null;
            var options = new List<RouteOptionRuleTarget>();
            if (this.OptionsSettingsGroup != null)
            {
                RouteOptionSettingsContext routeOptionSettingsContext = new RouteOptionSettingsContext();
                var optionsSettings = this.OptionsSettingsGroup.GetOptionSettings(routeOptionSettingsContext);
                if (optionsSettings != null)
                {
                    foreach (var optionSettings in optionsSettings)
                    {
                        List<SupplierCodeMatch> optionSupplierCodeMatches;
                        if (context.SupplierCodeMatches.TryGetValue(optionSettings.SupplierId, out optionSupplierCodeMatches))
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
                foreach (var supplierCodeMatchEntry in context.SupplierCodeMatches)
                {
                    foreach (var supplierCodeMatch in supplierCodeMatchEntry.Value)
                    {
                        TryCreateOption(context, options, target, supplierCodeMatch, null, this.OptionFilterSettings);
                    }
                }
            }
            return options;
        }

        private void TryCreateOption(IRouteRuleExecutionContext context, List<RouteOptionRuleTarget> options, RouteRuleTarget routeRuleTarget, SupplierCodeMatch supplierCodeMatch, Decimal? percentage, RouteRuleOptionFilterSettings optionFilter)
        {
            SupplierZoneRate supplierZoneRate;
            if (context.SupplierZoneRates.TryGetValue(supplierCodeMatch.SupplierZoneId, out supplierZoneRate))
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
