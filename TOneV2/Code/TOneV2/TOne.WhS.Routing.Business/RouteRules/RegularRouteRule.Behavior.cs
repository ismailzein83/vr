using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Routing.Entities;
using Vanrise.Common;

namespace TOne.WhS.Routing.Business
{
    public partial class RegularRouteRule : RouteRuleSettings
    {
        #region SaleEntity Execution

        public override void ExecuteForSaleEntity(ISaleEntityRouteRuleExecutionContext context, RouteRuleTarget target)
        {
            var options = CreateOptions(context, target);
            if (options != null)
            {
                options = ApplyOptionsOrder(options);
                int optionsAdded = 0;
                foreach (RouteOptionRuleTarget option in options)
                {
                    if(!FilterOption(target, option))
                    {
                        if (context.TryAddOption(option))
                        {
                            optionsAdded++;
                            if (context.NumberOfOptions.HasValue && context.NumberOfOptions.Value == optionsAdded)
                                break;
                        }
                    }                    
                }
                ApplyOptionsPercentage(context.GetOptions());
            }
        }

        private IEnumerable<RouteOptionRuleTarget> CreateOptions(ISaleEntityRouteRuleExecutionContext context, RouteRuleTarget target)
        {
            var options = new List<RouteOptionRuleTarget>();
            if (this.OptionsSettingsGroup != null)
            {
                IRouteOptionSettingsContext routeOptionSettingsContext = new RouteOptionSettingsContext
                {
                    SupplierFilterSettings = new SupplierFilterSettings
                    {
                        RoutingProductId = target.RoutingProductId
                    }
                };
                var optionsSettings = routeOptionSettingsContext.GetGroupOptionSettings(this.OptionsSettingsGroup);
                if (optionsSettings != null)
                {
                    foreach (var optionSettings in optionsSettings)
                    {
                        SupplierCodeMatchWithRate optionSupplierCodeMatch = context.GetSupplierCodeMatch(optionSettings.SupplierId);
                        if (optionSupplierCodeMatch != null)
                        {
                            var option = CreateOption(target, optionSupplierCodeMatch, optionSettings.Percentage);
                            options.Add(option);
                        }
                    }
                }
            }
            else
            {
                var allSuppliersCodeMatches = context.GetAllSuppliersCodeMatches();
                if (allSuppliersCodeMatches != null)
                {
                    foreach (var supplierCodeMatch in allSuppliersCodeMatches)
                    {
                        var option = CreateOption(target, supplierCodeMatch, null);
                        options.Add(option);
                    }
                }
            }
            return options;
        }

        #endregion

        #region Routing Product Execution

        public override void CreateSupplierZoneOptionsForRP(IRPRouteRuleExecutionContext context, RouteRuleTarget target)
        {
            if (this.OptionsSettingsGroup != null)
            {
                IRouteOptionSettingsContext routeOptionSettingsContext = new RouteOptionSettingsContext
                {
                    SupplierFilterSettings = new SupplierFilterSettings
                    {
                        RoutingProductId = target.RoutingProductId
                    }
                };
                var optionsSettings = routeOptionSettingsContext.GetGroupOptionSettings(this.OptionsSettingsGroup);
                if (optionsSettings != null)
                {
                    foreach (var optionSettings in optionsSettings)
                    {
                        List<SupplierCodeMatchWithRate> optionSupplierCodeMatches = context.GetSupplierCodeMatches(optionSettings.SupplierId);
                        if (optionSupplierCodeMatches != null)
                        {
                            foreach (var supplierCodeMatch in optionSupplierCodeMatches)
                            {
                                var option = CreateOption(target, supplierCodeMatch, optionSettings.Percentage);
                                if (!FilterOption(target, option))
                                    context.TryAddSupplierZoneOption(option);
                            }
                        }
                    }
                }
            }
            else
            {
                var allSuppliersCodeMatches = context.GetAllSuppliersCodeMatches();
                if (allSuppliersCodeMatches != null)
                {
                    foreach (var supplierCodeMatch in allSuppliersCodeMatches)
                    {
                        var option = CreateOption(target, supplierCodeMatch, null);
                        if (!FilterOption(target, option))
                            context.TryAddSupplierZoneOption(option);
                    }
                }
            }
        }

        public override void ApplyRuleToRPOptions(IRPRouteRuleExecutionContext context, ref IEnumerable<RPRouteOption> options)
        {
            options = ApplyOptionsOrder(options);
            ApplyOptionsPercentage(options);
        }

        #endregion

        #region Private Methods

        private RouteOptionRuleTarget CreateOption(RouteRuleTarget routeRuleTarget, SupplierCodeMatchWithRate supplierCodeMatchWithRate, Decimal? percentage)
        {
            var supplierCodeMatch = supplierCodeMatchWithRate.CodeMatch;
            var option = new RouteOptionRuleTarget
            {
                RouteTarget = routeRuleTarget,
                SupplierId = supplierCodeMatch.SupplierId,
                SupplierCode = supplierCodeMatch.SupplierCode,
                SupplierZoneId = supplierCodeMatch.SupplierZoneId,
                SupplierRate = supplierCodeMatchWithRate.RateValue,
                EffectiveOn = routeRuleTarget.EffectiveOn
            };
            if (percentage.HasValue)
                option.Percentage = percentage.Value;
            return option;
        }

        private IEnumerable<T> ApplyOptionsOrder<T>(IEnumerable<T> options) where T : IRouteOptionOrderTarget
        {
            if (this.OptionOrderSettings != null)
            {
                var optionOrderContext = new RouteOptionOrderExecutionContext
                {
                    Options = options.VRCast<IRouteOptionOrderTarget>()
                };
                this.OptionOrderSettings.Execute(optionOrderContext);
                options = optionOrderContext.Options.VRCast<T>();
            }
            return options;
        }

        private bool FilterOption(RouteRuleTarget target, RouteOptionRuleTarget option)
        {
            if (this.OptionFilters != null)
            {
                foreach (var optionFilter in this.OptionFilters)
                {
                    var routeOptionFilterExecutionContext = new RouteOptionFilterExecutionContext()
                    {
                        Option = option,
                        SaleRate = target.SaleRate
                    };
                    optionFilter.Execute(routeOptionFilterExecutionContext);
                    if (routeOptionFilterExecutionContext.FilterOption)
                        return true;
                }
            }
            return false;
        }

        private void ApplyOptionsPercentage<T>(IEnumerable<T> options) where T : IRouteOptionPercentageTarget
        {
            if (this.OptionPercentageSettings != null)
            {
                var routeOptionPercentageExecutionContext = new RouteOptionPercentageExecutionContext
                {
                    Options = options.VRCast<IRouteOptionPercentageTarget>()
                };
                this.OptionPercentageSettings.Execute(routeOptionPercentageExecutionContext);
            }
        }

        #endregion
    }
}
