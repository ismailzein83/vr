using System.Collections.Generic;

namespace TOne.WhS.Sales.Entities
{
    public class ZoneChanges
    {
        public long ZoneId { get; set; }

        public string ZoneName { get; set; }

        public IEnumerable<DraftRateToChange> NewRates { get; set; }

        public IEnumerable<DraftRateToClose> ClosedRates { get; set; }

        public DraftNewSaleZoneRoutingProduct NewRoutingProduct { get; set; }

        public DraftChangedSaleZoneRoutingProduct RoutingProductChange { get; set; }

        public NewService NewService { get; set; }

        public ServiceChange ServiceChange { get; set; }
    }
}
