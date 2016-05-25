

namespace TOne.WhS.Sales.Entities
{
    public class ZoneChanges
    {
        public long ZoneId { get; set; }

        public DraftNewRate NewRate { get; set; }

        public DraftChangedRate RateChange { get; set; }

        public NewZoneRoutingProduct NewRoutingProduct { get; set; }

        public ZoneRoutingProductChange RoutingProductChange { get; set; }

        public NewService NewService { get; set; }

        public ServiceChange ServiceChange { get; set; }
    }
}
