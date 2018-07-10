using System;
using System.Linq;
using TOne.WhS.Deal.Entities;
using System.Collections.Generic;

namespace TOne.WhS.Deal.MainExtensions
{
    public class FixedSupplierRateEvaluator : DealSupplierRateEvaluator
    {
        public override Guid ConfigId { get { return new Guid("F89330E6-F3C2-4B85-B303-3C1FACAE6AC6"); } }
        public decimal Rate { get; set; }
        public override void EvaluateRate(IDealSupplierRateEvaluatorContext context)
        {
            var supplierRates = new List<DealRate>();
            foreach (var dealZoneItem in context.SupplierZoneItem)
            {
                DealRate dealRate = new DealRate
                {
                    ZoneId = dealZoneItem.ZoneId,
                    BED = dealZoneItem.BED,
                    EED = dealZoneItem.EED,
                    Rate = Rate,
                    CurrencyId = context.CurrencyId
                };
                if (!dealRate.EED.HasValue || dealRate.BED < dealRate.EED.Value)
                    supplierRates.Add(dealRate);
            }

            if (supplierRates.Any())
                context.SupplierRates = supplierRates;
        }
        public override string GetDescription()
        {
            return string.Format("{0}", this.Rate);
        }
    }
}
