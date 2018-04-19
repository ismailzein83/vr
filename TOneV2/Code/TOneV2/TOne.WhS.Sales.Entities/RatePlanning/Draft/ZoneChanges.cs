using System;
using System.Collections.Generic;

namespace TOne.WhS.Sales.Entities
{
    public class ZoneChanges
    {
        public long ZoneId { get; set; }

        public string ZoneName { get; set; }

        public int CountryId { get; set; }

        public IEnumerable<DraftRateToChange> NewRates { get; set; }
        public DateTime? NewOtherRateBED { get; set; }
        public decimal? ProfitPerc { get; set; }

        public IEnumerable<DraftRateToClose> ClosedRates { get; set; }

        public DraftNewSaleZoneRoutingProduct NewRoutingProduct { get; set; }

        public DraftChangedSaleZoneRoutingProduct RoutingProductChange { get; set; }

        public DraftNewZoneService NewService { get; set; }

        public DraftClosedZoneService ClosedService { get; set; }

        public DraftResetZoneService ResetService { get; set; }
    }
}
