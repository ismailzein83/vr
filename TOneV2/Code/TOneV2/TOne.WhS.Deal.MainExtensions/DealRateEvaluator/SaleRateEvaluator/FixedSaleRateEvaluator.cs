using System;
using System.Collections.Generic;
using System.Linq;
using TOne.WhS.Deal.Entities;
using Vanrise.Common;

namespace TOne.WhS.Deal.MainExtensions
{
    public class FixedSaleRateEvaluator : DealSaleRateEvaluator
    {
        public override Guid ConfigId { get { return new Guid("2349B728-30EF-46B0-B54A-F9350F66FF2F"); } }
        public Decimal Rate { get; set; }
        public override void EvaluateRate(IDealSaleRateEvaluatorContext context)
        {
            var saleRates = new List<DealRate>();

            foreach (var zoneId in context.ZoneIds)
            {
                saleRates.Add(new DealRate
                {
                    ZoneId = zoneId,
                    BED = context.DealBED,
                    EED = context.DealEED,
                    Rate = Rate,
                    CurrencyId = context.CurrencyId
                });

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
