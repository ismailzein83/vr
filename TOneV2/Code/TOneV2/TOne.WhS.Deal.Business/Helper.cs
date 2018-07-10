using System;
using System.Collections.Generic;
using System.Linq;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.Deal.Entities;
using Vanrise.Common;

namespace TOne.WhS.Deal.Business
{
    public static class Helper
    {
        public static decimal GetDiscountedRateValue(decimal rate, int discount)
        {
            var discountValue = (rate * discount) / 100;
            var discountedRate = rate - discountValue;
            return discountedRate;
        }
        public static Dictionary<long, List<DealRate>> StructureDealRateByZoneId(IEnumerable<DealRate> dealRates)
        {
            var dealRateByZoneId = new Dictionary<long, List<DealRate>>();
            foreach (var dealRate in dealRates)
            {
                List<DealRate> rates = dealRateByZoneId.GetOrCreateItem(dealRate.ZoneId);
                rates.Add(dealRate);
            }
            return dealRateByZoneId;
        }
        public static List<DealSupplierZoneGroupZoneItem> BuildSupplierZones(IEnumerable<long> supplierZoneIds, DateTime beginDate, DateTime? dealEED)
        {
            if (supplierZoneIds == null || !supplierZoneIds.Any())
                throw new NullReferenceException("supplierZoneIds");

            var dealSupplierZoneGroupZoneItem = new List<DealSupplierZoneGroupZoneItem>();
            var supplierZoneManager = new SupplierZoneManager();

            foreach (var supplierZoneId in supplierZoneIds)
            {
                var supplierZone = supplierZoneManager.GetSupplierZone(supplierZoneId);

                DealSupplierZoneGroupZoneItem dealSupplierZone = new DealSupplierZoneGroupZoneItem
                {
                    ZoneId = supplierZoneId,
                    BED = beginDate,
                    EED = supplierZone.EED.MinDate(dealEED)
                };
                if (!dealSupplierZone.EED.VRLessThan(dealSupplierZone.BED))
                    dealSupplierZoneGroupZoneItem.Add(dealSupplierZone);
            }
            return dealSupplierZoneGroupZoneItem;
        }

        public static List<DealSaleZoneGroupZoneItem> BuildSaleZones(IEnumerable<long> saleZoneIds, DateTime beginDate, DateTime? dealEED)
        {
            if (saleZoneIds == null || !saleZoneIds.Any())
                throw new NullReferenceException("saleZoneIds");

            var dealSaleZoneGroupZoneItems = new List<DealSaleZoneGroupZoneItem>();
            var saleZoneManager = new SaleZoneManager();

            foreach (var saleZoneId in saleZoneIds)
            {
                var saleZone = saleZoneManager.GetSaleZone(saleZoneId);
                DealSaleZoneGroupZoneItem dealSaleZoneGroup = new DealSaleZoneGroupZoneItem
                {
                    ZoneId = saleZoneId,
                    BED = beginDate,
                    EED = saleZone.EED.MinDate(dealEED)
                };
                if (!dealSaleZoneGroup.EED.HasValue || dealSaleZoneGroup.EED.Value > dealSaleZoneGroup.BED)//  !dealSaleZoneGroup.EED.VRLessThanOrEqual(dealSaleZoneGroup.BED))
                    dealSaleZoneGroupZoneItems.Add(dealSaleZoneGroup);
            }
            return dealSaleZoneGroupZoneItems;
        }
    }
}
