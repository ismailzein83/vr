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

        public Dictionary<int, LCRRouteOptionSettings> Options { get; set; }

        public override bool UseOrderedExecution
        {
            get
            {
                return true;
            }
        }

        public override CorrespondentType CorrespondentType { get { return Entities.CorrespondentType.LCR; } }

        #endregion


        #region SaleEntity Execution

        public override List<RouteOptionRuleTarget> GetOrderedOptions(ISaleEntityRouteRuleExecutionContext context, RouteRuleTarget target)
        {
            var options = CreateOptions(context, target);
            if (options != null)
                return ApplyOptionsOrder(options);
            else
                return null;
        }

        public override bool IsOptionFiltered(ISaleEntityRouteRuleExecutionContext context, TOne.WhS.Routing.Entities.RouteRuleTarget target, TOne.WhS.Routing.Entities.RouteOptionRuleTarget option)
        {
            return FilterOption(context.GetSupplierCodeMatch(option.SupplierId), context.SaleZoneServiceList, target, option);
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
                    if (!FilterOption(supplierCodeMatch, context.SaleZoneServiceIds, target, option))
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

            return option;
        }

        private bool FilterOption(SupplierCodeMatchWithRate supplierCodeMatchWithRate, HashSet<int> customerServiceIds, RouteRuleTarget target, RouteOptionRuleTarget option)
        {
            HashSet<int> supplierServices = supplierCodeMatchWithRate != null ? supplierCodeMatchWithRate.SupplierServiceIds : null;

            if (ExecuteRateOptionFilter(target.SaleRate, option.SupplierRate))
                return true;

            if (ExecuteServiceOptionFilter(customerServiceIds, supplierServices))
                return true;

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

        private List<T> ApplyOptionsOrder<T>(IEnumerable<T> options) where T : IRouteOptionOrderTarget
        {
            options = options.OrderBy(itm => itm.SupplierRate).ThenByDescending(itm => itm.SupplierServiceWeight).ThenBy(itm => itm.SupplierId);
            return options != null ? options.ToList() : null;
        }

        #endregion
    }
}
