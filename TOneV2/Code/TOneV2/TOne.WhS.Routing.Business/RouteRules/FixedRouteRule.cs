﻿using System;
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

        public List<FixedRouteOptionSettings> Options { get; set; }

        public List<FixedRouteBackupOptionSettings> OverallBackupOptions { get; set; }

        private List<FixedRouteOptionSettings> _optionsWithBackups;

        private List<FixedRouteOptionSettings> OptionsWithBackups
        {
            get
            {
                if (_optionsWithBackups == null)
                {
                    if (this.Options != null)
                    {
                        _optionsWithBackups = Vanrise.Common.Utilities.CloneObject<List<FixedRouteOptionSettings>>(this.Options);

                        if (this.OverallBackupOptions != null)
                        {
                            foreach (FixedRouteOptionSettings option in _optionsWithBackups)
                            {
                                if (option.Backups != null)
                                    option.Backups.AddRange(this.OverallBackupOptions);
                                else
                                    option.Backups = new List<FixedRouteBackupOptionSettings>(this.OverallBackupOptions);
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
            if (context.SupplierIds == null || context.SupplierIds.Count == 0)
                return true;

            if (Options == null || Options.Count == 0)
                return false;

            foreach (var supplierId in context.SupplierIds)
            {
                foreach (var option in Options)
                {
                    if (option.SupplierId == supplierId)
                        return true;

                    if (option.Backups != null && option.Backups.Count > 0)
                    {
                        foreach (var backup in option.Backups)
                        {
                            if (backup.SupplierId == supplierId)
                                return true;
                        }
                    }
                }

                if (OverallBackupOptions != null && OverallBackupOptions.Count > 0)
                {
                    foreach (var overallbackup in OverallBackupOptions)
                    {
                        if (overallbackup.SupplierId == supplierId)
                            return true;
                    }
                }
            }
            return false;
        }

        public override string GetSuppliersDescription()
        {
            if (Options == null || Options.Count == 0)
                return null;

            CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
            List<string> suppliersWithBackups = new List<string>();

            foreach (var option in Options)
            {
                string optionDescription = carrierAccountManager.GetCarrierAccountName(option.SupplierId);
                if (option.Backups != null && option.Backups.Count > 0)
                {
                    IEnumerable<string> backupsNames = option.Backups.Select(item => carrierAccountManager.GetCarrierAccountName(item.SupplierId));
                    optionDescription = string.Concat(optionDescription, String.Format("<{0}>", string.Join(", ", backupsNames)));
                }
                suppliersWithBackups.Add(optionDescription);
            }

            string overallBackups = string.Empty;
            if (OverallBackupOptions != null && OverallBackupOptions.Count > 0)
                overallBackups = string.Concat(". Overall Backups: ", string.Join(", ", OverallBackupOptions.Select(item => carrierAccountManager.GetCarrierAccountName(item.SupplierId))));

            return string.Concat(string.Join<string>(", ", suppliersWithBackups), overallBackups);
        }

        #endregion

        #region SaleEntity Execution

        public override RouteRuleSettings BuildLinkedRouteRuleSettings(ILinkedRouteRuleContext context)
        {
            FixedRouteRule fixedRouteRule = new FixedRouteRule();

            if (context.RouteOptions != null && context.RouteOptions.Count > 0)
            {
                fixedRouteRule.Options = new List<FixedRouteOptionSettings>();
                fixedRouteRule.OverallBackupOptions = new List<FixedRouteBackupOptionSettings>();

                HashSet<int> overallAddedBackups = new HashSet<int>();

                foreach (RouteOption routeOption in context.RouteOptions)
                {
                    FixedRouteOptionSettings optionSettings = new FixedRouteOptionSettings() { Percentage = routeOption.Percentage, SupplierId = routeOption.SupplierId, NumberOfTries = routeOption.NumberOfTries };
                    fixedRouteRule.Options.Add(optionSettings);

                    if (!routeOption.IsLossy)
                    {
                        RouteRules.Filters.RateOptionFilter rateOptionFilter = new RouteRules.Filters.RateOptionFilter()
                        {
                            RateOption = RouteRules.Filters.RateOption.MaximumLoss,
                            RateOptionType = RouteRules.Filters.RateOptionType.Fixed,
                            RateOptionValue = 0
                        };
                        optionSettings.Filters = new List<RouteOptionFilterSettings>() { rateOptionFilter };
                    }

                    if (routeOption.Backups != null && routeOption.Backups.Count > 0)
                    {
                        optionSettings.Backups = new List<FixedRouteBackupOptionSettings>();
                        foreach (RouteBackupOption backup in routeOption.Backups)
                        {
                            if (overallAddedBackups.Contains(backup.SupplierId))
                                continue;

                            FixedRouteBackupOptionSettings backupSettings = new FixedRouteBackupOptionSettings()
                            {
                                NumberOfTries = backup.NumberOfTries,
                                SupplierId = backup.SupplierId
                            };

                            if (!backup.IsLossy)
                            {
                                RouteRules.Filters.RateOptionFilter rateOptionFilter = new RouteRules.Filters.RateOptionFilter()
                                {
                                    RateOption = RouteRules.Filters.RateOption.MaximumLoss,
                                    RateOptionType = RouteRules.Filters.RateOptionType.Fixed,
                                    RateOptionValue = 0
                                };
                                backupSettings.Filters = new List<RouteOptionFilterSettings>() { rateOptionFilter };
                            }

                            if (this.OverallBackupOptions != null && OverallBackupOptions.FirstOrDefault(itm => itm.SupplierId == backup.SupplierId) != null)
                            {
                                fixedRouteRule.OverallBackupOptions.Add(backupSettings);
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
                }

                if (fixedRouteRule.OverallBackupOptions.Count == 0)
                    fixedRouteRule.OverallBackupOptions = null;
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

        public override void CheckOptionFilter(ISaleEntityRouteRuleExecutionContext context, RouteRuleTarget target, BaseRouteOptionRuleTarget option, RoutingDatabase routingDatabase)
        {
            FilterOption(context.SaleZoneServiceList, target, option, routingDatabase);
        }

        public override void ExecuteForSaleEntity(ISaleEntityRouteRuleExecutionContext context, RouteRuleTarget target)
        {
            throw new NotSupportedException("ExecuteForSaleEntity is not supported for FixedRouteRule.");
        }
        public override RouteRuleSettings ExtendSuppliersList(RouteRuleSettings routeRuleSettings, List<RouteOption> routeOptions)
        {
            var fixedRouteRule = routeRuleSettings.CastWithValidate<FixedRouteRule>("FixedRouteRule");
            if (routeOptions == null || routeOptions.Count == 0)
                return fixedRouteRule;

            if (fixedRouteRule.Options == null)
                fixedRouteRule.Options = new List<FixedRouteOptionSettings>();

            foreach (var routeOption in routeOptions)
            {
                FixedRouteOptionSettings fixedRouteOptionSettings = new FixedRouteOptionSettings()
                {
                    Percentage = routeOption.Percentage,
                    SupplierId = routeOption.SupplierId,
                    NumberOfTries = routeOption.NumberOfTries
                };
                if (routeOption.Backups != null && routeOption.Backups.Count > 0)
                {
                    fixedRouteOptionSettings.Backups = new List<FixedRouteBackupOptionSettings>();
                    foreach (var backup in routeOption.Backups)
                    {
                        FixedRouteBackupOptionSettings fixedRouteBackupOptionSettings = new FixedRouteBackupOptionSettings()
                        {
                            NumberOfTries = backup.NumberOfTries,
                            SupplierId = backup.SupplierId
                        };

                        fixedRouteOptionSettings.Backups.Add(fixedRouteBackupOptionSettings);
                    }
                }
                fixedRouteRule.Options.Add(fixedRouteOptionSettings);
            }

            return fixedRouteRule;
        }

        #endregion

        #region RoutingProduct Execution

        public override void CreateSupplierZoneOptionsForRP(IRPRouteRuleExecutionContext context, RouteRuleTarget target)
        {
            if (this.OptionsWithBackups != null)
            {
                HashSet<int> addedSuppliers = new HashSet<int>();
                foreach (var optionSettings in this.OptionsWithBackups)
                {
                    var backups = optionSettings.Backups != null && optionSettings.Backups.Any() ? optionSettings.Backups.Select(itm => itm as IRouteBackupOptionSettings).ToList() : null;
                    context.CreateSupplierZoneOptionsForRP(target, FilterOption, optionSettings, backups, addedSuppliers);
                }
            }
        }

        public override void ApplyRuleToRPOptions(IRPRouteRuleExecutionContext context, ref IEnumerable<RPRouteOption> options)
        {
            if (options == null || this.OptionsWithBackups == null)
                return;

            bool isPercentage = this.OptionsWithBackups.FirstOrDefault(itm => itm.Percentage.HasValue) != null;
            if (isPercentage)
            {
                options = options.OrderByDescending(itm => itm.Percentage);
            }
            else
            {
                List<RPRouteOption> finalOptions = new List<RPRouteOption>();
                Dictionary<int, RPRouteOption> optionsBySupplier = options.ToDictionary(itm => itm.SupplierId, itm => itm);

                foreach (FixedRouteOptionSettings optionSettings in this.OptionsWithBackups)
                {
                    RPRouteOption rpRouteOption;
                    if (optionsBySupplier.TryGetValue(optionSettings.SupplierId, out rpRouteOption))
                        finalOptions.Add(rpRouteOption);
                }

                options = finalOptions;
            }
        }

        #endregion

        #region Private Methods

        private List<RouteOptionRuleTarget> CreateOptions(ISaleEntityRouteRuleExecutionContext context, RouteRuleTarget target)
        {
            var options = new List<RouteOptionRuleTarget>();
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

            return options;
        }

        private void FilterOption(HashSet<int> customerServiceIds, RouteRuleTarget target, BaseRouteOptionRuleTarget option, RoutingDatabase RoutingDatabase)
        {
            IFixedRouteOptionSettings fixedOption = option.OptionSettings as IFixedRouteOptionSettings;

            if (fixedOption == null)
                throw new NullReferenceException("option.OptionSettings should be of type IFixedRouteOptionSettings");

            if (fixedOption.Filters == null)
                return;

            foreach (var optionFilter in fixedOption.Filters)
            {
                var routeOptionFilterExecutionContext = new RouteOptionFilterExecutionContext()
                {
                    Option = option,
                    SaleRate = target.SaleRate,
                    CustomerServices = customerServiceIds,
                    SupplierServices = option.SupplierServiceIds,
                    SupplierId = option.SupplierId,
                    SupplierZoneId = option.SupplierZoneId,
                    RoutingDatabase = RoutingDatabase
                };
                optionFilter.Execute(routeOptionFilterExecutionContext);
                if (routeOptionFilterExecutionContext.FilterOption)
                {
                    option.FilterOption = true;
                    return;
                }
            }
        }

        #endregion
    }
}