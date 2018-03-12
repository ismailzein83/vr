using System;
using System.Collections.Generic;

namespace TOne.WhS.Routing.Entities
{
    public abstract class BaseRPRouteDetail
    {
        public long SaleZoneId { get; set; }

        public string SaleZoneName { get; set; }

        public HashSet<int> SaleZoneServiceIds { get; set; }

        public string SellingNumberPlan { get; set; }

        public bool IsBlocked { get; set; }

        public int ExecutedRuleId { get; set; }

        public decimal? EffectiveRateValue { get; set; }

        public string CurrencySymbol { get; set; }
    }

    public class RPRouteDetailByZone : BaseRPRouteDetail
    {
        public int RoutingProductId { get; set; }
        public string RoutingProductName { get; set; }
        public IEnumerable<RPRouteOptionDetail> RouteOptionsDetails { get; set; }

    }

    public class RPRouteDetailByCode : BaseRPRouteDetail
    {
        public string Code { get; set; }
        public int RoutingProductId { get; set; }
        public string RoutingProductName { get; set; }
        public IEnumerable<RPRouteOptionByCodeDetail> RouteOptionsDetails { get; set; }
    }
}