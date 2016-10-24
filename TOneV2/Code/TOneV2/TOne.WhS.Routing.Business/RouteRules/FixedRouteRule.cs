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

        public List<RouteOptionSettings> Options { get; set; }

        public Dictionary<int, List<RouteOptionFilterSettings>> Filters { get; set; }

        public override Guid ConfigId
        {
            get { return new Guid("12E2CAD6-ABD9-4D2B-B2F3-51C3DF501DE9"); }
        }

        public override bool UseOrderedExecution
        {
            get
            {
                return true;
            }
        }

        public override CorrespondentType CorrespondentType
        {
            get
            {
                return Entities.CorrespondentType.Override;
            }
        }

        #endregion


        #region SaleEntity Execution

        public override List<RouteOptionRuleTarget> GetOrderedOptions(ISaleEntityRouteRuleExecutionContext context, RouteRuleTarget target)
        {
            return CreateOptions(context, target);
        }

        public override bool IsOptionFiltered(ISaleEntityRouteRuleExecutionContext context, RouteRuleTarget target, RouteOptionRuleTarget option)
        {
            return FilterOption(context.GetSupplierCodeMatch(option.SupplierId), context.CustomerServiceIdHashSet, target, option);
        }

        public override void ApplyOptionsPercentage(IEnumerable<RouteOption> options)
        {
            base.ApplyOptionsPercentage(options);
            ApplyOptionsPercentage<RouteOption>(options);
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

                var fixedOptions = this.Options;
                HashSet<int> filteredSupplierIds = SupplierGroupContext.GetFilteredSupplierIds(supplierFilterSettings);
                if (filteredSupplierIds != null)
                {
                    var filteredSuppliers = fixedOptions.Where(option => filteredSupplierIds.Contains(option.SupplierId));
                    if (filteredSuppliers != null && filteredSuppliers.Count() > 0)
                        fixedOptions = filteredSuppliers.ToList();
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
                                if (!FilterOption(supplierCodeMatch, null, target, option))
                                    context.TryAddSupplierZoneOption(option);
                            }
                        }
                    }
                }
            }
        }

        public override void ApplyRuleToRPOptions(IRPRouteRuleExecutionContext context, ref IEnumerable<RPRouteOption> options)
        {
            ApplyOptionsPercentage<RPRouteOption>(options);
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
                        var option = CreateOption(target, optionSupplierCodeMatch, optionSettings.Percentage);
                        options.Add(option);
                    }
                }
            }

            return options;
        }

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
                ExactSupplierServiceIds = supplierCodeMatchWithRate.ExactSupplierServiceIds,
                SupplierServiceWeight = supplierCodeMatchWithRate.SupplierServiceWeight
            };
            if (percentage.HasValue)
                option.Percentage = percentage.Value;
            return option;
        }

        private bool FilterOption(SupplierCodeMatchWithRate supplierCodeMatchWithRate, HashSet<int> customerServiceIds, RouteRuleTarget target, RouteOptionRuleTarget option)
        {
            if (Filters != null)
            {
                List<RouteOptionFilterSettings> optionFilters = Filters.GetRecord(supplierCodeMatchWithRate.CodeMatch.SupplierId);
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

            return false;
        }

        private void ApplyOptionsPercentage<T>(IEnumerable<T> options) where T : IRouteOptionPercentageTarget
        {
            if (options == null)
                return;

            decimal totalAssignedPercentage = options.Sum(itm => itm.Percentage.Value);
            if (totalAssignedPercentage == 100 || totalAssignedPercentage == 0)
                return;

            decimal unassignedPercentages = 100 - totalAssignedPercentage;

            foreach (var option in options)
            {
                option.Percentage = decimal.Round(option.Percentage.Value + option.Percentage.Value * unassignedPercentages / totalAssignedPercentage, 2);
            }
        }

        #endregion
    }
}