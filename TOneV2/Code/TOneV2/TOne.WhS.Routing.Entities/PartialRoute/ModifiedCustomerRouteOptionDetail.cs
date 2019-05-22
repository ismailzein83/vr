using System.Collections.Generic;

namespace TOne.WhS.Routing.Entities
{
    public abstract class BaseModifiedCustomerRouteOptionDetail
    {
        public string SupplierName { get; set; }
        public string SupplierZoneName { get; set; }
        public string SupplierCode { get; set; }
        public decimal SupplierRate { get; set; }
        public string ExactSupplierServiceSymbols { get; set; }
        public bool IsBlocked { get; set; }
        public bool IsForced { get; set; }
        public bool IsLossy { get; set; }
        public RouteOptionEvaluatedStatus? EvaluatedStatus { get; set; }
    }

    public class ModifiedCustomerRouteOptionDetail : BaseModifiedCustomerRouteOptionDetail
    {
        public int? Percentage { get; set; }
        public List<ModifiedCustomerRouteBackupOptionDetail> Backups { get; set; }

    }

    public class ModifiedCustomerRouteBackupOptionDetail : BaseModifiedCustomerRouteOptionDetail
    {
    }
}