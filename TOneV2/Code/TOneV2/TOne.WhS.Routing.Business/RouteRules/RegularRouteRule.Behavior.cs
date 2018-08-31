using System;
using System.Collections.Generic;
using System.Linq;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Routing.Business.RouteRules.OptionSettingsGroups;
using TOne.WhS.Routing.Business.RouteRules.Orders;
using TOne.WhS.Routing.Entities;
using Vanrise.Common;

namespace TOne.WhS.Routing.Business
{
    public partial class RegularRouteRule : RouteRuleSettings
    {
        #region Central Execution

        public override bool UseOrderedExecution { get { return true; } }

        #endregion

        #region SaleEntity Execution

        public override RouteRuleSettings BuildLinkedRouteRuleSettings(ILinkedRouteRuleContext context)
        {
            RegularRouteRule regularRouteRule = new RegularRouteRule()
            {
                OrderType = this.OrderType,
                OptionPercentageSettings = Vanrise.Common.Utilities.CloneObject<RouteOptionPercentageSettings>(this.OptionPercentageSettings),
                OptionOrderSettings = Vanrise.Common.Utilities.CloneObject<List<RouteOptionOrderSettings>>(this.OptionOrderSettings),
                OptionFilters = Vanrise.Common.Utilities.CloneObject<List<RouteOptionFilterSettings>>(this.OptionFilters),
            };

            if (context.RouteOptions != null && context.RouteOptions.Count > 0)
            {
                List<RouteOptionSettings> options = new List<RouteOptionSettings>();
                foreach (RouteOption routeOption in context.RouteOptions)
                {
                    RouteOptionSettings optionSettings = new RouteOptionSettings()
                    {
                        SupplierId = routeOption.SupplierId
                    };
                    options.Add(optionSettings);
                }
                regularRouteRule.OptionsSettingsGroup = new SelectiveOptions() { Options = options };
            }

            return regularRouteRule;
        }

        public override List<RouteOptionRuleTarget> GetOrderedOptions(ISaleEntityRouteRuleExecutionContext context, RouteRuleTarget target)
        {
            var options = CreateOptions(context, target);
            if (options != null)
                return ApplyOptionsOrder(options, context.RoutingDatabase);
            else
                return null;
        }

        public override void GetQualityConfigurationIds(IRouteRuleQualityContext context)
        {
            List<Guid> qualityConfigurationIds = new List<Guid>();

            if (OptionOrderSettings != null)
            {
                foreach (var optionOrderSetting in OptionOrderSettings)
                {
                    if (optionOrderSetting.ConfigId == OptionOrderByQuality.s_ConfigId)
                    {
                        OptionOrderByQuality optionOrderByQuality = optionOrderSetting.CastWithValidate<OptionOrderByQuality>("optionOrderSetting");
                        if (!optionOrderByQuality.QualityConfigurationId.HasValue)
                            context.IsDefault = true;
                        else
                            qualityConfigurationIds.Add(optionOrderByQuality.QualityConfigurationId.Value);
                    }
                }
            }

            context.QualityConfigurationIds = qualityConfigurationIds.Count > 0 ? qualityConfigurationIds : null;
        }

        public override void CheckOptionFilter(ISaleEntityRouteRuleExecutionContext context, TOne.WhS.Routing.Entities.RouteRuleTarget target, BaseRouteOptionRuleTarget option)
        {
            FilterOption(context.GetSupplierCodeMatch(option.SupplierId), context.SaleZoneServiceList, target, option);
        }

        public override void ApplyOptionsPercentage(IEnumerable<RouteOption> options)
        {
            if (this.OptionPercentageSettings != null && options != null)
            {
                var activeOptions = options.FindAllRecords(itm => !itm.IsBlocked);
                if (activeOptions != null)
                    this.ApplyOptionsPercentage<RouteOption>(activeOptions);
            }
        }

