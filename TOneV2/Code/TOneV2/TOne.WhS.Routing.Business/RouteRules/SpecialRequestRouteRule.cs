using System;
using System.Collections.Generic;
using System.Linq;
using TOne.WhS.Routing.Entities;
using Vanrise.Common;

namespace TOne.WhS.Routing.Business
{
    public class SpecialRequestRouteRule : RouteRuleSettings
    {
        #region Properties/Ctor

        public override Guid ConfigId { get { return new Guid("0166E5C4-0F13-4741-BD77-2C771BCAFA24"); } }

        public override CorrespondentType CorrespondentType { get { return Entities.CorrespondentType.SpecialRequest; } }

        public override bool UseOrderedExecution { get { return true; } }

        public Dictionary<int, SpecialRequestRouteOptionSettings> Options { get; set; }

        #endregion

        #region SaleEntity Execution

        public override RouteRuleSettings BuildLinkedRouteRuleSettings(ILinkedRouteRuleContext context)
        {
            SpecialRequestRouteRule specialRequestRouteRule = new SpecialRequestRouteRule();
            if (context.RouteOptions != null && context.RouteOptions.Count > 0)
            {
                specialRequestRouteRule.Options = new Dictionary<int, SpecialRequestRouteOptionSettings>();
                int counter = 0;
                foreach (RouteOption routeOption in context.RouteOptions)
                {
                    counter++;
                    SpecialRequestRouteOptionSettings optionSettings;
                    SpecialRequestRouteOptionSettings relatedOption;
                    if (Options != null && Options.TryGetValue(routeOption.SupplierId, out relatedOption))
                    {
                        optionSettings = new SpecialRequestRouteOptionSettings()
                        {
                            ForceOption = relatedOption.ForceOption,
                            NumberOfTries = relatedOption.NumberOfTries,
                            Percentage = routeOption.Percentage,
                            Position = counter,
                            SupplierId = routeOption.SupplierId
                        };
                    }
                    else
                    {
                        optionSettings = new SpecialRequestRouteOptionSettings()
                        {
                            ForceOption = false,
                            NumberOfTries = 1,
                            Percentage = routeOption.Percentage,
                            Position = counter,
                            SupplierId = routeOption.SupplierId
                        };
                    }
                    specialRequestRouteRule.Options.Add(routeOption.SupplierId, optionSettings);
                }
            }
            return specialRequestRouteRule;
        }

        public override int? GetMaxNumberOfOptions(ISaleEntityRouteRuleExecutionContext context)
        {
            int? numberOfOptions = base.GetMaxNumberOfOptions(context);
            if (!numberOfOptions.HasValue)
                throw new NullReferenceException("numberOfOptions must have a value for Speical Request Route Rule");

            return Options != null ? Math.Max(numberOfOptions.Value, Options.Count) : numberOfOptions.Value;
        }

        public override List<RouteOptionRuleTarget> GetOrderedOptions(ISaleEntityRouteRuleExecutionContext context, RouteRuleTarget target)
        {
            var options = CreateOptions(context, target);
            if (options != null)
                return ApplyOptionsOrder(options, context.NumberOfOptions);
            else
                return null;
        }

        public override bool IsOptionFiltered(ISaleEntityRouteRuleExecutionContext context, TOne.WhS.Routing.Entities.RouteRuleTarget target, TOne.WhS.Routing.Entities.RouteOptionRuleTarget option)
        {
            return FilterOption(context.GetSupplierCodeMatch(option.SupplierId), context.SaleZoneServiceList, target, option);
        }

