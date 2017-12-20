using System;
using System.Collections.Generic;
using System.Linq;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Routing.Entities;
using Vanrise.Common;

namespace TOne.WhS.Routing.Business
{
    public class FixedRouteRule : RouteRuleSettings
    {
        #region Properties/Ctor

        public override Guid ConfigId { get { return new Guid("12E2CAD6-ABD9-4D2B-B2F3-51C3DF501DE9"); } }

        public override CorrespondentType CorrespondentType { get { return Entities.CorrespondentType.Override; } }

        public override bool UseOrderedExecution { get { return true; } }

        public List<FixedRuleOptionSettings> Options { get; set; }

        Dictionary<int, List<FixedRuleOptionSettings>> _optionsBySupplierId;
        Dictionary<int, List<FixedRuleOptionSettings>> OptionsBySupplierId
        {
            get
            {
                if (_optionsBySupplierId == null)
                {
                    if (Options != null)
                    {
                        _optionsBySupplierId = new Dictionary<int, List<FixedRuleOptionSettings>>();

                        foreach (FixedRuleOptionSettings option in Options)
                        {
                            List<FixedRuleOptionSettings> optionList = _optionsBySupplierId.GetOrCreateItem(option.SupplierId);
                            optionList.Add(option);
                        }
                    }
                }
                return _optionsBySupplierId;
            }
        }

        #endregion

        #region SaleEntity Execution

        public override RouteRuleSettings BuildLinkedRouteRuleSettings(ILinkedRouteRuleContext context)
        {
            FixedRouteRule fixedRouteRule = new FixedRouteRule();

            if (context.RouteOptions != null && context.RouteOptions.Count > 0)
            {
                Dictionary<int, List<FixedRuleOptionSettings>> clonedOptionsBySupplierId = null;
                if (OptionsBySupplierId != null)
                    clonedOptionsBySupplierId = Vanrise.Common.Utilities.CloneObject<Dictionary<int, List<FixedRuleOptionSettings>>>(OptionsBySupplierId);

                fixedRouteRule.Options = new List<FixedRuleOptionSettings>();

                foreach (RouteOption routeOption in context.RouteOptions)
                {
                    FixedRuleOptionSettings optionSettings = new FixedRuleOptionSettings() { Percentage = routeOption.Percentage, SupplierId = routeOption.SupplierId };

                    List<FixedRuleOptionSettings> relatedOptions;
                    if (clonedOptionsBySupplierId != null && clonedOptionsBySupplierId.TryGetValue(routeOption.SupplierId, out relatedOptions) && relatedOptions.Count > 0)
                    {
                        FixedRuleOptionSettings relatedOption = relatedOptions.First();
                        relatedOptions.Remove(relatedOption);
                        optionSettings.Filters = relatedOption.Filters != null ? Vanrise.Common.Utilities.CloneObject<List<RouteOptionFilterSettings>>(relatedOption.Filters) : null;
                    }
                    fixedRouteRule.Options.Add(optionSettings);
                }
            }

            return fixedRouteRule;
        }

        public override int? GetMaxNumberOfOptions(ISaleEntityRouteRuleExecutionContext context)
        {
            return null;
        }

        public override List<RouteOptionRuleTarget> GetOrderedOptions(ISaleEntityRouteRuleExecutionContext context, RouteRuleTarget target)
        {
            return CreateOptions(context, target);
        }

        public override bool IsOptionFiltered(ISaleEntityRouteRuleExecutionContext context, RouteRuleTarget target, RouteOptionRuleTarget option)
        {
            return FilterOption(context.GetSupplierCodeMatch(option.SupplierId), context.SaleZoneServiceList, target, option);
        }

