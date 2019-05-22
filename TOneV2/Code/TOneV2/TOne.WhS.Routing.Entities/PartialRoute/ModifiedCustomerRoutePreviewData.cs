namespace TOne.WhS.Routing.Entities
{
    public class ModifiedCustomerRoutePreviewData
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public string Code { get; set; }
        public long SaleZoneId { get; set; }

        public bool OrigIsBlocked { get; set; }
        public int? OrigExecutedRuleId { get; set; }
        public string OrigRouteOptions { get; set; }

        public bool IsBlocked { get; set; }
        public int? ExecutedRuleId { get; set; }
        public string RouteOptions { get; set; }
        public string SupplierIds { get; set; }

        public bool IsApproved { get; set; }
    }
}