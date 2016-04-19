namespace TOne.WhS.Routing.Entities
{
    public class TrafficStatsMeasure
    {
        public int TotalNumberOfAttempts { get; set; }

        public int TotalDeliveredAttempts { get; set; }

        public int TotalSuccesfulAttempts { get; set; }

        public decimal TotalDurationInSeconds { get; set; }
    }
    public class SaleZoneSupplierTrafficStatsMeasure : TrafficStatsMeasure
    {
        public int SupplierId { get; set; }
        public long SaleZoneId { get; set; }

    }

    public class SupplierZoneTrafficStatsMeasure : TrafficStatsMeasure
    {
        public long SupplierZoneId { get; set; }
    }
}
