using System;
using System.Collections.Generic;
using System.Linq;
using TOne.WhS.Routing.Entities;

namespace TOne.WhS.Routing.Business
{
    public class LCRRouteRule : RouteRuleSettings
    {
        #region Properties/Ctor

        public override Guid ConfigId { get { return new Guid("31B3226E-A2B2-40D5-8C33-83C6601E8730"); } }

        public override CorrespondentType CorrespondentType { get { return Entities.CorrespondentType.LCR; } }

        public override bool UseOrderedExecution { get { return true; } }

        public List<ExcludedOption> ExcludedOptions { get; set; }

        #endregion

        #region SaleEntity Execution

        public override RouteRuleSettings BuildLinkedRouteRuleSettings(ILinkedRouteRuleContext context)
        {
            SpecialRequestRouteRule specialRequestRouteRule = new SpecialRequestRouteRule();
            if (context.RouteOptions != null && context.RouteOptions.Count > 0)
            {
                specialRequestRouteRule.Options = new List<SpecialRequestRouteOptionSettings>();
                int counter = 0;
                foreach (RouteOption routeOption in context.RouteOptions)
                {
                    counter++;
                    SpecialRequestRouteOptionSettings optionSettings = new SpecialRequestRouteOptionSettings()
                    {
                        ForceOption = false,
                        NumberOfTries = 1,
                        Percentage = routeOption.Percentage,
                        SupplierId = routeOption.SupplierId
                    };
                    specialRequestRouteRule.Options.Add(optionSettings);
                }
            }
            return specialRequestRouteRule;
        }

        public override List<RouteOptionRuleTarget> GetOrderedOptions(ISaleEntityRouteRuleExecutionContext context, RouteRuleTarget target)
        {
            var options = CreateOptions(context, target);
            if (options != null)
                return ApplyOptionsOrder(options);
            else
                return null;
        }

        public override void CheckOptionFilter(ISaleEntityRouteRuleExecutionContext context, TOne.WhS.Routing.Entities.RouteRuleTarget target, BaseRouteOptionRuleTarget option)
        {
            FilterOption(context.GetSupplierCodeMatch(option.SupplierId), context.SaleZoneServiceList, target, option);
        }

        public override void ExecuteForSaleEntity(ISaleEntityRouteRuleExecutionContext context, RouteRuleTarget target)
        {
            throw new NotSupportedException("ExecuteForSaleEntity is not supported for LCRRouteRule.");
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
                    FilterOption(supplierCodeMatch, context.SaleZoneServiceIds, target, option);
                    if (!option.FilterOption)
                        context.TryAddSupplierZoneOption(option);
                }
            }
        }

        public override void ApplyRuleToRPOptions(IRPRouteRuleExecutionContext context, ref IEnumerable<RPRouteOption> options)
        {
            if (options != null)
                options = ApplyOptionsOrder(options);
        }

        #endregion

        #region Private Methods

        private List<RouteOptionRuleTarget> CreateOptions(ISaleEntityRouteRuleExecutionContext context, RouteRuleTarget target)
        {
            var options = new List<RouteOptionRuleTarget>();

            var filteredSuppliersCodeMatches = context.GetFilteredSuppliersCodeMatches();
            if (filteredSuppliersCodeMatches != null)
            {
                foreach (var supplierCodeMatch in filteredSuppliersCodeMatches)
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
                SupplierServiceIds = supplierCodeMatchWithRate.SupplierServiceIds,
                SupplierServiceWeight = supplierCodeMatchWithRate.SupplierServiceWeight,
                NumberOfTries = 1,
                SupplierRateId = supplierCodeMatchWithRate.SupplierRateId,
                SupplierRateEED = supplierCodeMatchWithRate.SupplierRateEED
            };

            return option;
        }

        private void FilterOption(SupplierCodeMatchWithRate supplierCodeMatchWithRate, HashSet<int> customerServiceIds, RouteRuleTarget target, BaseRouteOptionRuleTarget option)
        {
            if (this.ExcludedOptions != null && this.ExcludedOptions.Any(itm => itm.SupplierId == option.SupplierId))
            {
                option.FilterOption = true;
                return;
            }

            if (ExecuteRateOptionFilter(target.SaleRate, option.SupplierRate))
            {
                option.FilterOption = true;
                return;
            }

            HashSet<int> supplierServices = supplierCodeMatchWithRate != null ? supplierCodeMatchWithRate.SupplierServiceIds : null;
            if (ExecuteServiceOptionFilter(customerServiceIds, supplierServices))
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

        private List<T> ApplyOptionsOrder<T>(IEnumerable<T> options) where T : IRouteOptionOrderTarget
        {
            options = options.OrderBy(itm => itm.SupplierRate).ThenByDescending(itm => itm.SupplierServiceWeight).ThenBy(itm => itm.SupplierId);
            return options != null ? options.ToList() : null;
        }

        #endregion

        #region Private Classes

        public class ExcludedOption
        {
            public int SupplierId { get; set; }
        }

        #endregion
    }
}
