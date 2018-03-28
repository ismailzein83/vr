using System.Collections.Generic;
using TOne.WhS.Deal.Entities;
using Vanrise.Common;

namespace TOne.WhS.Deal.Business
{
    public static class Helper
    {
        public static decimal GetDiscountedRateValue(decimal rate, int discount)
        {
            var discountValue = (rate * discount) / 100;
            var discountedRate = rate - discountValue;
            return discountedRate;
        }
        public static Dictionary<long, List<DealRate>> StructureDealRateByZoneId(IEnumerable<DealRate> dealRates)
        {
            var dealRateByZoneId = new Dictionary<long, List<DealRate>>();
            foreach (var dealRate in dealRates)
            {
                List<DealRate> rates = dealRateByZoneId.GetOrCreateItem(dealRate.ZoneId);
                rates.Add(dealRate);
            }
            return dealRateByZoneId;
        }
    }
}
