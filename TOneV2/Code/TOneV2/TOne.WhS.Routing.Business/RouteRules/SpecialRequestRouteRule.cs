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

        HashSet<int> _optionSupplierIds;

        HashSet<int> OptionsSupplierIds
        {
            get
            {
                if (_optionSupplierIds == null)
                {
                    _optionSupplierIds = new HashSet<int>();
                    if (Options != null)
                    {
                        foreach (SpecialRequestRouteOptionSettings option in Options)
                            _optionSupplierIds.Add(option.SupplierId);
                    }
                }
                return _optionSupplierIds;
            }
        }

        #endregion

        #region SaleEntity Execution

        public override RouteRuleSettings BuildLinkedRouteRuleSettings(ILinkedRouteRuleContext context)
        {
            SpecialRequestRouteRule specialRequestRouteRule = new SpecialRequestRouteRule();
            //if (context.RouteOptions != null && context.RouteOptions.Count > 0)
            //{
            //    specialRequestRouteRule.Options = new List<SpecialRequestRouteOptionSettings>();
            //    int counter = 0;
            //    foreach (RouteOption routeOption in context.RouteOptions)
            //    {
            //        counter++;
            //        SpecialRequestRouteOptionSettings optionSettings;
            //        SpecialRequestRouteOptionSettings relatedOption;
            //        if (Options != null && Options.TryGetValue(routeOption.SupplierId, out relatedOption))
            //        {
            //            optionSettings = new SpecialRequestRouteOptionSettings()
            //            {
            //                ForceOption = relatedOption.ForceOption,
            //                NumberOfTries = relatedOption.NumberOfTries,
            //                Percentage = routeOption.Percentage,
            //                Position = counter,
            //                SupplierId = routeOption.SupplierId
            //            };
            //        }
            //        else
            //        {
            //            optionSettings = new SpecialRequestRouteOptionSettings()
            //            {
            //                ForceOption = false,
            //                NumberOfTries = 1,
            //                Percentage = routeOption.Percentage,
            //                Position = counter,
            //                SupplierId = routeOption.SupplierId
            //            };
            //        }
            //        specialRequestRouteRule.Options.Add(optionSettings);
            //    }
            //}
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
            return CreateOptions(context, target);
        }

        public override void CheckOptionFilter(ISaleEntityRouteRuleExecutionContext context, TOne.WhS.Routing.Entities.RouteRuleTarget target, BaseRouteOptionRuleTarget option)
        {
            FilterOption(context.SaleZoneServiceList, target, option);
        }

        public override void ExecuteForSaleEntity(ISaleEntityRouteRuleExecutionContext context, RouteRuleTarget target)
        {
            throw new NotSupportedException("ExecuteForSaleEntity is not supported for SpecialRequestRouteRule.");
        }

        public override List<RouteOption> GetFinalOptions(IFinalizeRouteOptionContext context)
        {
            if (context.RouteOptions == null)
                return null;

            if (OptionsSupplierIds == null)
                return context.RouteOptions;

            int blockedOptionsCount = context.RouteOptions.Count(itm => itm.IsFullyBlocked());

            int totalCount = blockedOptionsCount + context.NumberOfOptionsInSettings;
            if (context.RouteOptions.Count <= totalCount)
                return context.RouteOptions;

            int routeOptionsCount = context.RouteOptions.Count;
            for (int i = routeOptionsCount - 1; i >= totalCount; i--)
            {
                var currentRouteOption = context.RouteOptions[i];
                if (!OptionsSupplierIds.Contains(currentRouteOption.SupplierId))
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
                    FilterOption(context.SaleZoneServiceIds, target, option);
                    if (!option.FilterOption)
                        context.TryAddSupplierZoneOption(option);
                }
            }
        }

        public override void ApplyRuleToRPOptions(IRPRouteRuleExecutionContext context, ref IEnumerable<RPRouteOption> options)
        {
            //if (options != null)
            //{
            //    options = ApplyOptionsOrder(options, null);

            //    int? totalAssignedPercentage = null;

            //    var blockedOptions = options.FindAllRecords(itm => itm.SupplierStatus == SupplierStatus.Block);
            //    if (blockedOptions != null)
            //    {
            //        foreach (var blockedOption in blockedOptions)
            //            blockedOption.Percentage = null;
            //    }

            //    var unblockedOptions = options.FindAllRecords(itm => itm.SupplierStatus != SupplierStatus.Block);
            //    if (unblockedOptions != null)
            //    {
            //        var unblockedOptionsWithPercentage = unblockedOptions.FindAllRecords(itm => itm.Percentage.HasValue);
            //        if (unblockedOptionsWithPercentage != null && unblockedOptionsWithPercentage.Count() > 0)
            //            totalAssignedPercentage = unblockedOptionsWithPercentage.Sum(itm => itm.Percentage.Value);
            //    }

            //    if (!totalAssignedPercentage.HasValue || totalAssignedPercentage == 100 || totalAssignedPercentage == 0)
            //        return;

            //    int unassignedPercentages = 100 - totalAssignedPercentage.Value;

            //    int newTotalAssignedPercentage = 0;
            //    RPRouteOption rpRouteOptionWithHighestPercentage = null;

            //    foreach (var option in options)
            //    {
            //        if (!option.Percentage.HasValue)
            //            continue;

            //        if (option.SupplierStatus == SupplierStatus.Block)
            //        {
            //            option.Percentage = 0;
            //            continue;
            //        }

            //        option.Percentage = option.Percentage.Value + option.Percentage.Value * unassignedPercentages / totalAssignedPercentage.Value;
            //        newTotalAssignedPercentage += option.Percentage.Value;

            //        if (rpRouteOptionWithHighestPercentage == null || rpRouteOptionWithHighestPercentage.Percentage < option.Percentage)
            //            rpRouteOptionWithHighestPercentage = option;
            //    }

            //    if (newTotalAssignedPercentage != 100)
            //        rpRouteOptionWithHighestPercentage.Percentage = rpRouteOptionWithHighestPercentage.Percentage.Value + (100 - newTotalAssignedPercentage);
            //}
        }

        #endregion

        #region Private Methods

        private List<RouteOptionRuleTarget> CreateOptions(ISaleEntityRouteRuleExecutionContext context, RouteRuleTarget target)
        {
            var options = new List<RouteOptionRuleTarget>();

            var allSuppliersCodeMatches = context.GetAllSuppliersCodeMatches();
            if (allSuppliersCodeMatches != null)
            {
                if (this.Options != null)
                {
                    foreach (var optionSettings in this.Options)
                    {
                        var backups = optionSettings.Backups != null && optionSettings.Backups.Any() ? optionSettings.Backups.Select(itm => itm as IRouteBackupOptionSettings).ToList() : null;
                        RouteOptionRuleTarget routeOptionRuleTarget = context.BuildRouteOptionRuleTarget(target, optionSettings, backups);
                        if (routeOptionRuleTarget != null)
                            options.Add(routeOptionRuleTarget);
                    }
                }

                List<RouteOptionRuleTarget> lcrOptions = new List<RouteOptionRuleTarget>();
                foreach (var supplierCodeMatch in allSuppliersCodeMatches)
                {
                    if (OptionsSupplierIds == null || !OptionsSupplierIds.Contains(supplierCodeMatch.CodeMatch.SupplierId))
                    {
                        SpecialRequestRouteOptionSettings optionSettings = new SpecialRequestRouteOptionSettings() { NumberOfTries = 1, SupplierId = supplierCodeMatch.CodeMatch.SupplierId };
                        RouteOptionRuleTarget routeOptionRuleTarget = context.BuildRouteOptionRuleTarget(target, optionSettings, null);
                        if (routeOptionRuleTarget != null)
                            lcrOptions.Add(routeOptionRuleTarget);
                    }
                }
                if (lcrOptions.Any())
                    options.AddRange(lcrOptions.OrderBy(itm => itm.SupplierRate).ThenByDescending(itm => itm.SupplierServiceWeight).ThenBy(itm => itm.SupplierId));
            }

            return options;
        }

        private RouteOptionRuleTarget CreateOption(RouteRuleTarget routeRuleTarget, SupplierCodeMatchWithRate supplierCodeMatchWithRate)
        {
            throw new NotImplementedException();
            //var supplierCodeMatch = supplierCodeMatchWithRate.CodeMatch;
            //SpecialRequestRouteOptionSettings supplierSettings;
            //int? numberOfTries = null;
            //int? percentage = null;

            //if (Options != null && Options.TryGetValue(supplierCodeMatch.SupplierId, out supplierSettings))
            //{
            //    numberOfTries = supplierSettings.NumberOfTries;
            //    percentage = supplierSettings.Percentage;
            //}

            //var option = new RouteOptionRuleTarget
            //{
            //    RouteTarget = routeRuleTarget,
            //    SupplierId = supplierCodeMatch.SupplierId,
            //    SupplierCode = supplierCodeMatch.SupplierCode,
            //    SupplierZoneId = supplierCodeMatch.SupplierZoneId,
            //    SupplierRate = supplierCodeMatchWithRate.RateValue,
            //    EffectiveOn = routeRuleTarget.EffectiveOn,
            //    IsEffectiveInFuture = routeRuleTarget.IsEffectiveInFuture,
            //    ExactSupplierServiceIds = supplierCodeMatchWithRate.ExactSupplierServiceIds,
            //    SupplierServiceIds = supplierCodeMatchWithRate.SupplierServiceIds,
            //    SupplierServiceWeight = supplierCodeMatchWithRate.SupplierServiceWeight,
            //    Percentage = percentage,
            //    NumberOfTries = numberOfTries.HasValue ? numberOfTries.Value : 1,
            //    SupplierRateId = supplierCodeMatchWithRate.SupplierRateId,
            //    SupplierRateEED = supplierCodeMatchWithRate.SupplierRateEED
            //};

            //return option;
        }

        private void FilterOption(HashSet<int> customerServiceIds, RouteRuleTarget target, BaseRouteOptionRuleTarget option)
        {
            ISpecialRequestRouteOptionSettings specialRequestOption = option.OptionSettings as ISpecialRequestRouteOptionSettings;

            if (specialRequestOption == null)
                throw new NullReferenceException("option.OptionSettings should be of type ISpecialRequestRouteOptionSettings");

            if (specialRequestOption.ForceOption)
                return;

            if (ExecuteRateOptionFilter(target.SaleRate, option.SupplierRate))
            {
                option.FilterOption = true;
                return;
            }

            if (ExecuteServiceOptionFilter(customerServiceIds, option.SupplierServiceIds))
            {
                option.FilterOption = true;
                return;
            }
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

        //private List<T> ApplyOptionsOrder<T>(IEnumerable<T> options, int? numberOfOptions) where T : IRouteOptionOrderTarget
        //{
        //    if (options == null)
        //        return null;

        //    IEnumerable<T> finalOptions = new List<T>(options);

        //    if (Options != null)
        //        finalOptions = finalOptions.FindAllRecords(itm => !Options.ContainsKey(itm.SupplierId));

        //    finalOptions = finalOptions.OrderBy(itm => itm.SupplierRate).ThenByDescending(itm => itm.SupplierServiceWeight).ThenBy(itm => itm.SupplierId);

        //    List<T> orderedOptions = finalOptions.ToList();

        //    if (Options != null)
        //    {
        //        List<SpecialRequestRouteOptionSettings> settings = Options.Values.OrderByDescending(itm => itm.Position).ToList();

        //        foreach (SpecialRequestRouteOptionSettings setting in settings)
        //        {
        //            var matchedSupplier = options.FindRecord(itm => itm.SupplierId == setting.SupplierId);
        //            if (matchedSupplier != null)
        //            {
        //                orderedOptions.Insert(0, matchedSupplier);
        //            }
        //        }
        //    }

        //    return orderedOptions;
        //}

        #endregion
    }
}