using System;
using System.Collections.Generic;
using TOne.WhS.Deal.Entities;
using Vanrise.Common;

namespace TOne.WhS.Deal.MainExtensions
{
    public class DiscountSupplierRateEvaluator : DealSupplierRateEvaluator
    {
        public override Guid ConfigId { get { return new Guid("434BB6E0-A725-422E-A66A-BE839192AE5C"); } }
        public int Discount { get; set; }
        public override void EvaluateRate(IDealSupplierRateEvaluatorContext context)
        {
            var supplierRateByZoneId = new Dictionary<long, List<DealRate>>();

            foreach (var supplierZoneRate in context.SupplierZoneRateByZoneId)
            {
                List<DealRate> supplierDealRates = supplierRateByZoneId.GetOrCreateItem(supplierZoneRate.Key);
                var suplierRateValue = supplierZoneRate.Value;

                var discountValue = (suplierRateValue.Rate * Discount) / 100;
                var discountedRate = suplierRateValue.Rate - discountValue;

                supplierDealRates.Add(new DealRate
                {
                    Rate = discountedRate,
                    BED = suplierRateValue.BED,
                    EED = suplierRateValue.EED,
                    CurrencyId = suplierRateValue.CurrencyId.Value
                });
            }
            context.SupplierDealRatesByZoneId = supplierRateByZoneId;
        }

        public override string GetDescription()
        {
            return string.Format("{0}%", this.Discount);
        }
    }
}
