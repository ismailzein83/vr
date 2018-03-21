using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Deal.Entities;

namespace TOne.WhS.Deal.Business
{
    public class VolCommitmentDealSettings : DealSettings
    {
        public static Guid VolCommitmentDealSettingsConfigId = new Guid("B606E88C-4AE5-4BF0-BCE5-10D456A092F5");
        public override Guid ConfigId { get { return VolCommitmentDealSettingsConfigId; } }

        public VolCommitmentDealType DealType { get; set; }

        public int CarrierAccountId { get; set; }

        public List<VolCommitmentDealItem> Items { get; set; }

        public int LastGroupNumber { get; set; }

        public int CurrencyId { get; set; }

        public override bool ValidateDataBeforeSave(IValidateBeforeSaveContext validateBeforeSaveContext)
        {
            return true;
        }


        public override void GetZoneGroups(IDealGetZoneGroupsContext context)
        {
            switch (DealType)
            {
                case VolCommitmentDealType.Buy:
                    if (Items != null)
                    {
                        List<DealSupplierZoneGroup> dealSupplierZoneGroups = new List<DealSupplierZoneGroup>();
                        foreach (VolCommitmentDealItem volCommitmentDealItem in Items)
                        {
                            var zoneIds = volCommitmentDealItem.SaleZones.Select(z => z.ZoneId);
                            DealSupplierZoneGroup dealSupplierZoneGroup = new DealSupplierZoneGroup()
                            {
                                DealId = context.DealId,
                                BED = BeginDate,
                                DealSupplierZoneGroupNb = volCommitmentDealItem.ZoneGroupNumber,
                                EED = EndDate,
                                SupplierId = CarrierAccountId,
                                Tiers = BuildSupplierTiers(volCommitmentDealItem.Tiers, zoneIds),
                                Zones = BuildSupplierZones(zoneIds)
                            };
                            dealSupplierZoneGroups.Add(dealSupplierZoneGroup);
                        }
                        context.SupplierZoneGroups = dealSupplierZoneGroups;
                    }
                    break;
                case VolCommitmentDealType.Sell:
                    if (Items != null)
                    {
                        List<DealSaleZoneGroup> dealSaleZoneGroups = new List<DealSaleZoneGroup>();
                        foreach (VolCommitmentDealItem volCommitmentDealItem in Items)
                        {
                            var zoneIds = volCommitmentDealItem.SaleZones.Select(z => z.ZoneId);
                            DealSaleZoneGroup dealSaleZoneGroup = new DealSaleZoneGroup()
                            {
                                DealId = context.DealId,
                                BED = BeginDate,
                                DealSaleZoneGroupNb = volCommitmentDealItem.ZoneGroupNumber,
                                EED = EndDate,
                                CustomerId = CarrierAccountId,
                                Tiers = BuildSaleTiers(volCommitmentDealItem.Tiers, zoneIds),
                                Zones = BuildSaleZones(zoneIds)
                            };
                            dealSaleZoneGroups.Add(dealSaleZoneGroup);
                        }
                        context.SaleZoneGroups = dealSaleZoneGroups;
                    }
                    break;
            }
        }

        #region Sale Methods
        private List<DealSaleZoneGroupZoneItem> BuildSaleZones(IEnumerable<long> zoneIds)
        {
            if (zoneIds == null || !zoneIds.Any())
                throw new NullReferenceException("zoneIds");

            List<DealSaleZoneGroupZoneItem> dealSaleZoneGroupZoneItems = new List<DealSaleZoneGroupZoneItem>();
            foreach (long zoneId in zoneIds)
            {
                DealSaleZoneGroupZoneItem dealSaleZoneGroupZoneItem = new DealSaleZoneGroupZoneItem() { ZoneId = zoneId };
                dealSaleZoneGroupZoneItems.Add(dealSaleZoneGroupZoneItem);
            }
            return dealSaleZoneGroupZoneItems;
        }

