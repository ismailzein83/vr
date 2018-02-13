using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Routing.Entities
{
    public class BaseCustomerRouteOptionDetail
    {
        public int CustomerRouteOptionDetailId { get { return this.SupplierId; } }
        public int SupplierId { get; set; }
        public string SupplierName { get; set; }
        public long SupplierZoneId { get; set; }
        public string SupplierZoneName { get; set; }
        public string SupplierCode { get; set; }
        public Decimal SupplierRate { get; set; }
        public List<int> ExactSupplierServiceIds { get; set; }
        public int? ExecutedRuleId { get; set; }
        public List<int> LinkedRouteOptionRuleIds { get; set; }
        public bool IsBlocked { get; set; }

        //public bool IsForced { get; set; }
        //public bool IsLossy { get; set; }

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
