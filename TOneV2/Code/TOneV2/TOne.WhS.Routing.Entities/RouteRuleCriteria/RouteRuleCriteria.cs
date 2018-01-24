using System;
using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Routing.Entities;

namespace TOne.WhS.Routing.Entities
{
    public class RouteRuleCriteria : BaseRouteRuleCriteria
    {
        public override Guid ConfigId { get { return new Guid("6FD3F59F-33F1-4D42-8364-7030AE79B249"); } }

        public int? RoutingProductId { get; set; }

        public CodeCriteriaGroupSettings CodeCriteriaGroupSettings { get; set; }

        public SaleZoneGroupSettings SaleZoneGroupSettings { get; set; }

        public CustomerGroupSettings CustomerGroupSettings { get; set; }

        public RoutingExcludedDestinations ExcludedDestinations { get; set; }

        public override RoutingExcludedDestinations GetExcludedDestinations() { return ExcludedDestinations; }

        public override int? GetRoutingProductId() { return RoutingProductId; }

        public override CodeCriteriaGroupSettings GetCodeCriteriaGroupSettings() { return CodeCriteriaGroupSettings; }

        public override SaleZoneGroupSettings GetSaleZoneGroupSettings() { return SaleZoneGroupSettings; }

        public override CustomerGroupSettings GetCustomerGroupSettings() { return CustomerGroupSettings; }
    }
}