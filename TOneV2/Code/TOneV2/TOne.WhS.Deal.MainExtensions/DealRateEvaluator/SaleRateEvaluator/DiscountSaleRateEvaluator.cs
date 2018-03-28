using System;
using System.Collections.Generic;
using System.Linq;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Deal.Entities;
using Vanrise.Common;
using Vanrise.Entities;

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
                    throw new DataIntegrityValidationException(string.Format("Zone: {0} is not assigned to a countryId", zoneId));

                IEnumerable<SaleRateHistoryRecord> saleRateHistoryRecords = context.GetCustomerZoneRatesFunc(zoneName, countryId.Value);

                if (saleRateHistoryRecords == null || !saleRateHistoryRecords.Any())
                    continue;

                foreach (var saleRate in saleRateHistoryRecords)
                {
                    saleRates.Add(new DealRate
                    {
                        ZoneId = zoneId,
                        Rate = Business.Helper.GetDiscountedRateValue(saleRate.Rate, Discount),
                        BED = Utilities.Max(saleRate.BED, context.DealBED),
                        EED = saleRate.EED.MinDate(context.DealEED),
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
