using System;
using System.Collections.Generic;

namespace TOne.WhS.Routing.Entities
{
    public enum CustomerRouteMarginTableType { Current = 1, Future = 2 };

    public class BaseCustomerRouteMargin
    {
        public int CustomerID { get; set; }

        public long SaleZoneID { get; set; }

        public Decimal SaleRate { get; set; }

        public int? SaleDealID { get; set; } 
    }

    public class CustomerRouteMargin : BaseCustomerRouteMargin
    {
        public long CustomerRouteMarginID { get; set; } 

        public CustomerRouteOptionMargin CustomerRouteOptionMargin { get; set; }

        public CustomerRouteOptionMargin OptimalCustomerRouteOptionMargin { get; set; }

        public bool IsRisky { get; set; }
    }

    public struct CustomerRouteMarginIdentifier
    {
        public int CustomerID { get; set; }

        public long SaleZoneID { get; set; }

        public long SupplierZoneID { get; set; }

        public override int GetHashCode()
        {
            return this.CustomerID.GetHashCode() + this.SaleZoneID.GetHashCode() + this.SupplierZoneID.GetHashCode();
        }
    }
}