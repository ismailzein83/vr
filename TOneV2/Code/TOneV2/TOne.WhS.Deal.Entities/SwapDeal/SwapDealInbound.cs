using System;
using System.Collections.Generic;

namespace TOne.WhS.Deal.Entities
{
    public class SwapDealInbound
    {
        public int ZoneGroupNumber { get; set; }

        public string Name { get; set; }

        public int CountryId { get; set; }

        public List<long> SaleZoneIds { get; set; }

        public int Volume { get; set; }

        public Decimal Rate { get; set; }

        public BaseDealRateEvaluator EvaluatedRate { get; set; }
    }
}