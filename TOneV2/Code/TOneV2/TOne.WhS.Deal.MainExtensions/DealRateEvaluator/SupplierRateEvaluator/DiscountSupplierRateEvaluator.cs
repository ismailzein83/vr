using System;
using System.Collections.Generic;
using System.Linq;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
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
            var supplierRateManager = new SupplierRateManager();
            var supplierDealRates = new List<DealRate>();

            foreach (var zoneId in context.ZoneIds)
            {
                SupplierRate supplierRate = context.SupplierZoneRateByZoneId.GetRecord(zoneId);

                if (supplierRate == null)
                    continue;

                var supplierCurrencyId = supplierRateManager.GetCurrencyId(supplierRate);

                supplierDealRates.Add(new DealRate
                {
                    ZoneId = zoneId,
                    Rate = Business.Helper.GetDiscountedRateValue(supplierRate.Rate, Discount),
                    BED = Utilities.Max(supplierRate.BED, context.DealBED),
                    EED = supplierRate.EED.MaxDate(context.DealEED),
                    CurrencyId = supplierCurrencyId
                });
            }
            if (supplierDealRates.Any())
                context.SupplierRates = supplierDealRates;
        }

        public override string GetDescription()
        {
            return string.Format("{0}%", this.Discount);
        }
    }
}