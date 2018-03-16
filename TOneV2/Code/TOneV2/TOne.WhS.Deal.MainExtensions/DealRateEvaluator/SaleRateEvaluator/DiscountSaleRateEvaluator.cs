using System;
using System.Collections.Generic;
using System.Linq;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Deal.Entities;
using Vanrise.Common;

namespace TOne.WhS.Deal.MainExtensions
{
    public class DiscountSaleRateEvaluator : DealSaleRateEvaluator
    {
        public override Guid ConfigId { get { return new Guid("49AF4D76-C067-47DA-A600-A1EA0E1AAD99"); } }
        public int Discount { get; set; }
        public override void EvaluateRate(IDealSaleRateEvaluatorContext context)
        {
            var saleRateByZoneId = new Dictionary<long, List<DealRate>>();
            var saleZoneManager = new SaleZoneManager();

            foreach (var zoneId in context.ZoneIds)
            {
                var zoneName = saleZoneManager.GetSaleZoneName(zoneId);
                var countryId = saleZoneManager.GetSaleZoneCountryId(zoneId);

                if (!countryId.HasValue)
                    return; //Throw exception

                IEnumerable<SaleRateHistoryRecord> saleRates = context.GetCustomerZoneRatesFunc(zoneName, countryId.Value);

                List<DealRate> dealRates = saleRateByZoneId.GetOrCreateItem(zoneId);
                foreach (var saleRate in saleRates)
                {
                    var discountValue = (saleRate.Rate * Discount) / 100;
                    var discountedRate = saleRate.Rate - discountValue;

                    dealRates.Add(new DealRate
                    {
                        Rate = discountedRate,
                        BED = saleRate.BED,
                        EED = saleRate.EED,
                        CurrencyId = saleRate.CurrencyId
                    });
                }
            }
            if (saleRateByZoneId.Any())
                context.SaleRatesByZoneId = saleRateByZoneId;
        }
        public override string GetDescription()
        {
            return string.Format("{0}%", this.Discount);
        }
    }
}
