using System;
using System.Collections.Generic;
using System.Linq;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Routing.Business.RouteRules.Filters;
using TOne.WhS.Routing.Business.RouteRules.Orders;
using TOne.WhS.Routing.Entities;
using Vanrise.Common;

namespace TOne.WhS.Routing.Business
{
    public partial class RegularRouteRule : RouteRuleSettings
    {
        #region Central Execution

        public override bool UseOrderedExecution
        {
            get
            {
                return true;
            }
        }
        private CorrespondentType? _correspondentType;

        public override CorrespondentType CorrespondentType
        {
            get
            {
                if (!_correspondentType.HasValue)
                {
                    if (AreOptionsExist())
                        _correspondentType = CorrespondentType.Override;

                    else if (!AreOptionsExist() && AreOptionsOnlyOrderByType<OptionOrderByRate>() && AreOptionsFilteredByTypes<RateOptionFilter, ServiceOptionFilter>())
                        _correspondentType = CorrespondentType.LCR;

                    else _correspondentType = CorrespondentType.Other;
                }
                return _correspondentType.Value;
            }
        }

        private bool AreOptionsExist()
        {
            if (OptionsSettingsGroup != null)
                return true;
            return false;
        }

        private bool AreOptionsOnlyOrderByType<T>()
        {
            if (OptionOrderSettings != null && OptionOrderSettings.Count == 1 && OptionOrderSettings.First() is T)
                return true;
            return false;
        }

        private bool AreOptionsFilteredByTypes<T, Q>()
        {
            bool isFilteredByT = false;
            bool isFilteredByQ = false;
            if (OptionFilters != null && OptionFilters.Count >= 2)
            {
                foreach (RouteOptionFilterSettings item in OptionFilters)
                {
                    if (item is T)
                        isFilteredByT = true;
                    if (item is Q)
                        isFilteredByQ = true;
                    if (isFilteredByT && isFilteredByQ)
                        return true;
                }
            }
            return false;
        }

        public override List<RouteOptionRuleTarget> GetOrderedOptions(ISaleEntityRouteRuleExecutionContext context, RouteRuleTarget target)
        {
            var options = CreateOptions(context, target);
            if (options != null)
                return ApplyOptionsOrder(options);
            else
                return null;
        }

        public override bool IsOptionFiltered(ISaleEntityRouteRuleExecutionContext context, TOne.WhS.Routing.Entities.RouteRuleTarget target, TOne.WhS.Routing.Entities.RouteOptionRuleTarget option)
        {
            return FilterOption(context.GetSupplierCodeMatch(option.SupplierId), context.CustomerServiceIdHashSet, target, option);
        }

        public override void ApplyOptionsPercentage(IEnumerable<RouteOption> options)
        {
            this.ApplyOptionsPercentage<RouteOption>(options);
        }

        #endregion

        #region SaleEntity Execution

        //public const int ExtensionConfigId = 11;

        public override void ExecuteForSaleEntity(ISaleEntityRouteRuleExecutionContext context, RouteRuleTarget target)
        {
            var options = CreateOptions(context, target);
            if (options != null)
            {
                options = ApplyOptionsOrder(options);
                int optionsAdded = 0;
                foreach (RouteOptionRuleTarget option in options)
                {
                    if (!FilterOption(context.GetSupplierCodeMatch(option.SupplierId), context.CustomerServiceIdHashSet, target, option))
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
                        if (optionSettings.SupplierId == target.CustomerId)
                            continue;

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
                        if (supplierCodeMatch.CodeMatch.SupplierId == target.CustomerId)
                            continue;
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
                                if (!FilterOption(supplierCodeMatch, null, target, option))
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
                        if (!FilterOption(supplierCodeMatch, null, target, option))
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
                EffectiveOn = routeRuleTarget.EffectiveOn,
                IsEffectiveInFuture = routeRuleTarget.IsEffectiveInFuture,
                ExactSupplierServiceIds = supplierCodeMatchWithRate.SupplierServiceIds
            };
            if (percentage.HasValue)
                option.Percentage = percentage.Value;
            return option;
        }

        private List<T> ApplyOptionsOrder<T>(IEnumerable<T> options) where T : IRouteOptionOrderTarget
        {
            if (this.OptionOrderSettings != null && OptionOrderSettings.Count > 0)
            {
                if (OptionOrderSettings.Count == 1)
                {
                    RouteOptionOrderSettings optionOrderSettings = OptionOrderSettings[0];
                    var optionOrderContext = ExecuteOptionOrderSettings<T>(options, optionOrderSettings);
                    switch (optionOrderContext.OrderDitection)
                    {
                        case OrderDirection.Ascending: options = optionOrderContext.Options.OrderBy(itm => itm.OptionWeight).VRCast<T>(); break;
                        case OrderDirection.Descending: options = optionOrderContext.Options.OrderByDescending(itm => itm.OptionWeight).VRCast<T>(); break;
                    }
                }
                else
                {
                    Dictionary<int, WeightResult> orderValues = new Dictionary<int, WeightResult>();
                    Func<IRouteOptionOrderTarget, bool> predicate = (itm) => itm.OptionWeight != 0;

                    foreach (RouteOptionOrderSettings optionOrderSettings in OptionOrderSettings)
                    {
                        var optionOrderContext = ExecuteOptionOrderSettings<T>(options, optionOrderSettings);

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
                        var resultOrdered = orderValues.OrderBy(itm => itm.Value.Result);
                        foreach (KeyValuePair<int, WeightResult> item in resultOrdered)
                        {
                            list.Add(options.First(itm => itm.SupplierId == item.Key));
                        }
                        options = list;
                    }
                }
            }
            return options != null ? options.ToList() : null;
        }

        private RouteOptionOrderExecutionContext ExecuteOptionOrderSettings<T>(IEnumerable<T> options, RouteOptionOrderSettings optionOrderSettings) where T : IRouteOptionOrderTarget
        {
            var optionOrderContext = new RouteOptionOrderExecutionContext
            {
                Options = options.VRCast<IRouteOptionOrderTarget>(),
            };
            optionOrderSettings.Execute(optionOrderContext);
            return optionOrderContext;
        }

        private bool FilterOption(SupplierCodeMatchWithRate supplierCodeMatchWithRate, HashSet<int> customerServiceIds, RouteRuleTarget target, RouteOptionRuleTarget option)
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
                        SupplierServices = supplierCodeMatchWithRate != null ? supplierCodeMatchWithRate.SupplierServiceIds : null
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

        private class WeightResult
        {
            public WeightResult()
            {
                Result = 0;
            }
            public decimal Result { get; set; }
        }
    }
}
