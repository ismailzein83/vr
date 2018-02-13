using System;
using System.Collections.Generic;

namespace TOne.WhS.Routing.Entities
{
    public class CustomerRouteDetail
    {
        public string CustomerRouteDetailId { get { return string.Format("{0}@{1}", this.CustomerId, this.Code); } }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string SaleZoneName { get; set; }
        public long SaleZoneId { get; set; }
        public string Code { get; set; }
        public Decimal? Rate { get; set; }
        public bool IsBlocked { get; set; }
        public HashSet<int> SaleZoneServiceIds { get; set; }
        public int? ExecutedRuleId { get; set; }
        public List<RouteOption> Options { get; set; }
        public List<CustomerRouteOptionDetail> RouteOptionDetails { get; set; }
        public List<int> LinkedRouteRuleIds { get; set; }
        public string ExecutedRouteRuleName { get; set; }
        public string ExecutedRouteRuleSettingsTypeName { get; set; }
        public int LinkedRouteRuleCount { get { return LinkedRouteRuleIds != null ? LinkedRouteRuleIds.Count : 0; } }
        public bool CanEditMatchingRule { get; set; }
    }

    public class BaseCustomerRouteOptionDetail
    {
        public int CustomerRouteOptionDetailId { get { return this.SupplierId; } }
        public int SupplierId { get; set; }
        public string SupplierName { get; set; }
        public long SupplierZoneId { get; set; }
        public string SupplierZoneName { get; set; }
        public string SupplierCode { get; set; }
        public Decimal SupplierRate { get; set; }
        public bool IsBlocked { get; set; }
        public List<int> ExactSupplierServiceIds { get; set; }
        public int? ExecutedRuleId { get; set; }
        public List<int> LinkedRouteOptionRuleIds { get; set; }
        public int LinkedRouteOptionRuleCount { get { return LinkedRouteOptionRuleIds != null ? LinkedRouteOptionRuleIds.Count : 0; } }
    }

    public class CustomerRouteOptionDetail : BaseCustomerRouteOptionDetail
    {
        public int? Percentage { get; set; }

        public List<CustomerRouteBackupOptionDetail> Backups { get; set; }
    }

    public class CustomerRouteBackupOptionDetail : BaseCustomerRouteOptionDetail
    {

    }
}