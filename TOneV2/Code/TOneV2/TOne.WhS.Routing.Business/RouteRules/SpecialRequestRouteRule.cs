﻿using System;
using System.Collections.Generic;
using System.Linq;
using TOne.WhS.BusinessEntity.Business;
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

        public Dictionary<int, ExcludedSpecialRequestOption> ExcludedOptions { get; set; }

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
                        List<SpecialRequestRouteBackupOptionSettings> _overallBackupOptions = null;
                        if (this.OverallBackupOptions != null)
                        {
                            _overallBackupOptions = Vanrise.Common.Utilities.CloneObject<List<SpecialRequestRouteBackupOptionSettings>>(this.OverallBackupOptions);
                            for (var i = _overallBackupOptions.Count - 1; i >= 0; i--)
                            {
                                SpecialRequestRouteBackupOptionSettings overallBackupOption = _overallBackupOptions[i];
                                if (this.ExcludedOptions != null && this.ExcludedOptions.ContainsKey(overallBackupOption.SupplierId))
                                    _overallBackupOptions.RemoveAt(i);
                            }

                            if (_overallBackupOptions.Count == 0)
                                _overallBackupOptions = null;
                        }

                        _optionsWithBackups = Vanrise.Common.Utilities.CloneObject<List<SpecialRequestRouteOptionSettings>>(this.Options);
                        for (var j = _optionsWithBackups.Count - 1; j >= 0; j--)
                        {
                            SpecialRequestRouteOptionSettings option = _optionsWithBackups[j];
                            if (this.ExcludedOptions != null && this.ExcludedOptions.ContainsKey(option.SupplierId))
                            {
                                _optionsWithBackups.RemoveAt(j);
                                continue;
                            }

                            if (option.Backups != null)
                            {
                                for (var k = option.Backups.Count - 1; k >= 0; k--)
                                {
                                    SpecialRequestRouteBackupOptionSettings backupOption = option.Backups[k];
                                    if (this.ExcludedOptions != null && this.ExcludedOptions.ContainsKey(backupOption.SupplierId))
                                        option.Backups.RemoveAt(k);
                                }

                                if (option.Backups.Count == 0)
                                    option.Backups = null;
                            }

                            if (_overallBackupOptions != null)
                            {
                                if (option.Backups != null)
                                    option.Backups.AddRange(_overallBackupOptions);
                                else
                                    option.Backups = new List<SpecialRequestRouteBackupOptionSettings>(_overallBackupOptions);
                            }
                        }
                    }
                }
                return _optionsWithBackups;
            }
        }

        #endregion
        #region Public Methods
        public override bool AreSuppliersIncluded(IRouteRuleAreSuppliersIncludedContext context)
        {
            if (context.SupplierIds == null || context.SupplierIds.Count == 0 || ExcludedOptions == null || ExcludedOptions.Count == 0)
                return true;

            foreach (var supplierId in context.SupplierIds)
                if (!ExcludedOptions.ContainsKey(supplierId))
                    return true;

            return false;
        }

        public override string GetSuppliersDescription()
        {
            if (ExcludedOptions == null || ExcludedOptions.Count == 0)
                return "All Suppliers";

            CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
            IEnumerable<string> supplierNames = ExcludedOptions.Select(item => carrierAccountManager.GetCarrierAccountName(item.Key));

            return String.Format("All Suppliers Except: {0}", string.Join(", ", supplierNames));
        }
        public override RouteRuleSettings ExtendSuppliersList(RouteRuleSettings routeRuleSettings, List<RouteOption> routeOptions)
        {
            var specialRequestRouteRule = routeRuleSettings.CastWithValidate<SpecialRequestRouteRule>("SpecialRequestRouteRule");
            if (routeOptions == null || routeOptions.Count == 0)
                return specialRequestRouteRule;

            if (specialRequestRouteRule.Options == null)
                specialRequestRouteRule.Options = new List<SpecialRequestRouteOptionSettings>();

            foreach (var routeOption in routeOptions)
            {
                if (ExcludedOptions != null && ExcludedOptions.Any(item => item.Value.SupplierId == routeOption.SupplierId))
                    continue;

                SpecialRequestRouteOptionSettings specialRequestRouteOptionSettings = new SpecialRequestRouteOptionSettings()
                {
                    Percentage = routeOption.Percentage,
                    SupplierId = routeOption.SupplierId,
                    NumberOfTries = routeOption.NumberOfTries
                };
                if (routeOption.Backups != null && routeOption.Backups.Count > 0)
                {
                    specialRequestRouteOptionSettings.Backups = new List<SpecialRequestRouteBackupOptionSettings>();
                    foreach (var backup in routeOption.Backups)
                    {
                        if (ExcludedOptions != null && ExcludedOptions.Any(item => item.Value.SupplierId == backup.SupplierId))
                            continue;

                        SpecialRequestRouteBackupOptionSettings specialRequestRouteBackupOptionSettings = new SpecialRequestRouteBackupOptionSettings()
                        {
                            NumberOfTries = backup.NumberOfTries,
                            SupplierId = backup.SupplierId
                        };

                        specialRequestRouteOptionSettings.Backups.Add(specialRequestRouteBackupOptionSettings);
                    }
                }
                specialRequestRouteRule.Options.Add(specialRequestRouteOptionSettings);
            }
            return specialRequestRouteRule;
        }
        #endregion

        #region SaleEntity Execution

        public override RouteRuleSettings BuildLinkedRouteRuleSettings(ILinkedRouteRuleContext context)
        {
            SpecialRequestRouteRule specialRequestRouteRule = new SpecialRequestRouteRule();
            if (context.RouteOptions != null && context.RouteOptions.Count > 0)
            {
                specialRequestRouteRule.Options = new List<SpecialRequestRouteOptionSettings>();
                specialRequestRouteRule.OverallBackupOptions = new List<SpecialRequestRouteBackupOptionSettings>();

                bool hasPercentages = context.RouteOptions.Any(itm => itm.Percentage.HasValue);
                HashSet<int> overallAddedBackups = new HashSet<int>();

                foreach (RouteOption routeOption in context.RouteOptions)
                {
                    SpecialRequestRouteOptionSettings optionSettings;
                    if (OptionsSupplierIds != null && OptionsSupplierIds.Contains(routeOption.SupplierId))
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
                        if (hasPercentages)//option to be ignored; option from lcr in special request
                            continue;

                        optionSettings = new SpecialRequestRouteOptionSettings()
                        {
                            ForceOption = false,
                            NumberOfTries = 1,
                            Percentage = null,
                            SupplierId = routeOption.SupplierId
                        };
                    }
                    if (routeOption.Backups != null && routeOption.Backups.Count > 0)
                    {
                        optionSettings.Backups = new List<SpecialRequestRouteBackupOptionSettings>();
                        foreach (RouteBackupOption backup in routeOption.Backups)
                        {
                            if (overallAddedBackups.Contains(backup.SupplierId))
                                continue;

                            SpecialRequestRouteBackupOptionSettings backupSettings = new SpecialRequestRouteBackupOptionSettings()
                            {
                                ForceOption = backup.IsForced,
                                NumberOfTries = backup.NumberOfTries,
                                SupplierId = backup.SupplierId
                            };

                            if (this.OverallBackupOptions != null && OverallBackupOptions.FirstOrDefault(itm => itm.SupplierId == backup.SupplierId) != null)
                            {
                                specialRequestRouteRule.OverallBackupOptions.Add(backupSettings);
                                overallAddedBackups.Add(backup.SupplierId);
                            }
                            else
                            {
                                optionSettings.Backups.Add(backupSettings);
                            }
                        }

                        if (optionSettings.Backups.Count == 0)
                            optionSettings.Backups = null;
                    }
                    specialRequestRouteRule.Options.Add(optionSettings);
                }

                if (specialRequestRouteRule.OverallBackupOptions.Count == 0)
                    specialRequestRouteRule.OverallBackupOptions = null;
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

        public override void CheckOptionFilter(ISaleEntityRouteRuleExecutionContext context, TOne.WhS.Routing.Entities.RouteRuleTarget target, BaseRouteOptionRuleTarget option, RoutingDatabase routingDatabase)
        {
            FilterOption(context.SaleZoneServiceList, target, option, routingDatabase);
        }

        public override void ExecuteForSaleEntity(ISaleEntityRouteRuleExecutionContext context, RouteRuleTarget target)
        {
            throw new NotSupportedException("ExecuteForSaleEntity is not supported for SpecialRequestRouteRule.");
        }

        public override List<RouteOption> GetFinalOptions(IFinalizeRouteOptionContext context)
        {
            if (context.RouteOptions == null)
                return null;

            if (OptionsSupplierIds == null || !context.NumberOfOptionsInSettings.HasValue)
                return context.RouteOptions;

            int blockedOptionsCount = context.RouteOptions.Count(itm => itm.IsFullyBlocked());

            int totalCount = blockedOptionsCount + context.NumberOfOptionsInSettings.Value;
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
                    if (this.ExcludedOptions != null && this.ExcludedOptions.ContainsKey(supplierCodeMatch.CodeMatch.SupplierId))
                        continue;

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
                    if (this.ExcludedOptions != null && this.ExcludedOptions.ContainsKey(supplierCodeMatch.CodeMatch.SupplierId))
                        continue;

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

        private void FilterOption(HashSet<int> customerServiceIds, RouteRuleTarget target, BaseRouteOptionRuleTarget option, RoutingDatabase routingDatabase)
        {
            ISpecialRequestRouteOptionSettings specialRequestOption = option.OptionSettings as ISpecialRequestRouteOptionSettings;
            if (specialRequestOption == null)
                throw new NullReferenceException("option.OptionSettings should be of type ISpecialRequestRouteOptionSettings");

            if (specialRequestOption.SupplierDeals != null && specialRequestOption.SupplierDeals.Count > 0)
            {
                var supplierDeals = specialRequestOption.SupplierDeals.Select(item => item as BaseRouteSupplierDeal).ToList();
                if (!Entities.Helper.IsSupplierDealMatch(supplierDeals, option.SupplierDealId))
                {
                    option.FilterOption = true;
                    return;
                }
            }

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

        #region Private Classes

        public class ExcludedSpecialRequestOption
        {
            public int SupplierId { get; set; }
        }

        #endregion
    }
}