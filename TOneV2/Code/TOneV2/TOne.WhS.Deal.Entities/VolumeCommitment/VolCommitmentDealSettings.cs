using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Deal.Entities;

namespace TOne.WhS.Deal.Entities
{
    public enum VolCommitmentDealType
    {
        [Description("Buy")]
        Buy = 0,

        [Description("Sell")]
        Sell = 1
    }
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
                            DealSupplierZoneGroup dealSupplierZoneGroup = new DealSupplierZoneGroup()
                            {
                                DealId = context.DealId,
                                BED = BeginDate,
                                DealSupplierZoneGroupNb = volCommitmentDealItem.ZoneGroupNumber,
                                EED = EndDate,
                                SupplierId = CarrierAccountId,
                                Tiers = BuildSupplierTiers(volCommitmentDealItem.Tiers),
                                Zones = BuildSupplierZones(volCommitmentDealItem.ZoneIds)
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
                            DealSaleZoneGroup dealSaleZoneGroup = new DealSaleZoneGroup()
                            {
                                DealId = context.DealId,
                                BED = BeginDate,
                                DealSaleZoneGroupNb = volCommitmentDealItem.ZoneGroupNumber,
                                EED = EndDate,
                                CustomerId = CarrierAccountId,
                                Tiers = BuildSaleTiers(volCommitmentDealItem.Tiers),
                                Zones = BuildSaleZones(volCommitmentDealItem.ZoneIds)
                            };
                            dealSaleZoneGroups.Add(dealSaleZoneGroup);
                        }
                        context.SaleZoneGroups = dealSaleZoneGroups;
                    }
                    break;
            }
        }

        #region Sale Methods
        private List<DealSaleZoneGroupZoneItem> BuildSaleZones(List<long> zoneIds)
        {
            if (zoneIds == null || zoneIds.Count == 0)
                throw new NullReferenceException("zoneIds");

            List<DealSaleZoneGroupZoneItem> dealSaleZoneGroupZoneItems = new List<DealSaleZoneGroupZoneItem>();
            foreach (long zoneId in zoneIds)
            {
                DealSaleZoneGroupZoneItem dealSaleZoneGroupZoneItem = new DealSaleZoneGroupZoneItem() { ZoneId = zoneId };
                dealSaleZoneGroupZoneItems.Add(dealSaleZoneGroupZoneItem);
            }
            return dealSaleZoneGroupZoneItems;
        }

        private IOrderedEnumerable<DealSaleZoneGroupTier> BuildSaleTiers(List<VolCommitmentDealItemTier> volCommitmentDealItemTiers)
        {
            if (volCommitmentDealItemTiers == null || volCommitmentDealItemTiers.Count == 0)
                throw new NullReferenceException("volCommitmentDealItemTiers");

            int tierNumber = 1;
            IOrderedEnumerable<VolCommitmentDealItemTier> orderedVolCommitmentDealItemTiers = volCommitmentDealItemTiers.OrderBy(itm => itm.UpToVolume.HasValue ? itm.UpToVolume.Value : Int32.MaxValue);

            List<DealSaleZoneGroupTier> dealSaleZoneGroupTiers = new List<DealSaleZoneGroupTier>();
            int previousVolumeAssigned = 0;
            foreach (VolCommitmentDealItemTier volCommitmentDealItemTier in orderedVolCommitmentDealItemTiers)
            {
                DealSaleZoneGroupTier dealSaleZoneGroupTier = new DealSaleZoneGroupTier()
                {
                    RetroActiveFromTierNumber = volCommitmentDealItemTier.RetroActiveFromTierNumber,
                    VolumeInSeconds = volCommitmentDealItemTier.UpToVolume.HasValue ? (volCommitmentDealItemTier.UpToVolume.Value * 60) - previousVolumeAssigned : (int?)null,
                    TierNumber = tierNumber,
                    Rate = volCommitmentDealItemTier.DefaultRate,
                    ExceptionRates = BuildSaleExceptionRates(volCommitmentDealItemTier.ExceptionZoneRates),
                    CurrencyId = CurrencyId
                };
                dealSaleZoneGroupTiers.Add(dealSaleZoneGroupTier);
                tierNumber++;
                previousVolumeAssigned += dealSaleZoneGroupTier.VolumeInSeconds.HasValue ? dealSaleZoneGroupTier.VolumeInSeconds.Value : 0;
            }
            return dealSaleZoneGroupTiers.OrderBy(itm => itm.TierNumber);
        }

        private List<DealSaleZoneGroupTierZoneRate> BuildSaleExceptionRates(IEnumerable<VolCommitmentDealItemTierZoneRate> volCommitmentDealItemTierZoneRates)
        {
            if (volCommitmentDealItemTierZoneRates == null || volCommitmentDealItemTierZoneRates.Count() == 0)
                return null;

            List<DealSaleZoneGroupTierZoneRate> dealSaleZoneGroupTierZoneRates = new List<DealSaleZoneGroupTierZoneRate>();
            foreach (VolCommitmentDealItemTierZoneRate volCommitmentDealItemTierZoneRate in volCommitmentDealItemTierZoneRates)
            {
                if (volCommitmentDealItemTierZoneRate.ZoneIds == null || volCommitmentDealItemTierZoneRate.ZoneIds.Count == 0)
                    throw new NullReferenceException("volCommitmentDealItemTierZoneRate.ZoneIds");

                foreach (long zoneId in volCommitmentDealItemTierZoneRate.ZoneIds)
                {
                    DealSaleZoneGroupTierZoneRate dealSaleZoneGroupTierZoneRate = new DealSaleZoneGroupTierZoneRate()
                    {
                        ZoneId = zoneId,
                        Rate = volCommitmentDealItemTierZoneRate.Rate
                    };
                    dealSaleZoneGroupTierZoneRates.Add(dealSaleZoneGroupTierZoneRate);
                }
            }
            return dealSaleZoneGroupTierZoneRates;
        }

        #endregion

        #region Supplier Methods
        private List<DealSupplierZoneGroupZoneItem> BuildSupplierZones(List<long> zoneIds)
        {
            if (zoneIds == null || zoneIds.Count == 0)
                throw new NullReferenceException("zoneIds");

            List<DealSupplierZoneGroupZoneItem> dealSupplierZoneGroupZoneItems = new List<DealSupplierZoneGroupZoneItem>();
            foreach (long zoneId in zoneIds)
            {
                DealSupplierZoneGroupZoneItem dealSupplierZoneGroupZoneItem = new DealSupplierZoneGroupZoneItem() { ZoneId = zoneId };
                dealSupplierZoneGroupZoneItems.Add(dealSupplierZoneGroupZoneItem);
            }
            return dealSupplierZoneGroupZoneItems;
        }

        private IOrderedEnumerable<DealSupplierZoneGroupTier> BuildSupplierTiers(List<VolCommitmentDealItemTier> volCommitmentDealItemTiers)
        {
            if (volCommitmentDealItemTiers == null || volCommitmentDealItemTiers.Count == 0)
                throw new NullReferenceException("volCommitmentDealItemTiers");

            int tierNumber = 1;
            IOrderedEnumerable<VolCommitmentDealItemTier> orderedVolCommitmentDealItemTiers = volCommitmentDealItemTiers.OrderBy(itm => itm.UpToVolume.HasValue ? itm.UpToVolume.Value : Int32.MaxValue);

            List<DealSupplierZoneGroupTier> dealSupplierZoneGroupTiers = new List<DealSupplierZoneGroupTier>();
            int previousVolumeAssigned = 0;
            foreach (VolCommitmentDealItemTier volCommitmentDealItemTier in orderedVolCommitmentDealItemTiers)
            {
                DealSupplierZoneGroupTier dealSupplierZoneGroupTier = new DealSupplierZoneGroupTier()
                {
                    RetroActiveFromTierNumber = volCommitmentDealItemTier.RetroActiveFromTierNumber,
                    VolumeInSeconds = volCommitmentDealItemTier.UpToVolume.HasValue ? (volCommitmentDealItemTier.UpToVolume.Value * 60) - previousVolumeAssigned : (int?)null,
                    TierNumber = tierNumber,
                    Rate = volCommitmentDealItemTier.DefaultRate,
                    ExceptionRates = BuildSupplierExceptionRates(volCommitmentDealItemTier.ExceptionZoneRates),
                    CurrencyId = CurrencyId
                };
                dealSupplierZoneGroupTiers.Add(dealSupplierZoneGroupTier);
                tierNumber++;
                previousVolumeAssigned += dealSupplierZoneGroupTier.VolumeInSeconds.HasValue ? dealSupplierZoneGroupTier.VolumeInSeconds.Value : 0;
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
                if (volCommitmentDealItemTierZoneRate.ZoneIds == null || volCommitmentDealItemTierZoneRate.ZoneIds.Count == 0)
                    throw new NullReferenceException("volCommitmentDealItemTierZoneRate.ZoneIds");

                foreach (long zoneId in volCommitmentDealItemTierZoneRate.ZoneIds)
                {
                    DealSupplierZoneGroupTierZoneRate dealSupplierZoneGroupTierZoneRate = new DealSupplierZoneGroupTierZoneRate()
                    {
                        ZoneId = zoneId,
                        Rate = volCommitmentDealItemTierZoneRate.Rate
                    };
                    dealSupplierZoneGroupTierZoneRates.Add(dealSupplierZoneGroupTierZoneRate);
                }
            }
            return dealSupplierZoneGroupTierZoneRates;
        }

        #endregion
    }
}