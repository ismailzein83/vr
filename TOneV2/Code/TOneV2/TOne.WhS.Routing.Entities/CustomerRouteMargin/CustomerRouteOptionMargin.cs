using System;
using System.Collections.Generic;

namespace TOne.WhS.Routing.Entities
{
    public class BaseCustomerRouteOptionMargin
    {
        public long SupplierZoneID { get; set; }

        public HashSet<int> SupplierServiceIDs { get; set; }

        public Decimal SupplierRate { get; set; }

        public int? SupplierDealID { get; set; }
    }

    public class CustomerRouteOptionMargin : BaseCustomerRouteOptionMargin
    {
        public Decimal Margin { get; set; }

        public Guid? MarginCategoryID { get; set; }
    }
}