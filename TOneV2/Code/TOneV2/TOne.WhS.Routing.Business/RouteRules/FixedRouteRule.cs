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

        #region SaleEntity Execution

        public override RouteRuleSettings BuildLinkedRouteRuleSettings(ILinkedRouteRuleContext context)
        {
            FixedRouteRule fixedRouteRule = new FixedRouteRule();

            if (context.RouteOptions != null && context.RouteOptions.Count > 0)
            {
                fixedRouteRule.Options = new List<FixedRouteOptionSettings>();

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

        public override void CheckOptionFilter(ISaleEntityRouteRuleExecutionContext context, RouteRuleTarget target, BaseRouteOptionRuleTarget option)
        {
            FilterOption(context.SaleZoneServiceList, target, option);
        }

        public override void ExecuteForSaleEntity(ISaleEntityRouteRuleExecutionContext context, RouteRuleTarget target)
        {
            throw new NotSupportedException("ExecuteForSaleEntity is not supported for FixedRouteRule.");
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

        private void FilterOption(HashSet<int> customerServiceIds, RouteRuleTarget target, BaseRouteOptionRuleTarget option)
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

        #endregion
    }
}