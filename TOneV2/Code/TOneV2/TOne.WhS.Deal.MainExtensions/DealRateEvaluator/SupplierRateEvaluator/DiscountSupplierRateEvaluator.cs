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

            foreach (var supplierZoneItem in context.SupplierZoneItem)
            {
                SupplierRate supplierRate = context.SupplierZoneRateByZoneId.GetRecord(supplierZoneItem.ZoneId);

                if (supplierRate == null)
                    continue;

                var supplierCurrencyId = supplierRateManager.GetCurrencyId(supplierRate);

                DealRate dealRate = new DealRate
                 {
                     ZoneId = supplierZoneItem.ZoneId,
                     Rate = Business.Helper.GetDiscountedRateValue(supplierRate.Rate, Discount),
                     BED = Utilities.Max(supplierRate.BED, supplierZoneItem.BED),
                     EED = supplierRate.EED.MinDate(supplierZoneItem.EED),
                     CurrencyId = supplierCurrencyId
                 };
                if (!dealRate.EED.HasValue || dealRate.BED < dealRate.EED.Value)
                    supplierDealRates.Add(dealRate);
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