        public override void ExecuteForSaleEntity(ISaleEntityRouteRuleExecutionContext context, RouteRuleTarget target)
        {
            throw new NotSupportedException("ExecuteForSaleEntity is not supported for RegularRouteRule.");
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
                                FilterOption(supplierCodeMatch, context.SaleZoneServiceIds, target, option);
                                if (!option.FilterOption)
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
                        FilterOption(supplierCodeMatch, context.SaleZoneServiceIds, target, option);
                        if (!option.FilterOption)
                            context.TryAddSupplierZoneOption(option);
                    }
                }
            }
        }

        public override void ApplyRuleToRPOptions(IRPRouteRuleExecutionContext context, ref IEnumerable<RPRouteOption> options)
        {
            options = ApplyOptionsOrder(options, context.RoutingDatabase);

            if (this.OptionPercentageSettings != null && options != null)
            {
                var blockedOptions = options.FindAllRecords(itm => itm.SupplierStatus == SupplierStatus.Block);
                if (blockedOptions != null)
                {
                    foreach (var blockedOption in blockedOptions)
                        blockedOption.Percentage = null;
                }

                var unblockedOptions = options.FindAllRecords(itm => itm.SupplierStatus != SupplierStatus.Block);
                if (unblockedOptions != null)
                    ApplyOptionsPercentage(unblockedOptions);
            }
        }

        #endregion

        #region Private Methods

        private List<RouteOptionRuleTarget> CreateOptions(ISaleEntityRouteRuleExecutionContext context, RouteRuleTarget target)
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
                var allSuppliersCodeMatches = context.GetFilteredSuppliersCodeMatches();
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

        private RouteOptionRuleTarget CreateOption(RouteRuleTarget routeRuleTarget, SupplierCodeMatchWithRate supplierCodeMatchWithRate, int? percentage)
        {
            var supplierCodeMatch = supplierCodeMatchWithRate.CodeMatch;
            var option = new RouteOptionRuleTarget
            {
                RouteTarget = routeRuleTarget,
                SupplierId = supplierCodeMatch.SupplierId,
                SupplierCode = supplierCodeMatch.SupplierCode,
                SupplierZoneId = supplierCodeMatch.SupplierZoneId,
                SupplierRate = supplierCodeMatchWithRate.RateValue,
                EffectiveOn = routeRuleTarget.EffectiveOn,
                IsEffectiveInFuture = routeRuleTarget.IsEffectiveInFuture,
                ExactSupplierServiceIds = supplierCodeMatchWithRate.ExactSupplierServiceIds,
                SupplierServiceIds = supplierCodeMatchWithRate.SupplierServiceIds,
                SupplierServiceWeight = supplierCodeMatchWithRate.SupplierServiceWeight,
                NumberOfTries = 1,
                SupplierRateId = supplierCodeMatchWithRate.SupplierRateId,
                SupplierRateEED = supplierCodeMatchWithRate.SupplierRateEED
            };
            if (percentage.HasValue)
                option.Percentage = percentage.Value;
            return option;
        }

        private List<T> ApplyOptionsOrder<T>(IEnumerable<T> options, RoutingDatabase routingDatabase) where T : IRouteOptionOrderTarget
        {
            if (this.OptionOrderSettings != null && OptionOrderSettings.Count > 0)
            {
                if (OptionOrderSettings.Count == 1)
                {
                    RouteOptionOrderSettings optionOrderSettings = OptionOrderSettings[0];
                    var optionOrderContext = ExecuteOptionOrderSettings<T>(options, routingDatabase, optionOrderSettings);
                    switch (optionOrderContext.OrderDitection)
                    {
                        case OrderDirection.Ascending: options = optionOrderContext.Options.OrderBy(itm => itm.OptionWeight).ThenBy(itm => itm.SupplierId).VRCast<T>(); break;
                        case OrderDirection.Descending: options = optionOrderContext.Options.OrderByDescending(itm => itm.OptionWeight).ThenBy(itm => itm.SupplierId).VRCast<T>(); break;
                    }
                }
                else
                {
                    switch (OrderType)
                    {
                        case Business.OrderType.Sequential: options = OrderOptionsBySequential<T>(options, routingDatabase); break;
                        case Business.OrderType.Percentage: options = OrderOptionsByPercentage<T>(options, routingDatabase); break;
                    }
                }
            }
            return options != null ? options.ToList() : null;
        }

        private IEnumerable<T> OrderOptionsBySequential<T>(IEnumerable<T> options, RoutingDatabase routingDatabase) where T : IRouteOptionOrderTarget
        {
            Dictionary<IRouteOptionOrderTarget, List<decimal>> orderValues = new Dictionary<IRouteOptionOrderTarget, List<decimal>>();

            int coefficient;

            foreach (RouteOptionOrderSettings optionOrderSettings in OptionOrderSettings)
            {
                var optionOrderContext = ExecuteOptionOrderSettings<T>(options, routingDatabase, optionOrderSettings);

                switch (optionOrderContext.OrderDitection)
                {
                    case OrderDirection.Ascending: coefficient = 1; break;
                    case OrderDirection.Descending: coefficient = -1; break;
                    default: throw new NotSupportedException("optionOrderContext.OrderDitection");
                }

                foreach (IRouteOptionOrderTarget option in optionOrderContext.Options)
                {
                    List<decimal> optionWeights = orderValues.GetOrCreateItem<IRouteOptionOrderTarget, List<decimal>>(option);
                    optionWeights.Add(option.OptionWeight * coefficient);
                }
            }

            if (orderValues.Count > 0)
            {
                IOrderedEnumerable<KeyValuePair<IRouteOptionOrderTarget, List<decimal>>> orderedData = orderValues.OrderBy(itm => itm.Value[0]);
                if (OptionOrderSettings.Count > 1)
                {
                    for (var x = 1; x < OptionOrderSettings.Count; x++)
                    {
                        var index = x;
                        orderedData = orderedData.ThenBy(itm => itm.Value[index]);
                    }
                }
                orderedData = orderedData.ThenBy(itm => itm.Key.SupplierId);

                List<IRouteOptionOrderTarget> list = new List<IRouteOptionOrderTarget>();

                foreach (KeyValuePair<IRouteOptionOrderTarget, List<decimal>> item in orderedData)
                {
                    list.Add(item.Key);
                }
                options = list.VRCast<T>();
            }
            return options;
        }

        private IEnumerable<T> OrderOptionsByPercentage<T>(IEnumerable<T> options, RoutingDatabase routingDatabase) where T : IRouteOptionOrderTarget
        {
            Dictionary<int, WeightResult> orderValues = new Dictionary<int, WeightResult>();
            Func<IRouteOptionOrderTarget, bool> predicate = (itm) => itm.OptionWeight != 0;

            foreach (RouteOptionOrderSettings optionOrderSettings in OptionOrderSettings)
            {
                var optionOrderContext = ExecuteOptionOrderSettings<T>(options, routingDatabase, optionOrderSettings);

                IEnumerable<IRouteOptionOrderTarget> data = optionOrderContext.Options.FindAllRecords<IRouteOptionOrderTarget>(predicate);
                if (data == null || data.Count() == 0)
                {
                    continue;
                }
                decimal maxWeightAbs = data.Max(itm => Math.Abs(itm.OptionWeight));

                foreach (IRouteOptionOrderTarget option in optionOrderContext.Options)
                {
                    WeightResult result;
                    var weightNormalized = (option.OptionWeight / maxWeightAbs) * (optionOrderSettings.PercentageValue.Value / 100);
                    if (!orderValues.TryGetValue(option.SupplierId, out result))
                    {
                        result = new WeightResult();
                        orderValues.Add(option.SupplierId, result);
                    }
                    switch (optionOrderContext.OrderDitection)
                    {
                        case OrderDirection.Ascending: result.Result += weightNormalized; break;
                        case OrderDirection.Descending: result.Result -= weightNormalized; break;
                    }
                }
            }
            if (orderValues.Count > 0)
            {
                List<T> list = new List<T>();
                var resultOrdered = orderValues.OrderBy(itm => itm.Value.Result).ThenBy(itm => itm.Key);
                foreach (KeyValuePair<int, WeightResult> item in resultOrdered)
                {
                    list.Add(options.First(itm => itm.SupplierId == item.Key));
                }
                options = list;
            }
            return options;
        }

        private RouteOptionOrderExecutionContext ExecuteOptionOrderSettings<T>(IEnumerable<T> options, RoutingDatabase routingDatabase, RouteOptionOrderSettings optionOrderSettings)
            where T : IRouteOptionOrderTarget
        {
            var optionOrderContext = new RouteOptionOrderExecutionContext
            {
                Options = options.VRCast<IRouteOptionOrderTarget>(),
                RoutingDatabase = routingDatabase
            };
            optionOrderSettings.Execute(optionOrderContext);
            return optionOrderContext;
        }

        private void FilterOption(SupplierCodeMatchWithRate supplierCodeMatchWithRate, HashSet<int> customerServiceIds, RouteRuleTarget target, BaseRouteOptionRuleTarget option)
        {
            if (this.OptionFilters != null)
            {
                foreach (var optionFilter in this.OptionFilters)
                {
                    var routeOptionFilterExecutionContext = new RouteOptionFilterExecutionContext()
                    {
                        Option = option,
                        SaleRate = target.SaleRate,
                        CustomerServices = customerServiceIds,
                        SupplierServices = supplierCodeMatchWithRate != null ? supplierCodeMatchWithRate.SupplierServiceIds : null,
                        SupplierId = option.SupplierId
                    };
                    optionFilter.Execute(routeOptionFilterExecutionContext);
                    if (routeOptionFilterExecutionContext.FilterOption)
                    {
                        option.FilterOption = true;
                        return;
                    }
                }
            }

            if (this.OptionsSettingsGroup != null)
            {
                var routeOptionFilterExecutionContext = new RouteOptionFilterExecutionContext()
                    {
                        Option = option,
                        SaleRate = target.SaleRate,
                        CustomerServices = customerServiceIds,
                        SupplierServices = supplierCodeMatchWithRate != null ? supplierCodeMatchWithRate.SupplierServiceIds : null,
                        SupplierId = option.SupplierId
                    };

                if (this.OptionsSettingsGroup.IsOptionFiltered(routeOptionFilterExecutionContext))
                {
                    option.FilterOption = true;
                    return;
                }
            }
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

        #region Private Classes

        private class WeightResult
        {
            public WeightResult()
            {
                Result = 0;
            }
            public decimal Result { get; set; }
        }

        #endregion
    }
}