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

        public List<SpecialRequestRouteOptionSettings> Options { get; set; }

        Dictionary<int, List<SpecialRequestRouteOptionSettings>> _optionsBySupplierId;
        Dictionary<int, List<SpecialRequestRouteOptionSettings>> OptionsBySupplierId
        {
            get
            {
                if (_optionsBySupplierId == null)
                {
                    if (Options != null)
                    {
                        _optionsBySupplierId = new Dictionary<int, List<SpecialRequestRouteOptionSettings>>();

                        foreach (SpecialRequestRouteOptionSettings option in Options)
                        {
                            List<SpecialRequestRouteOptionSettings> optionList = _optionsBySupplierId.GetOrCreateItem(option.SupplierId);
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
            SpecialRequestRouteRule specialRequestRouteRule = new SpecialRequestRouteRule();
            if (context.RouteOptions != null && context.RouteOptions.Count > 0)
            {
                Dictionary<int, List<SpecialRequestRouteOptionSettings>> clonedOptionsBySupplierId = null;
                if (OptionsBySupplierId != null)
                    clonedOptionsBySupplierId = Vanrise.Common.Utilities.CloneObject<Dictionary<int, List<SpecialRequestRouteOptionSettings>>>(OptionsBySupplierId);

                specialRequestRouteRule.Options = new List<SpecialRequestRouteOptionSettings>();
                int counter = 0;
                foreach (RouteOption routeOption in context.RouteOptions)
                {
                    counter++;
                    SpecialRequestRouteOptionSettings optionSettings;
                    List<SpecialRequestRouteOptionSettings> relatedOptions;
                    if (clonedOptionsBySupplierId != null && clonedOptionsBySupplierId.TryGetValue(routeOption.SupplierId, out relatedOptions) && relatedOptions.Count > 0)
                    {
                        SpecialRequestRouteOptionSettings relatedOption = relatedOptions.First();
                        relatedOptions.Remove(relatedOption);
                        optionSettings = new SpecialRequestRouteOptionSettings()
                        {
                            ForceOption = relatedOption.ForceOption,
                            NumberOfTries = routeOption.NumberOfTries,
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
                    specialRequestRouteRule.Options.Add(optionSettings);
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

            if (OptionsBySupplierId == null)
                return context.RouteOptions;

            int blockedOptionsCount = context.RouteOptions.Count(itm => itm.IsBlocked || itm.IsFiltered);

            int totalCount = blockedOptionsCount + context.NumberOfOptionsInSettings;
            if (context.RouteOptions.Count <= totalCount)
                return context.RouteOptions;

            int routeOptionsCount = context.RouteOptions.Count;
            for (int i = routeOptionsCount - 1; i >= totalCount; i--)
            {
                var currentRouteOption = context.RouteOptions[i];
                if (!OptionsBySupplierId.ContainsKey(currentRouteOption.SupplierId))
                    context.RouteOptions.Remove(currentRouteOption);
                else
                    break;
            }
            return context.RouteOptions;
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
                    var option = CreateRPOption(target, supplierCodeMatch);
                    if (!FilterOption(supplierCodeMatch, context.SaleZoneServiceIds, target, option))
                        context.TryAddSupplierZoneOption(option);
                }
            }
        }

        public override void ApplyRuleToRPOptions(IRPRouteRuleExecutionContext context, ref IEnumerable<RPRouteOption> options)
        {
            if (options != null)
            {
                options = ApplyRPOptionsOrder(options, null);

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


        private List<RPRouteOption> ApplyRPOptionsOrder(IEnumerable<RPRouteOption> options, int? numberOfOptions)
        {
            if (options == null)
                return null;

            List<SpecialRequestRPRouteOption> rpRouteOptions = new List<SpecialRequestRPRouteOption>();

            List<SpecialRequestRouteOptionSettings> relatedOptions;
            foreach (RPRouteOption option in options)
            {
                SpecialRequestRPRouteOption specialRequestRPRouteOption;
                if (OptionsBySupplierId.TryGetValue(option.SupplierId, out relatedOptions) && relatedOptions.Count > 0)
                {
                    SpecialRequestRouteOptionSettings relatedOption = relatedOptions.First();
                    specialRequestRPRouteOption = new SpecialRequestRPRouteOption() { Position = relatedOption.Position, RPRouteOption = option };
                }
                else
                {
                    specialRequestRPRouteOption = new SpecialRequestRPRouteOption() { Position = Int32.MaxValue, RPRouteOption = option };
                }
                rpRouteOptions.Add(specialRequestRPRouteOption);

            }
            return rpRouteOptions.OrderBy(itm => itm.Position).ThenBy(itm => itm.RPRouteOption.SupplierRate).ThenByDescending(itm => itm.RPRouteOption.SupplierServiceWeight).
                ThenBy(itm => itm.RPRouteOption.SupplierId).Select(itm => itm.RPRouteOption).ToList();
        }
        #endregion

        #region Private Methods

        private List<SpecialRequestRouteOptionRuleTarget> CreateOptions(ISaleEntityRouteRuleExecutionContext context, RouteRuleTarget target)
        {
            var options = new List<SpecialRequestRouteOptionRuleTarget>();

            var allSuppliersCodeMatches = context.GetAllSuppliersCodeMatches();
            if (allSuppliersCodeMatches != null)
            {
                foreach (var supplierCodeMatch in allSuppliersCodeMatches)
                {
                    var optionsCreated = CreateOptionList(target, supplierCodeMatch);
                    options.AddRange(optionsCreated);
                }
            }

            return options;
        }

        private List<SpecialRequestRouteOptionRuleTarget> CreateOptionList(RouteRuleTarget routeRuleTarget, SupplierCodeMatchWithRate supplierCodeMatchWithRate)
        {
            var supplierCodeMatch = supplierCodeMatchWithRate.CodeMatch;
            List<SpecialRequestRouteOptionSettings> supplierSettings = null;

            if (OptionsBySupplierId != null)
                supplierSettings = OptionsBySupplierId.GetRecord(supplierCodeMatch.SupplierId);

            List<SpecialRequestRouteOptionRuleTarget> options = new List<SpecialRequestRouteOptionRuleTarget>();
            if (supplierSettings != null && supplierSettings.Count > 0)
            {
                foreach (SpecialRequestRouteOptionSettings supplierSetting in supplierSettings)
                {
                    var option = BuildRouteOptionRuleTarget(routeRuleTarget, supplierCodeMatchWithRate, supplierCodeMatch, supplierSetting.Percentage, supplierSetting.NumberOfTries, supplierSetting.Position, supplierSetting.ForceOption);
                    options.Add(option);
                }
            }
            else
            {
                var option = BuildRouteOptionRuleTarget(routeRuleTarget, supplierCodeMatchWithRate, supplierCodeMatch, null, 1, Int32.MaxValue, false);
                options.Add(option);
            }
            return options;
        }

        private SpecialRequestRouteOptionRuleTarget CreateRPOption(RouteRuleTarget routeRuleTarget, SupplierCodeMatchWithRate supplierCodeMatchWithRate)
        {
            var supplierCodeMatch = supplierCodeMatchWithRate.CodeMatch;
            List<SpecialRequestRouteOptionSettings> supplierSettings = null;

            if (OptionsBySupplierId != null)
                supplierSettings = OptionsBySupplierId.GetRecord(supplierCodeMatch.SupplierId);

            List<SpecialRequestRouteOptionRuleTarget> options = new List<SpecialRequestRouteOptionRuleTarget>();
            if (supplierSettings != null && supplierSettings.Count > 0)
            {
                bool forceOption = false;
                int? percentage = null;
                int numberOfTries = 0;
                int position = 0;

                foreach (SpecialRequestRouteOptionSettings supplierSetting in supplierSettings)
                {
                    forceOption = forceOption || supplierSetting.ForceOption;
                    numberOfTries = Math.Max(numberOfTries, supplierSetting.NumberOfTries);
                    position = Math.Min(position, supplierSetting.Position);

                    if (supplierSetting.Percentage.HasValue)
                    {
                        if (!percentage.HasValue)
                            percentage = supplierSetting.Percentage.Value;
                        else
                            percentage += supplierSetting.Percentage.Value;
                    }
                }

                return BuildRouteOptionRuleTarget(routeRuleTarget, supplierCodeMatchWithRate, supplierCodeMatch, percentage, numberOfTries, position, forceOption);
            }
            else
            {
                return BuildRouteOptionRuleTarget(routeRuleTarget, supplierCodeMatchWithRate, supplierCodeMatch, null, 1, Int32.MaxValue, false);
            }
        }

        private SpecialRequestRouteOptionRuleTarget BuildRouteOptionRuleTarget(RouteRuleTarget routeRuleTarget, SupplierCodeMatchWithRate supplierCodeMatchWithRate, SupplierCodeMatch supplierCodeMatch,
            int? percentage, int numberOfTries, int position, bool forceOption)
        {
            var option = new SpecialRequestRouteOptionRuleTarget
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
                NumberOfTries = numberOfTries,
                SupplierRateId = supplierCodeMatchWithRate.SupplierRateId,
                SupplierRateEED = supplierCodeMatchWithRate.SupplierRateEED,
                Position = position,
                ForceOption = forceOption
            };
            return option;
        }

        private bool FilterOption(SupplierCodeMatchWithRate supplierCodeMatchWithRate, HashSet<int> customerServiceIds, RouteRuleTarget target, RouteOptionRuleTarget option)
        {
            SpecialRequestRouteOptionRuleTarget castOption = option.CastWithValidate<SpecialRequestRouteOptionRuleTarget>("option");
            bool checkFilters = !castOption.ForceOption;

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

        private List<RouteOptionRuleTarget> ApplyOptionsOrder<T>(IEnumerable<T> options, int? numberOfOptions) where T : ISpecialRequestRouteOptionOrderTarget
        {
            if (options == null)
                return null;

            return options.OrderBy(itm => itm.Position).ThenBy(itm => itm.SupplierRate).ThenByDescending(itm => itm.SupplierServiceWeight).ThenBy(itm => itm.SupplierId).Select(itm => itm as RouteOptionRuleTarget).ToList();
        }

        #endregion

        private class SpecialRequestRouteOptionRuleTarget : RouteOptionRuleTarget, ISpecialRequestRouteOptionOrderTarget
        {
            public int Position { get; set; }
            public bool ForceOption { get; set; }
        }

        private interface ISpecialRequestRouteOptionOrderTarget : IRouteOptionOrderTarget
        {
            int Position { get; set; }
        }

        private class SpecialRequestRPRouteOption
        {
            public RPRouteOption RPRouteOption { get; set; }
            public int Position { get; set; }
        }
    }
}
