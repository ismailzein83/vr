using System;
using System.Collections.Generic;
using System.Linq;
using TOne.WhS.Deal.Business;
using TOne.WhS.Deal.Entities;
using Vanrise.Common;

namespace TOne.WhS.Deal.MainExtensions
{
    public class FixedSupplierRateEvaluator : DealSupplierRateEvaluator
    {
        public override Guid ConfigId { get { return new Guid("F89330E6-F3C2-4B85-B303-3C1FACAE6AC6"); } }
        public decimal Rate { get; set; }
        public override void EvaluateRate(IDealSupplierRateEvaluatorContext context)
        {
            var suplierRateByZoneId = new Dictionary<long, List<DealRate>>();

            foreach (var zoneId in context.ZoneIds)
            {
                List<DealRate> rates = suplierRateByZoneId.GetOrCreateItem(zoneId);

                rates.Add(new DealRate
                {
                    BED = context.DealBED,
                    EED = context.DealEED,
                    Rate = Rate,
                    CurrencyId = context.CurrencyId
                });
            }
            if (suplierRateByZoneId.Any())
                context.SupplierDealRatesByZoneId = suplierRateByZoneId;
        }
        public override string GetDescription()
        {
            return string.Format("{0}", this.Rate);
        }
    }
}
