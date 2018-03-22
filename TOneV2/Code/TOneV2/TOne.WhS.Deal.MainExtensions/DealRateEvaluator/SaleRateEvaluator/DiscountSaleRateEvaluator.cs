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
            var saleRates = new List<DealRate>();
            var saleZoneManager = new SaleZoneManager();

            foreach (var zoneId in context.ZoneIds)
            {
                var zoneName = saleZoneManager.GetSaleZoneName(zoneId);
                var countryId = saleZoneManager.GetSaleZoneCountryId(zoneId);

                if (!countryId.HasValue)
                    continue; 

                IEnumerable<SaleRateHistoryRecord> saleRateHistoryRecords = context.GetCustomerZoneRatesFunc(zoneName, countryId.Value);

                if (saleRateHistoryRecords == null || !saleRateHistoryRecords.Any())
                    continue;

                foreach (var saleRate in saleRateHistoryRecords)
                {
                    var discountValue = (saleRate.Rate * Discount) / 100;
                    var discountedRate = saleRate.Rate - discountValue;

                    saleRates.Add(new DealRate
                    {
                        Rate = discountedRate,
                        BED = saleRate.BED,
                        EED = saleRate.EED,
                        CurrencyId = saleRate.CurrencyId
                    });
                }
            }
            if (saleRates.Any())
                context.SaleRates = saleRates;
        }
        public override string GetDescription()
        {
            return string.Format("{0}%", this.Discount);
        }
    }
}
