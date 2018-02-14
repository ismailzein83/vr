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

        public List<SpecialRequestRouteBackupOptionSettings> OverallBackupOptions { get; set; }

        private HashSet<int> _optionSupplierIds;
        private HashSet<int> OptionsSupplierIds
        {
            get
            {
                if (_optionSupplierIds == null)
                {
                    _optionSupplierIds = new HashSet<int>();
                    if (this.OptionsWithBackups != null)
                    {
                        foreach (SpecialRequestRouteOptionSettings option in this.OptionsWithBackups)
                            _optionSupplierIds.Add(option.SupplierId);
                    }
                }
                return _optionSupplierIds;
            }
        }

        private List<SpecialRequestRouteOptionSettings> _optionsWithBackups;
        private List<SpecialRequestRouteOptionSettings> OptionsWithBackups
        {
            get
            {
                if (_optionsWithBackups == null)
                {
                    if (this.Options != null)
                    {
                        _optionsWithBackups = Vanrise.Common.Utilities.CloneObject<List<SpecialRequestRouteOptionSettings>>(this.Options);

                        if (this.OverallBackupOptions != null)
                        {
                            foreach (SpecialRequestRouteOptionSettings option in _optionsWithBackups)
                            {
                                if (option.Backups != null)
                                    option.Backups.AddRange(this.OverallBackupOptions);
                                else
                                    option.Backups = new List<SpecialRequestRouteBackupOptionSettings>(this.OverallBackupOptions);
                            }
                        }
                    }
                }
                return _optionsWithBackups;
            }
        }

        #endregion

        #region SaleEntity Execution

        public override RouteRuleSettings BuildLinkedRouteRuleSettings(ILinkedRouteRuleContext context)
        {
            SpecialRequestRouteRule specialRequestRouteRule = new SpecialRequestRouteRule();
            if (context.RouteOptions != null && context.RouteOptions.Count > 0)
            {
                specialRequestRouteRule.Options = new List<SpecialRequestRouteOptionSettings>();
                foreach (RouteOption routeOption in context.RouteOptions)
                {
                    SpecialRequestRouteOptionSettings optionSettings;
                    if (_optionSupplierIds != null && _optionSupplierIds.Contains(routeOption.SupplierId))
                    {
                        optionSettings = new SpecialRequestRouteOptionSettings()
                        {
                            ForceOption = routeOption.IsForced,
                            NumberOfTries = routeOption.NumberOfTries,
                            Percentage = routeOption.Percentage,
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
                throw new NullReferenceException("numberOfOptions must have a value for Special Request Route Rule");

            return this.OptionsWithBackups != null ? Math.Max(numberOfOptions.Value, this.OptionsWithBackups.Count) : numberOfOptions.Value;
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
                    context.RouteOptions.RemoveAt(i);
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
                HashSet<int> addedSuppliers = new HashSet<int>();
                if (this.OptionsWithBackups != null)
                {
                    foreach (var optionSettings in this.OptionsWithBackups)
                    {
                        var backups = optionSettings.Backups != null && optionSettings.Backups.Any() ? optionSettings.Backups.Select(itm => itm as IRouteBackupOptionSettings).ToList() : null;
                        context.CreateSupplierZoneOptionsForRP(target, FilterOption, optionSettings, backups, addedSuppliers);
                    }
                }

                foreach (var supplierCodeMatch in allSuppliersCodeMatches)
                {
                    if (OptionsSupplierIds == null || !OptionsSupplierIds.Contains(supplierCodeMatch.CodeMatch.SupplierId))
                    {
                        SpecialRequestRouteOptionSettings optionSettings = new SpecialRequestRouteOptionSettings() { NumberOfTries = 1, SupplierId = supplierCodeMatch.CodeMatch.SupplierId };
                        context.CreateSupplierZoneOptionsForRP(target, FilterOption, optionSettings, null, addedSuppliers);
                    }
                }
            }
        }

        public override void ApplyRuleToRPOptions(IRPRouteRuleExecutionContext context, ref IEnumerable<RPRouteOption> options)
        {
            if (options == null)
                return;

            bool isPercentage = this.OptionsWithBackups != null && this.OptionsWithBackups.FirstOrDefault(itm => itm.Percentage.HasValue) != null;
            if (isPercentage)
            {
                options = options.OrderByDescending(itm => itm.Percentage.HasValue ? itm.Percentage : -1).ThenBy(itm => itm.SupplierRate).ThenByDescending(itm => itm.SupplierServiceWeight).ThenBy(itm => itm.SupplierId);
            }
            else
            {
                if (this.OptionsWithBackups != null)
                {
                    List<RPRouteOption> finalOptions = new List<RPRouteOption>();
                    Dictionary<int, RPRouteOption> optionsBySupplier = options.ToDictionary(itm => itm.SupplierId, itm => itm);

                    foreach (SpecialRequestRouteOptionSettings optionSettings in this.OptionsWithBackups)
                    {
                        RPRouteOption rpRouteOption;
                        if (optionsBySupplier.TryGetValue(optionSettings.SupplierId, out rpRouteOption))
                            finalOptions.Add(rpRouteOption);
                    }

                    var lcrOptions = options.FindAllRecords(itm => this.OptionsSupplierIds == null || !this.OptionsSupplierIds.Contains(itm.SupplierId));
                    if (lcrOptions.Any())
                        finalOptions.AddRange(lcrOptions.OrderBy(itm => itm.SupplierRate).ThenByDescending(itm => itm.SupplierServiceWeight).ThenBy(itm => itm.SupplierId));

                    options = finalOptions;
                }
                else
                {
                    options = options.OrderBy(itm => itm.SupplierRate).ThenByDescending(itm => itm.SupplierServiceWeight).ThenBy(itm => itm.SupplierId);
                }
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
                if (this.OptionsWithBackups != null)
                {
                    foreach (var optionSettings in this.OptionsWithBackups)
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

        private void FilterOption(HashSet<int> customerServiceIds, RouteRuleTarget target, BaseRouteOptionRuleTarget option)
        {
            ISpecialRequestRouteOptionSettings specialRequestOption = option.OptionSettings as ISpecialRequestRouteOptionSettings;
            if (specialRequestOption == null)
                throw new NullReferenceException("option.OptionSettings should be of type ISpecialRequestRouteOptionSettings");

            if (ExecuteRateOptionFilter(target.SaleRate, option.SupplierRate))
            {
                if (specialRequestOption.ForceOption)
                    option.IsForced = true;
                else
                    option.FilterOption = true;

                return;
            }

            if (ExecuteServiceOptionFilter(customerServiceIds, option.SupplierServiceIds))
            {
                if (specialRequestOption.ForceOption)
                    option.IsForced = true;
                else
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

        #endregion
    }
}