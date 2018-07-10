using System;
using System.Linq;
using TOne.WhS.Deal.Entities;
using System.Collections.Generic;

namespace TOne.WhS.Deal.MainExtensions
{
    public class FixedSaleRateEvaluator : DealSaleRateEvaluator
    {
        public override Guid ConfigId { get { return new Guid("2349B728-30EF-46B0-B54A-F9350F66FF2F"); } }
        public Decimal Rate { get; set; }
        public override void EvaluateRate(IDealSaleRateEvaluatorContext context)
        {
            var saleRates = new List<DealRate>();

            foreach (var zoneItem in context.SaleZoneGroupItem)
            {
                DealRate dealRate = new DealRate
                {
                    ZoneId = zoneItem.ZoneId,
                    BED = zoneItem.BED,
                    EED = zoneItem.EED,
                    Rate = Rate,
                    CurrencyId = context.CurrencyId
                };
                if (!dealRate.EED.HasValue || dealRate.BED < dealRate.EED.Value)
                    saleRates.Add(dealRate);
            }
            if (saleRates.Any())
                context.SaleRates = saleRates;
        }
        public override string GetDescription()
        {
            return string.Format("{0}", this.Rate);
        }
    }
}
