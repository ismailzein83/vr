using System;
using System.Collections.Generic;

namespace TOne.WhS.Sales.Entities
{
    public class CountryPricingTemplate
    {
        public int CountryId { get; set; }

        public List<long> ExcludedZoneIds { get; set; }
    }
}
