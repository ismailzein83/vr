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

        public List<string> ExcludedCodes { get; set; }

        public SaleZoneGroupSettings SaleZoneGroupSettings { get; set; }

        public CustomerGroupSettings CustomerGroupSettings { get; set; }

        public override int? GetRoutingProductId() { return RoutingProductId; }
        public override CodeCriteriaGroupSettings GetCodeCriteriaGroupSettings() { return CodeCriteriaGroupSettings; }
        public override List<string> GetExcludedCodes() { return ExcludedCodes; }
        public override SaleZoneGroupSettings GetSaleZoneGroupSettings() { return SaleZoneGroupSettings; }
        public override CustomerGroupSettings GetCustomerGroupSettings() { return CustomerGroupSettings; }
    }
}