        private void SetRateLocatorContext(IEnumerable<long> zoneIds, out Func<string, int, IEnumerable<SaleRateHistoryRecord>> getCustomerZoneRatesFunc)
        {
            var carrierAccountManager = new CarrierAccountManager();
            int sellingProductId = carrierAccountManager.GetSellingProductId(CarrierAccountId);

            var customerZoneRateHistoryLocator =
               new CustomerZoneRateHistoryLocator(new CustomerZoneRateHistoryReader(new List<int> { CarrierAccountId }
                   , new List<int> { sellingProductId }, zoneIds, true, false));

            getCustomerZoneRatesFunc = (zoneName, countryId) => customerZoneRateHistoryLocator.GetCustomerZoneRateHistory(CarrierAccountId, sellingProductId, zoneName, null, countryId, null, null);
        }
        private IOrderedEnumerable<DealSaleZoneGroupTier> BuildSaleTiers(List<VolCommitmentDealItemTier> volCommitmentDealItemTiers, IEnumerable<long> zoneIds)
        {
            if (volCommitmentDealItemTiers == null || volCommitmentDealItemTiers.Count == 0)
                throw new NullReferenceException("volCommitmentDealItemTiers");

            int tierNumber = 1;
            IOrderedEnumerable<VolCommitmentDealItemTier> orderedVolCommitmentDealItemTiers = volCommitmentDealItemTiers.OrderBy(itm => itm.UpToVolume ?? Int32.MaxValue);

            var dealSaleZoneGroupTiers = new List<DealSaleZoneGroupTier>();
            int previousVolumeAssigned = 0;

            Func<string, int, IEnumerable<SaleRateHistoryRecord>> getCustomerZoneRatesFunc;
            SetRateLocatorContext(zoneIds, out getCustomerZoneRatesFunc);

            var context = new DealSaleRateEvaluatorContext
            {
                GetCustomerZoneRatesFunc = getCustomerZoneRatesFunc,
                DealBED = BeginDate,
                DealEED = EndDate,
                ZoneIds = zoneIds.ToList()
            };

            foreach (VolCommitmentDealItemTier volCommitmentDealItemTier in orderedVolCommitmentDealItemTiers)
            {
                var saleEvaluatedRate = volCommitmentDealItemTier.EvaluatedRate as DealSaleRateEvaluator;

                if (saleEvaluatedRate == null)
                    throw new NullReferenceException("DealSaleRateEvaluator");

                saleEvaluatedRate.EvaluateRate(context);

                DealSaleZoneGroupTier dealSaleZoneGroupTier = new DealSaleZoneGroupTier
                {
                    RetroActiveFromTierNumber = volCommitmentDealItemTier.RetroActiveFromTierNumber,
                    VolumeInSeconds = volCommitmentDealItemTier.UpToVolume.HasValue ? (volCommitmentDealItemTier.UpToVolume.Value * 60) - previousVolumeAssigned : (int?)null,
                    TierNumber = tierNumber,
                    RatesByZoneId = context.SaleRatesByZoneId,
                    CurrencyId = CurrencyId
                };
                dealSaleZoneGroupTiers.Add(dealSaleZoneGroupTier);
                tierNumber++;
                previousVolumeAssigned += dealSaleZoneGroupTier.VolumeInSeconds ?? 0;
            }
            return dealSaleZoneGroupTiers.OrderBy(itm => itm.TierNumber);
        }

        #endregion

        #region Supplier Methods
        private List<DealSupplierZoneGroupZoneItem> BuildSupplierZones(IEnumerable<long> zoneIds)
        {
            if (zoneIds == null || !zoneIds.Any())
                throw new NullReferenceException("zoneIds");

            List<DealSupplierZoneGroupZoneItem> dealSupplierZoneGroupZoneItems = new List<DealSupplierZoneGroupZoneItem>();
            foreach (long zoneId in zoneIds)
            {
                DealSupplierZoneGroupZoneItem dealSupplierZoneGroupZoneItem = new DealSupplierZoneGroupZoneItem() { ZoneId = zoneId };
                dealSupplierZoneGroupZoneItems.Add(dealSupplierZoneGroupZoneItem);
            }
            return dealSupplierZoneGroupZoneItems;
        }

