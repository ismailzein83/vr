using System;

namespace TOne.WhS.RouteSync.Entities
{
    public class RouteOption
    {
        public string SupplierId { get; set; }
        public decimal? SupplierRate { get; set; }
        public Decimal? Percentage { get; set; }
        public bool IsBlocked { get; set; }
        public int NumberOfTries { get; set; }
        public bool IsValid { get { return NumberOfTries > 0; } }
    }
}
