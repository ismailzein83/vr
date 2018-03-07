using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Deal.Entities;
using Vanrise.Common;

namespace TOne.WhS.Deal.Entities
{
    public enum DealContract
    {
        [Description("Balanced Amount")]
        BalancedAmount = 0,

        [Description("Balanced Duration")]
        BalancedDuration = 1,

        [Description("UnBalanced")]
        UnBalanced = 2
    };

    public enum DealType
    {
        [Description("Gentelmen")]
        Gentelmen = 0,

        [Description("Commitment")]
        Commitment = 1
    }

    public class SwapDealSettings : DealSettings
    {
        public static Guid SwapDealSettingsConfigId = new Guid("63C1310D-FDEA-4AC7-BDE1-58FD11E4EC65");
        public override Guid ConfigId { get { return SwapDealSettingsConfigId; } }

        public int CarrierAccountId { get; set; }

        public DealContract DealContract { get; set; }

        public DealType DealType { get; set; }

        public List<SwapDealInbound> Inbounds { get; set; }

        public int LastInboundGroupNumber { get; set; }

        public int? Difference { set; get; }

        public List<SwapDealOutbound> Outbounds { get; set; }

        public int LastOutboundGroupNumber { get; set; }

        public int GracePeriod { get; set; }

        public int CurrencyId { get; set; }

        public override bool ValidateDataBeforeSave(IValidateBeforeSaveContext validateBeforeSaveContext)
        {
            validateBeforeSaveContext.ValidateMessages = new List<string>();
            bool validationResult = true;
            if (validateBeforeSaveContext.IsEditMode)
            {
                if (validateBeforeSaveContext.ExistingDeal == null)
                    throw new NullReferenceException("validateBeforeSaveContext.ExistingDeal");
                if (validateBeforeSaveContext.ExistingDeal.Settings == null)
                    throw new NullReferenceException("validateBeforeSaveContext.ExistingDeal.Settings");
                var swapSettings = validateBeforeSaveContext.ExistingDeal.Settings.CastWithValidate<SwapDealSettings>("deal.Settings");
                if (swapSettings != null)
                {
                    if (DealType != swapSettings.DealType)
                    {
                        validateBeforeSaveContext.ValidateMessages.Add("Cannot change Deal Type ");
                        validationResult = false;
                    }
                    if (CarrierAccountId != swapSettings.CarrierAccountId)
                    {
                        validateBeforeSaveContext.ValidateMessages.Add("Cannot change Carrier");
                        validationResult = false;
                    }


                    if (DealType == DealType.Commitment)
                    {
                        validateBeforeSaveContext.ValidateMessages.Add("You Cannot update a deal of type commitment");
                        validationResult = false;
                    }
                }

            }
            if (GracePeriod > (EndDate.Value - BeginDate).Days)
            {
                validationResult = false;
                validateBeforeSaveContext.ValidateMessages.Add("Grace Period should be less than the difference between BED and EED");
            }

            if ((Inbounds != null && Inbounds.Count>0) && (Outbounds != null && Outbounds.Count > 0))
            {
                validationResult = ValidateSaleAndCost(validateBeforeSaveContext);
            }

            return validationResult;
        }
        public bool ValidateSaleAndCost(IValidateBeforeSaveContext validateBeforeSaveContext)
        {
            bool validationResult = true;
            if (DealContract == DealContract.BalancedAmount)
            {
                var saleAmount = GetSaleAmount();
                var costAmount = GetCostAmount();
                if (saleAmount.HasValue && costAmount.HasValue)
                {
                    decimal balancedAmount = saleAmount > costAmount
                        ? Math.Abs((saleAmount.Value - costAmount.Value) / saleAmount.Value) * 100
                        : Math.Abs((saleAmount.Value - costAmount.Value) / costAmount.Value) * 100;
                    if (balancedAmount < Difference.Value)
                    {
                        validationResult = false;
                        validateBeforeSaveContext.ValidateMessages.Add("Balanced Amount is less than expected");
                    }
                }
            }
            else if (DealContract == DealContract.BalancedDuration)
            {
                var saleVolume = GetSaleVolume();
                var costVolume = GetCostVolume();
                if (saleVolume.HasValue && costVolume.HasValue)
                {
                    decimal balancedVolume = saleVolume > costVolume
                        ? Math.Abs((saleVolume.Value - costVolume.Value) / saleVolume.Value) * 100
                        : Math.Abs((saleVolume.Value - costVolume.Value) / costVolume.Value) * 100;
                    if (balancedVolume < Difference.Value)
                    {
                        validationResult = false;
                        validateBeforeSaveContext.ValidateMessages.Add("Balanced Volume is less than expected");
                    }
                }

            }
            return validationResult;
        }
        public override void GetZoneGroups(IDealGetZoneGroupsContext context)
        {
            context.SaleZoneGroups = BuildSaleZoneGroups(context.DealId);
            context.SupplierZoneGroups = BuildSupplierZoneGroups(context.DealId);
        }
        //TODO: ASA Check if inbounds can have null value and throw exception if not
        public decimal? GetSaleAmount()
        {
            if (Inbounds == null || Inbounds.Count == 0)
                return null;
            decimal saleAmount = 0;
            foreach (var inbound in Inbounds)
            {
                saleAmount += inbound.Volume * inbound.Rate;
            }
            return saleAmount;
        }
        public decimal? GetCostAmount()
        {
            if (Outbounds == null || Outbounds.Count == 0)
                return null;
            decimal costAmount = 0;
            foreach (var outbound in Outbounds)
            {
                costAmount += outbound.Volume * outbound.Rate;
            }
            return costAmount;
        }
        public int? GetSaleVolume()
        {
            if (Inbounds == null || Inbounds.Count == 0)
                return null;
            int saleVolume = 0;
            foreach (var inbound in Inbounds)
            {
                saleVolume += inbound.Volume;
            }
            return saleVolume;
        }
        public int? GetCostVolume()
        {
            if (Outbounds == null || Outbounds.Count == 0)
                return null;
            int costVolume = 0;
            foreach (var outbound in Outbounds)
            {
                costVolume += outbound.Volume;
            }
            return costVolume;
        }
        private List<DealSaleZoneGroup> BuildSaleZoneGroups(int dealId)
        {
            if (Inbounds == null || Inbounds.Count == 0)
                return null;

            List<DealSaleZoneGroup> saleZoneGroups = new List<DealSaleZoneGroup>();
            foreach (SwapDealInbound swapDealInbound in Inbounds)
            {
                DealSaleZoneGroup dealSaleZoneGroup = new DealSaleZoneGroup()
                {
                    DealId = dealId,
                    BED = BeginDate,
                    CustomerId = CarrierAccountId,
                    DealSaleZoneGroupNb = swapDealInbound.ZoneGroupNumber,
                    EED = EndDate,
                    Tiers = BuildSaleTiers(swapDealInbound),
                    Zones = BuildSaleZones(swapDealInbound.SaleZoneIds)
                };
                saleZoneGroups.Add(dealSaleZoneGroup);
            }
            return saleZoneGroups;
        }