        public override void ExecuteForSaleEntity(ISaleEntityRouteRuleExecutionContext context, RouteRuleTarget target)
        {
            throw new NotSupportedException("ExecuteForSaleEntity is not supported for SpecialRequestRouteRule.");
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

        public override List<RouteOption> GetFinalOptions(IFinalizeRouteOptionContext context)
        {
            if (context.RouteOptions == null)
                return null;

            if (Options == null)
                return context.RouteOptions;

            int blockedOptionsCount = context.RouteOptions.Count(itm => itm.IsBlocked || itm.IsFiltered);

            int totalCount = blockedOptionsCount + context.NumberOfOptionsInSettings;
            if (context.RouteOptions.Count <= totalCount)
                return context.RouteOptions;

            int routeOptionsCount = context.RouteOptions.Count;
            for (int i = routeOptionsCount - 1; i >= totalCount; i--)
            {
                var currentRouteOption = context.RouteOptions[i];
                if (!Options.ContainsKey(currentRouteOption.SupplierId))
                    context.RouteOptions.Remove(currentRouteOption);
                else
                    break;
            }
            return context.RouteOptions;
        }

        private int AddOption(List<RouteOption> options, RouteOption optionToAdd, int currentIndex, HashSet<int> addedSupplierIds, bool incrementIndex)
        {
            int newIndex = incrementIndex ? currentIndex++ : currentIndex;
            options.Add(optionToAdd);
            addedSupplierIds.Add(optionToAdd.SupplierId);
            return newIndex;
        }

        #endregion

        #region RoutingProduct Execution

        public override void CreateSupplierZoneOptionsForRP(IRPRouteRuleExecutionContext context, RouteRuleTarget target)
        {
            var allSuppliersCodeMatches = context.GetAllSuppliersCodeMatches();
            if (allSuppliersCodeMatches != null)
            {
                foreach (var supplierCodeMatch in allSuppliersCodeMatches)
                {
                    var option = CreateOption(target, supplierCodeMatch);
                    if (!FilterOption(supplierCodeMatch, context.SaleZoneServiceIds, target, option))
                        context.TryAddSupplierZoneOption(option);
                }
            }
        }

        public override void ApplyRuleToRPOptions(IRPRouteRuleExecutionContext context, ref IEnumerable<RPRouteOption> options)
        {
            if (options != null)
            {
                options = ApplyOptionsOrder(options, null);

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
        }

        #endregion

        #region Private Methods

        private List<RouteOptionRuleTarget> CreateOptions(ISaleEntityRouteRuleExecutionContext context, RouteRuleTarget target)
        {
            var options = new List<RouteOptionRuleTarget>();

            var allSuppliersCodeMatches = context.GetAllSuppliersCodeMatches();
            if (allSuppliersCodeMatches != null)
            {
                foreach (var supplierCodeMatch in allSuppliersCodeMatches)
                {
                    var option = CreateOption(target, supplierCodeMatch);
                    options.Add(option);
                }
            }

            return options;
        }
        private RouteOptionRuleTarget CreateOption(RouteRuleTarget routeRuleTarget, SupplierCodeMatchWithRate supplierCodeMatchWithRate)
        {
            var supplierCodeMatch = supplierCodeMatchWithRate.CodeMatch;
            SpecialRequestRouteOptionSettings supplierSettings;
            int? numberOfTries = null;
            int? percentage = null;

            if (Options != null && Options.TryGetValue(supplierCodeMatch.SupplierId, out supplierSettings))
            {
                numberOfTries = supplierSettings.NumberOfTries;
                percentage = supplierSettings.Percentage;
            }

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
                Percentage = percentage,
                NumberOfTries = numberOfTries.HasValue ? numberOfTries.Value : 1,
                SupplierRateId = supplierCodeMatchWithRate.SupplierRateId,
                SupplierRateEED = supplierCodeMatchWithRate.SupplierRateEED
            };

            return option;
        }

        private bool FilterOption(SupplierCodeMatchWithRate supplierCodeMatchWithRate, HashSet<int> customerServiceIds, RouteRuleTarget target, RouteOptionRuleTarget option)
        {
            bool checkFilters = true;
            SpecialRequestRouteOptionSettings supplierSettings;

            if (Options != null && Options.TryGetValue(option.SupplierId, out supplierSettings))
            {
                checkFilters = !supplierSettings.ForceOption;
            }

            if (checkFilters)
            {
                HashSet<int> supplierServices = supplierCodeMatchWithRate != null ? supplierCodeMatchWithRate.SupplierServiceIds : null;

                if (ExecuteRateOptionFilter(target.SaleRate, option.SupplierRate))
                    return true;

                if (ExecuteServiceOptionFilter(customerServiceIds, supplierServices))
                    return true;
            }
            return false;
        }
        private bool ExecuteServiceOptionFilter(HashSet<int> customerServices, HashSet<int> supplierServices)
        {
            if (customerServices == null)
                return false;

            foreach (int itm in customerServices)
            {
                if (supplierServices == null || !supplierServices.Contains(itm))
                    return true;
            }

            return false;
        }
        private bool ExecuteRateOptionFilter(Decimal? saleRate, Decimal supplierRate)
        {
            if (!saleRate.HasValue)
                return false;

            return (saleRate.Value - supplierRate) < 0;
        }

        private List<T> ApplyOptionsOrder<T>(IEnumerable<T> options, int? numberOfOptions) where T : IRouteOptionOrderTarget
        {
            if (options == null)
                return null;

            IEnumerable<T> finalOptions = new List<T>(options);

            if (Options != null)
                finalOptions = finalOptions.FindAllRecords(itm => !Options.ContainsKey(itm.SupplierId));

            finalOptions = finalOptions.OrderBy(itm => itm.SupplierRate).ThenByDescending(itm => itm.SupplierServiceWeight).ThenBy(itm => itm.SupplierId);

            List<T> orderedOptions = finalOptions.ToList();

            if (Options != null)
            {
                List<SpecialRequestRouteOptionSettings> settings = Options.Values.OrderByDescending(itm => itm.Position).ToList();

                foreach (SpecialRequestRouteOptionSettings setting in settings)
                {
                    var matchedSupplier = options.FindRecord(itm => itm.SupplierId == setting.SupplierId);
                    if (matchedSupplier != null)
                    {
                        orderedOptions.Insert(0, matchedSupplier);
                    }
                }
            }

            return orderedOptions;
        }

        #endregion
    }
}