        public override void ApplyOptionsPercentage(IEnumerable<RouteOption> options)
        {
            if (options == null)
                return;

            int? totalAssignedPercentage = null;

            var unblockedOptions = options.FindAllRecords(itm => !itm.IsBlocked && !itm.IsFiltered);
            if (unblockedOptions != null)
            {
                var unblockedOptionsWithPercentage = unblockedOptions.FindAllRecords(itm => itm.Percentage.HasValue);
                if (unblockedOptionsWithPercentage != null && unblockedOptionsWithPercentage.Count() > 0)
                    totalAssignedPercentage = unblockedOptionsWithPercentage.Sum(itm => itm.Percentage.Value);
            }

            if (!totalAssignedPercentage.HasValue || totalAssignedPercentage == 100 || totalAssignedPercentage == 0)
                return;

            int unassignedPercentages = 100 - totalAssignedPercentage.Value;
            int newTotalAssignedPercentage = 0;
            RouteOption routeOptionWithHighestPercentage = null;

            foreach (var option in options)
            {
                if (!option.Percentage.HasValue)
                    continue;

                if (option.IsBlocked || option.IsFiltered)
                {
                    option.Percentage = 0;
                    continue;
                }

                option.Percentage = option.Percentage.Value + option.Percentage.Value * unassignedPercentages / totalAssignedPercentage.Value;
                newTotalAssignedPercentage += option.Percentage.Value;

                if (routeOptionWithHighestPercentage == null || routeOptionWithHighestPercentage.Percentage < option.Percentage)
                    routeOptionWithHighestPercentage = option;
            }

            if (newTotalAssignedPercentage != 100)
                routeOptionWithHighestPercentage.Percentage = routeOptionWithHighestPercentage.Percentage.Value + (100 - newTotalAssignedPercentage);
        }

        public override void ExecuteForSaleEntity(ISaleEntityRouteRuleExecutionContext context, RouteRuleTarget target)
        {
            throw new NotSupportedException("ExecuteForSaleEntity is not supported for FixedRouteRule.");
        }

        #endregion

        #region RoutingProduct Execution

        public override void CreateSupplierZoneOptionsForRP(IRPRouteRuleExecutionContext context, RouteRuleTarget target)
        {
            if (this.Options != null)
            {
                SupplierFilterSettings supplierFilterSettings = new SupplierFilterSettings
                {
                    RoutingProductId = target.RoutingProductId
                };

                HashSet<int> filteredSupplierIds = SupplierGroupContext.GetFilteredSupplierIds(supplierFilterSettings);
                if (OptionsBySupplierId != null)
                {
                    foreach (var options in OptionsBySupplierId)
                    {
                        int supplierId = options.Key;
                        List<FixedRuleOptionSettings> fixedRuleOptionSettingsList = options.Value;

                        if (filteredSupplierIds == null || filteredSupplierIds.Count == 0 || filteredSupplierIds.Contains(supplierId))
                        {
                            List<SupplierCodeMatchWithRate> optionSupplierCodeMatches = context.GetSupplierCodeMatches(supplierId);
                            if (optionSupplierCodeMatches != null)
                            {
                                int? percentage = null;
                                if (fixedRuleOptionSettingsList != null)
                                {
                                    foreach (FixedRuleOptionSettings fixedRuleOptionSettings in fixedRuleOptionSettingsList)
                                    {
                                        if (fixedRuleOptionSettings.Percentage.HasValue)
                                        {
                                            if (!percentage.HasValue)
                                                percentage = fixedRuleOptionSettings.Percentage;
                                            else
                                                percentage += fixedRuleOptionSettings.Percentage;
                                        }
                                    }
                                }

                                FixedRuleOptionSettings optionSettings = new FixedRuleOptionSettings() { SupplierId = supplierId, Percentage = percentage };
                                foreach (var supplierCodeMatch in optionSupplierCodeMatches)
                                {
                                    var option = CreateOption(target, supplierCodeMatch, optionSettings);
                                    if (!FilterOption(supplierCodeMatch, context.SaleZoneServiceIds, target, option))
                                        context.TryAddSupplierZoneOption(option);
                                }
                            }
                        }
                    }
                }
            }
        }

        public override void ApplyRuleToRPOptions(IRPRouteRuleExecutionContext context, ref IEnumerable<RPRouteOption> options)
        {
            if (options == null)
                return;

            int? totalAssignedPercentage = null;

            var blockedOptions = options.FindAllRecords(itm => itm.SupplierStatus == SupplierStatus.Block);
            if (blockedOptions != null)
            {
                foreach (var blockedOption in blockedOptions)
                    blockedOption.Percentage = null;
            }

            var unblockedOptions = options.FindAllRecords(itm => itm.SupplierStatus != SupplierStatus.Block);
            if (unblockedOptions != null)
            {
                var unblockedOptionsWithPercentage = unblockedOptions.FindAllRecords(itm => itm.Percentage.HasValue);
                if (unblockedOptionsWithPercentage != null && unblockedOptionsWithPercentage.Count() > 0)
                    totalAssignedPercentage = unblockedOptionsWithPercentage.Sum(itm => itm.Percentage.Value);
            }

            if (!totalAssignedPercentage.HasValue || totalAssignedPercentage == 100 || totalAssignedPercentage == 0)
                return;

            int unassignedPercentages = 100 - totalAssignedPercentage.Value;

            int newTotalAssignedPercentage = 0;
            RPRouteOption rpRouteOptionWithHighestPercentage = null;

            foreach (var option in options)
            {
                if (!option.Percentage.HasValue)
                    continue;

                if (option.SupplierStatus == SupplierStatus.Block)
                {
                    option.Percentage = 0;
                    continue;
                }

                option.Percentage = option.Percentage.Value + option.Percentage.Value * unassignedPercentages / totalAssignedPercentage.Value;
                newTotalAssignedPercentage += option.Percentage.Value;

                if (rpRouteOptionWithHighestPercentage == null || rpRouteOptionWithHighestPercentage.Percentage < option.Percentage)
                    rpRouteOptionWithHighestPercentage = option;
            }

            if (newTotalAssignedPercentage != 100)
                rpRouteOptionWithHighestPercentage.Percentage = rpRouteOptionWithHighestPercentage.Percentage.Value + (100 - newTotalAssignedPercentage);
        }