        private List<DealSaleZoneGroupZoneItem> BuildSaleZones(List<long> saleZoneIds)
        {
            if (saleZoneIds == null || saleZoneIds.Count == 0)
                throw new NullReferenceException("saleZoneIds");

            List<DealSaleZoneGroupZoneItem> dealSaleZoneGroupZoneItems = new List<DealSaleZoneGroupZoneItem>();
            foreach (long zoneId in saleZoneIds)
            {
                DealSaleZoneGroupZoneItem dealSaleZoneGroupZoneItem = new DealSaleZoneGroupZoneItem() { ZoneId = zoneId };
                dealSaleZoneGroupZoneItems.Add(dealSaleZoneGroupZoneItem);
            }
            return dealSaleZoneGroupZoneItems;
        }

        private IOrderedEnumerable<DealSaleZoneGroupTier> BuildSaleTiers(SwapDealInbound swapDealInbound)
        {
            DealSaleZoneGroupTier dealSaleZoneGroupTier = new DealSaleZoneGroupTier()
            {
                ExceptionRates = null,
                Rate = swapDealInbound.Rate,
                RetroActiveFromTierNumber = null,
                TierNumber = 1,
                VolumeInSeconds = swapDealInbound.Volume * 60,
                CurrencyId = CurrencyId
            };
            return new List<DealSaleZoneGroupTier>() { dealSaleZoneGroupTier }.OrderBy(itm => itm.TierNumber);
        }

        private List<DealSupplierZoneGroup> BuildSupplierZoneGroups(int dealId)
        {
            if (Outbounds == null || Outbounds.Count == 0)
                return null;

            List<DealSupplierZoneGroup> dealSupplierZoneGroups = new List<DealSupplierZoneGroup>();
            foreach (SwapDealOutbound swapDealOutbound in Outbounds)
            {
                DealSupplierZoneGroup dealSupplierZoneGroup = new DealSupplierZoneGroup()
                {
                    DealId = dealId,
                    BED = BeginDate,
                    SupplierId = CarrierAccountId,
                    DealSupplierZoneGroupNb = swapDealOutbound.ZoneGroupNumber,
                    EED = EndDate,
                    Tiers = BuildSupplierTiers(swapDealOutbound),
                    Zones = BuildSupplierZones(swapDealOutbound.SupplierZoneIds)
                };
                dealSupplierZoneGroups.Add(dealSupplierZoneGroup);
            }
            return dealSupplierZoneGroups;
        }

        private List<DealSupplierZoneGroupZoneItem> BuildSupplierZones(List<long> supplierZoneIds)
        {
            if (supplierZoneIds == null || supplierZoneIds.Count == 0)
                throw new NullReferenceException("supplierZoneIds");

            List<DealSupplierZoneGroupZoneItem> dealSupplierZoneGroupZoneItems = new List<DealSupplierZoneGroupZoneItem>();
            foreach (long zoneId in supplierZoneIds)
            {
                DealSupplierZoneGroupZoneItem dealSupplierZoneGroupZoneItem = new DealSupplierZoneGroupZoneItem() { ZoneId = zoneId };
                dealSupplierZoneGroupZoneItems.Add(dealSupplierZoneGroupZoneItem);
            }
            return dealSupplierZoneGroupZoneItems;
        }

        private IOrderedEnumerable<DealSupplierZoneGroupTier> BuildSupplierTiers(SwapDealOutbound swapDealOutbound)
        {
            DealSupplierZoneGroupTier dealSupplierZoneGroupTier = new DealSupplierZoneGroupTier()
            {
                ExceptionRates = null,
                Rate = swapDealOutbound.Rate,
                RetroActiveFromTierNumber = null,
                TierNumber = 1,
                VolumeInSeconds = swapDealOutbound.Volume * 60,
                CurrencyId = CurrencyId
            };
            return new List<DealSupplierZoneGroupTier>() { dealSupplierZoneGroupTier }.OrderBy(itm => itm.TierNumber);
        }
    }
}