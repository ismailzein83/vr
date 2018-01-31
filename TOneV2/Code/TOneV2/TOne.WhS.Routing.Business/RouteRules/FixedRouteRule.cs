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

        public Dictionary<int, FixedRouteOptionSettings> Options { get; set; }

        #endregion

        #region SaleEntity Execution

        public override RouteRuleSettings BuildLinkedRouteRuleSettings(ILinkedRouteRuleContext context)
        {
            FixedRouteRule fixedRouteRule = new FixedRouteRule();

            if (context.RouteOptions != null && context.RouteOptions.Count > 0)
            {
                fixedRouteRule.Options = new Dictionary<int, FixedRouteOptionSettings>();

                foreach (RouteOption routeOption in context.RouteOptions)
                {
                    FixedRouteOptionSettings optionSettings = new FixedRouteOptionSettings() { Percentage = routeOption.Percentage, SupplierId = routeOption.SupplierId };
                    fixedRouteRule.Options.Add(routeOption.SupplierId, optionSettings);

                    FixedRouteOptionSettings matchedFixedRouteOptionSettings;
                    if (Options != null && Options.TryGetValue(routeOption.SupplierId, out matchedFixedRouteOptionSettings) && matchedFixedRouteOptionSettings.Filters != null)
                    {
                        optionSettings.Filters = Vanrise.Common.Utilities.CloneObject<List<RouteOptionFilterSettings>>(matchedFixedRouteOptionSettings.Filters);
                    }
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
                SupplierFilterSettings supplierFilterSettings = new SupplierFilterSettings { RoutingProductId = target.RoutingProductId };

                List<FixedRouteOptionSettings> fixedOptions = null;
                HashSet<int> filteredSupplierIds = SupplierGroupContext.GetFilteredSupplierIds(supplierFilterSettings);
                if (filteredSupplierIds != null)
                {
                    var filteredSuppliers = this.Options.FindAllRecords(itm => filteredSupplierIds.Contains(itm.Key)); //fixedOptions.Where(option => filteredSupplierIds.Contains(option.SupplierId));
                    if (filteredSuppliers != null && filteredSuppliers.Count() > 0)
                        fixedOptions = filteredSuppliers.Select(itm => itm.Value).ToList();
                }
                else
                {
                    fixedOptions = this.Options.Values != null ? this.Options.Values.ToList() : null;
                }

                if (fixedOptions != null)
                {
                    foreach (var optionSettings in fixedOptions)
                    {
                        List<SupplierCodeMatchWithRate> optionSupplierCodeMatches = context.GetSupplierCodeMatches(optionSettings.SupplierId);
                        if (optionSupplierCodeMatches != null)
                        {
                            foreach (var supplierCodeMatch in optionSupplierCodeMatches)
                            {
                                var option = CreateOption(target, supplierCodeMatch, optionSettings.Percentage);
                                if (!FilterOption(supplierCodeMatch, context.SaleZoneServiceIds, target, option))
                                    context.TryAddSupplierZoneOption(option);
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


                List<FixedRouteOptionSettings> fixedOptions = null;
                HashSet<int> filteredSupplierIds = SupplierGroupContext.GetFilteredSupplierIds(supplierFilterSettings);
                if (filteredSupplierIds != null)
                {
                    var filteredSuppliers = this.Options.FindAllRecords(itm => filteredSupplierIds.Contains(itm.Key)); //fixedOptions.Where(option => filteredSupplierIds.Contains(option.SupplierId));
                    if (filteredSuppliers != null && filteredSuppliers.Count() > 0)
                        fixedOptions = filteredSuppliers.Select(itm => itm.Value).ToList();
                }
                else
                {
                    fixedOptions = this.Options.Values != null ? this.Options.Values.ToList() : null;
                }

                if (fixedOptions != null)
                {
                    foreach (var optionSettings in fixedOptions)
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
                SupplierServiceWeight = supplierCodeMatchWithRate.SupplierServiceWeight,
                NumberOfTries = 1,
                SupplierRateId = supplierCodeMatchWithRate.SupplierRateId,
                SupplierRateEED = supplierCodeMatchWithRate.SupplierRateEED
            };
            if (percentage.HasValue)
                option.Percentage = percentage.Value;
            return option;
        }

        private bool FilterOption(SupplierCodeMatchWithRate supplierCodeMatchWithRate, HashSet<int> customerServiceIds, RouteRuleTarget target, RouteOptionRuleTarget option)
        {
            if (Options != null)
            {
                var fixedOption = Options.GetRecord(supplierCodeMatchWithRate.CodeMatch.SupplierId);
                if (fixedOption != null && fixedOption.Filters != null)
                {
                    List<RouteOptionFilterSettings> optionFilters = fixedOption.Filters;
                    if (optionFilters != null)
                    {
                        foreach (var optionFilter in optionFilters)
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
                }
            }

            return false;
        }

        #endregion
    }
}