        private IOrderedEnumerable<DealSupplierZoneGroupTier> BuildSupplierTiers(List<VolCommitmentDealItemTier> volCommitmentDealItemTiers, IEnumerable<long> zoneIds)
        {
            if (volCommitmentDealItemTiers == null || volCommitmentDealItemTiers.Count == 0)
                throw new NullReferenceException("volCommitmentDealItemTiers");

            int tierNumber = 1;
            IOrderedEnumerable<VolCommitmentDealItemTier> orderedVolCommitmentDealItemTiers = volCommitmentDealItemTiers.OrderBy(itm => itm.UpToVolume ?? Int32.MaxValue);

            var context = new DealSupplierRateEvaluatorContext
            {
                DealBED = BeginDate,
                DealEED = EndDate,
                ZoneIds = zoneIds.ToList()
            };

            List<DealSupplierZoneGroupTier> dealSupplierZoneGroupTiers = new List<DealSupplierZoneGroupTier>();
            int previousVolumeAssigned = 0;
            foreach (VolCommitmentDealItemTier volCommitmentDealItemTier in orderedVolCommitmentDealItemTiers)
            {
                var supplierEvaluatedRate = volCommitmentDealItemTier.EvaluatedRate as DealSupplierRateEvaluator;

                if (supplierEvaluatedRate == null)
                    throw new NullReferenceException("DealSupplierRateEvaluator");

                supplierEvaluatedRate.EvaluateRate(context);

                DealSupplierZoneGroupTier dealSupplierZoneGroupTier = new DealSupplierZoneGroupTier
                {
                    RetroActiveFromTierNumber = volCommitmentDealItemTier.RetroActiveFromTierNumber,
                    VolumeInSeconds = volCommitmentDealItemTier.UpToVolume.HasValue ? (volCommitmentDealItemTier.UpToVolume.Value * 60) - previousVolumeAssigned : (int?)null,
                    RatesByZoneId = context.SupplierDealRatesByZoneId,
                    TierNumber = tierNumber,
                    CurrencyId = CurrencyId
                };
                dealSupplierZoneGroupTiers.Add(dealSupplierZoneGroupTier);
                tierNumber++;
                previousVolumeAssigned += dealSupplierZoneGroupTier.VolumeInSeconds ?? 0;
            }
            return dealSupplierZoneGroupTiers.OrderBy(itm => itm.TierNumber);
        }

        private List<DealSupplierZoneGroupTierZoneRate> BuildSupplierExceptionRates(IEnumerable<VolCommitmentDealItemTierZoneRate> volCommitmentDealItemTierZoneRates)
        {
            if (volCommitmentDealItemTierZoneRates == null || volCommitmentDealItemTierZoneRates.Count() == 0)
                return null;

            List<DealSupplierZoneGroupTierZoneRate> dealSupplierZoneGroupTierZoneRates = new List<DealSupplierZoneGroupTierZoneRate>();
            foreach (VolCommitmentDealItemTierZoneRate volCommitmentDealItemTierZoneRate in volCommitmentDealItemTierZoneRates)
            {
                if (volCommitmentDealItemTierZoneRate.Zones == null || volCommitmentDealItemTierZoneRate.Zones.Any())
                    throw new NullReferenceException("volCommitmentDealItemTierZoneRate.ZoneIds");

                foreach (var zone in volCommitmentDealItemTierZoneRate.Zones)
                {
                    DealSupplierZoneGroupTierZoneRate dealSupplierZoneGroupTierZoneRate = new DealSupplierZoneGroupTierZoneRate()
                    {
                        ZoneId = zone.ZoneId,
                        // Rate = volCommitmentDealItemTierZoneRate.Rate
                    };
                    dealSupplierZoneGroupTierZoneRates.Add(dealSupplierZoneGroupTierZoneRate);
                }
            }
            return dealSupplierZoneGroupTierZoneRates;
        }

        #endregion
    }
}