        #endregion

        #region Private Methods

        private List<RouteOptionRuleTarget> CreateOptions(ISaleEntityRouteRuleExecutionContext context, RouteRuleTarget target)
        {
            var options = new List<RouteOptionRuleTarget>();
            if (this.Options != null)
            {
                SupplierFilterSettings supplierFilterSettings = new SupplierFilterSettings
                {
                    RoutingProductId = target.RoutingProductId
                };

                var fixedOptions = this.Options;
                HashSet<int> filteredSupplierIds = SupplierGroupContext.GetFilteredSupplierIds(supplierFilterSettings);
                if (filteredSupplierIds != null)
                {
                    var filteredSuppliers = fixedOptions.Where(option => filteredSupplierIds.Contains(option.SupplierId));
                    if (filteredSuppliers != null && filteredSuppliers.Count() > 0)
                        fixedOptions = filteredSuppliers.ToList();
                }

                foreach (var optionSettings in fixedOptions)
                {
                    SupplierCodeMatchWithRate optionSupplierCodeMatch = context.GetSupplierCodeMatch(optionSettings.SupplierId);
                    if (optionSupplierCodeMatch != null)
                    {
                        var option = CreateOption(target, optionSupplierCodeMatch, optionSettings);
                        options.Add(option);
                    }
                }
            }

            return options;
        }

        private FixedRouteOptionRuleTarget CreateOption(RouteRuleTarget routeRuleTarget, SupplierCodeMatchWithRate supplierCodeMatchWithRate, FixedRuleOptionSettings fixedOption)
        {
            var supplierCodeMatch = supplierCodeMatchWithRate.CodeMatch;
            var option = new FixedRouteOptionRuleTarget
            {
                RouteTarget = routeRuleTarget,
                SupplierId = supplierCodeMatch.SupplierId,
                SupplierCode = supplierCodeMatch.SupplierCode,
                SupplierZoneId = supplierCodeMatch.SupplierZoneId,
                SupplierRate = supplierCodeMatchWithRate.RateValue,
                EffectiveOn = routeRuleTarget.EffectiveOn,
                IsEffectiveInFuture = routeRuleTarget.IsEffectiveInFuture,
                ExactSupplierServiceIds = supplierCodeMatchWithRate.ExactSupplierServiceIds,
                SupplierServiceWeight = supplierCodeMatchWithRate.SupplierServiceWeight,
                NumberOfTries = 1,
                SupplierRateId = supplierCodeMatchWithRate.SupplierRateId,
                SupplierRateEED = supplierCodeMatchWithRate.SupplierRateEED,
                Filters = fixedOption.Filters
            };

            if (fixedOption != null && fixedOption.Percentage.HasValue)
                option.Percentage = fixedOption.Percentage.Value;

            return option;
        }

        private bool FilterOption(SupplierCodeMatchWithRate supplierCodeMatchWithRate, HashSet<int> customerServiceIds, RouteRuleTarget target, RouteOptionRuleTarget option)
        {
            FixedRouteOptionRuleTarget fixedRouteOptionRuleTarget = option.CastWithValidate<FixedRouteOptionRuleTarget>("option");
            if (fixedRouteOptionRuleTarget.Filters != null)
            {
                foreach (var optionFilter in fixedRouteOptionRuleTarget.Filters)
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
                        return true;
                }
            }

            return false;
        }

        #endregion

        private class FixedRouteOptionRuleTarget : RouteOptionRuleTarget
        {
            public List<RouteOptionFilterSettings> Filters { get; set; }
        }
    }
}