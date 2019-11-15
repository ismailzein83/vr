using System;

namespace TOne.WhS.Routing.Entities
{
    public class BaseCustomerRouteMarginSummary : BaseCustomerRouteMargin
    {
        public Decimal MinSupplierRate { get; set; }

        public Decimal MaxMargin { get; set; }

        public Guid? MaxMarginCategoryID { get; set; }

        public Decimal MaxSupplierRate { get; set; }

        public Decimal MinMargin { get; set; }

        public Guid? MinMarginCategoryID { get; set; }
    }

    public class CustomerRouteMarginSummaryStaging : BaseCustomerRouteMarginSummary
    {
        public int NumberOfCodes { get; set; }

        public Decimal SumOfSupplierRate { get; set; }
    }

    public class CustomerRouteMarginSummary : BaseCustomerRouteMarginSummary
    {
        public long CustomerRouteMarginSummaryID { get; set; } 

        public Decimal AvgSupplierRate { get; set; }

        public Decimal AvgMargin { get; set; }

        public Guid? AvgMarginCategoryID { get; set; }
    }
}