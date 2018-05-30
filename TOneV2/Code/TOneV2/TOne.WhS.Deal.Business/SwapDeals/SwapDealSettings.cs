using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Deal.Entities;
using Vanrise.Common;

namespace TOne.WhS.Deal.Business
{
    public enum DealContract
    {
        [Description("Balanced Amount")]
        BalancedAmount = 0,

        [Description("Balanced Duration")]
        BalancedDuration = 1,

        [Description("UnBalanced")]
        UnBalanced = 2
    }

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


        #region Public Methods

        public override int GetCarrierAccountId()
        {
            return CarrierAccountId;
        }

        public override string GetSaleZoneGroupName(int dealGroupNumber)
        {
            var inbound = Inbounds.FindRecord(x => x.ZoneGroupNumber == dealGroupNumber);
            inbound.ThrowIfNull("inbound", dealGroupNumber);
            return inbound.Name;
        }

        public override string GetSupplierZoneGroupName(int dealGroupNumber)
        {
            var outbound = Outbounds.FindRecord(x => x.ZoneGroupNumber == dealGroupNumber);
            outbound.ThrowIfNull("outbound", dealGroupNumber);
            return outbound.Name;
        }

        public override bool ValidateDataBeforeSave(IValidateBeforeSaveContext validateBeforeSaveContext)
        {
            SwapDealManager swapDealManager = new SwapDealManager();
            SaleZoneManager saleZoneManager = new SaleZoneManager();
            SupplierZoneManager supplierZoneManager = new SupplierZoneManager();
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
            var excludedSaleZones = swapDealManager.GetExcludedSaleZones(validateBeforeSaveContext);
            var excludedSupplierZones = swapDealManager.GetExcludedSupplierZones(validateBeforeSaveContext);
            if (excludedSaleZones.Count() > 0 || excludedSupplierZones.Count > 0)
            {
                var excludedSaleZoneNames = saleZoneManager.GetSaleZoneNames(excludedSaleZones);
                var excludedSupplierZoneNames = supplierZoneManager.GetSupplierZoneNames(excludedSupplierZones);
                validationResult = false;
                validateBeforeSaveContext.ValidateMessages.Add(string.Format("The following sale zone(s) {0} are overlapping with zones in other deals", string.Join(",", excludedSaleZoneNames)));
                validateBeforeSaveContext.ValidateMessages.Add(string.Format("The following supplier zone(s) {0} are overlapping with zones in other deals", string.Join(",", excludedSupplierZoneNames)));
            }


            if (GracePeriod > (EndDate.Value - BeginDate).Days)
            {
                validationResult = false;
                validateBeforeSaveContext.ValidateMessages.Add("Grace Period should be less than the difference between BED and EED");
            }

            if ((Inbounds != null && Inbounds.Count > 0) && (Outbounds != null && Outbounds.Count > 0))
            {
                validationResult = ValidateSaleAndCost(validateBeforeSaveContext);
            }

            return validationResult;
        }

        public override void GetZoneGroups(IDealGetZoneGroupsContext context)
        {
            if (Status == DealStatus.Draft || BeginDate == EndDate)
                return;

            switch (context.DealZoneGroupPart)
            {
                case DealZoneGroupPart.Both:
                    context.SaleZoneGroups = BuildSaleZoneGroups(context.DealId, context.EvaluateRates);
                    context.SupplierZoneGroups = BuildSupplierZoneGroups(context.DealId, context.EvaluateRates);
                    break;

                case DealZoneGroupPart.Sale:
                    context.SaleZoneGroups = BuildSaleZoneGroups(context.DealId, context.EvaluateRates);
                    break;

                case DealZoneGroupPart.Cost:
                    context.SupplierZoneGroups = BuildSupplierZoneGroups(context.DealId, context.EvaluateRates);
                    break;

                default: throw new NotSupportedException(string.Format("DealZoneGroupPart {0} not supported.", context.DealZoneGroupPart));
            }
        }

        public override List<long> GetDealSaleZoneIds()
        {
            List<long> zoneIds = new List<long>();
            foreach (var inbound in Inbounds)
            {
                var saleZones = inbound.SaleZones;
                foreach (var saleZone in saleZones)
                {
                    zoneIds.Add(saleZone.ZoneId);
                }
            }
            return zoneIds;
        }
        public override List<long> GetDealSupplierZoneIds()
        {
            List<long> zoneIds = new List<long>();
            foreach (var outbound in Outbounds)
            {
                var supplierZones = outbound.SupplierZones;
                foreach (var supplierZone in supplierZones)
                {
                    zoneIds.Add(supplierZone.ZoneId);
                }
            }
            return zoneIds;
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

        #endregion

        #region Private Methods

        private bool ValidateSaleAndCost(IValidateBeforeSaveContext validateBeforeSaveContext)
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


        private List<BaseDealSaleZoneGroup> BuildSaleZoneGroups(int dealId, bool evaluateRates)
        {
            if (Inbounds == null || Inbounds.Count == 0)
                return null;

            List<BaseDealSaleZoneGroup> saleZoneGroups = new List<BaseDealSaleZoneGroup>();
            foreach (SwapDealInbound swapDealInbound in Inbounds)
            {
                BaseDealSaleZoneGroup dealSaleZoneGroup;
                if (evaluateRates)
                    dealSaleZoneGroup = new DealSaleZoneGroup { Tiers = BuildSaleTiers(swapDealInbound) };
                else
                    dealSaleZoneGroup = new DealSaleZoneGroupWithoutRate { Tiers = BuildSaleTiersWithoutRate(swapDealInbound) };


                dealSaleZoneGroup.DealId = dealId;
                dealSaleZoneGroup.DealSaleZoneGroupNb = swapDealInbound.ZoneGroupNumber;
                dealSaleZoneGroup.CustomerId = CarrierAccountId;
                dealSaleZoneGroup.Zones = BuildSaleZones(swapDealInbound.SaleZones.Select(z => z.ZoneId));
                dealSaleZoneGroup.Status = base.Status;
                dealSaleZoneGroup.DeActivationDate = base.DeActivationDate;
                dealSaleZoneGroup.BED = BeginDate;
                dealSaleZoneGroup.EED = EndDate;

                saleZoneGroups.Add(dealSaleZoneGroup);
            }
            return saleZoneGroups;
        }

        private List<DealSaleZoneGroupZoneItem> BuildSaleZones(IEnumerable<long> saleZoneIds)
        {
            if (saleZoneIds == null || !saleZoneIds.Any())
                throw new NullReferenceException("saleZoneIds");

            return saleZoneIds.Select(zoneId => new DealSaleZoneGroupZoneItem
            {
                ZoneId = zoneId,
                BED = BeginDate,
                EED = EndDate

            }).ToList();
        }

        private IOrderedEnumerable<DealSaleZoneGroupTierWithoutRate> BuildSaleTiersWithoutRate(SwapDealInbound swapDealInbound)
        {
            DealSaleZoneGroupTierWithoutRate dealSaleZoneGroupTier = new DealSaleZoneGroupTierWithoutRate
            {
                RetroActiveFromTierNumber = null,
                TierNumber = 1,
                VolumeInSeconds = swapDealInbound.Volume * 60
            };

            var dealSaleTiers = new List<DealSaleZoneGroupTierWithoutRate> { dealSaleZoneGroupTier };

            if (swapDealInbound.ExtraVolumeRate.HasValue)
                dealSaleTiers.Add(new DealSaleZoneGroupTierWithoutRate
                {
                    TierNumber = 2
                });

            return dealSaleTiers.OrderBy(itm => itm.TierNumber);
        }

        private IOrderedEnumerable<DealSaleZoneGroupTier> BuildSaleTiers(SwapDealInbound swapDealInbound)
        {
            var zoneIds = swapDealInbound.SaleZones.Select(z => z.ZoneId);

            Dictionary<long, List<DealRate>> saleRatesByZoneId = GetDealRatesByZoneId(swapDealInbound.Rate, zoneIds);
            DealSaleZoneGroupTier dealSaleZoneGroupTier = new DealSaleZoneGroupTier
            {
                RatesByZoneId = saleRatesByZoneId,
                RetroActiveFromTierNumber = null,
                TierNumber = 1,
                VolumeInSeconds = swapDealInbound.Volume * 60
            };
            List<DealSaleZoneGroupTier> dealSaleZoneGroupTiers = new List<DealSaleZoneGroupTier> { dealSaleZoneGroupTier };

            if (swapDealInbound.ExtraVolumeRate.HasValue)
            {
                Dictionary<long, List<DealRate>> extraRateByZoneId = GetDealRatesByZoneId(swapDealInbound.ExtraVolumeRate.Value, zoneIds);
                dealSaleZoneGroupTiers.Add(new DealSaleZoneGroupTier
                {
                    RatesByZoneId = extraRateByZoneId,
                    TierNumber = 2
                });
            }

            return dealSaleZoneGroupTiers.OrderBy(itm => itm.TierNumber);
        }

        private List<BaseDealSupplierZoneGroup> BuildSupplierZoneGroups(int dealId, bool evaluateRates)
        {
            if (Outbounds == null || Outbounds.Count == 0)
                return null;

            var dealSupplierZoneGroups = new List<BaseDealSupplierZoneGroup>();
            foreach (SwapDealOutbound swapDealOutbound in Outbounds)
            {
                BaseDealSupplierZoneGroup dealSupplierZoneGroup;
                if (evaluateRates)
                    dealSupplierZoneGroup = new DealSupplierZoneGroup() { Tiers = BuildSupplierTiers(swapDealOutbound) };
                else
                    dealSupplierZoneGroup = new DealSupplierZoneGroupWithoutRate() { Tiers = BuildSupplierTiersWithoutRate(swapDealOutbound) };

                dealSupplierZoneGroup.DealId = dealId;
                dealSupplierZoneGroup.DealSupplierZoneGroupNb = swapDealOutbound.ZoneGroupNumber;
                dealSupplierZoneGroup.SupplierId = CarrierAccountId;
                dealSupplierZoneGroup.Zones = BuildSupplierZones(swapDealOutbound.SupplierZones.Select(z => z.ZoneId));
                dealSupplierZoneGroup.Status = base.Status;
                dealSupplierZoneGroup.DeActivationDate = base.DeActivationDate;
                dealSupplierZoneGroup.BED = BeginDate;
                dealSupplierZoneGroup.EED = EndDate;

                dealSupplierZoneGroups.Add(dealSupplierZoneGroup);
            }
            return dealSupplierZoneGroups;
        }

        private List<DealSupplierZoneGroupZoneItem> BuildSupplierZones(IEnumerable<long> supplierZoneIds)
        {
            if (supplierZoneIds == null || !supplierZoneIds.Any())
                throw new NullReferenceException("supplierZoneIds");

            return supplierZoneIds.Select(zoneId => new DealSupplierZoneGroupZoneItem
            {
                ZoneId = zoneId,
                BED = BeginDate,
                EED = EndDate

            }).ToList();
        }

        private IOrderedEnumerable<DealSupplierZoneGroupTier> BuildSupplierTiers(SwapDealOutbound swapDealOutbound)
        {
            IEnumerable<long> supplierZoneIds = swapDealOutbound.SupplierZones.Select(z => z.ZoneId);
            Dictionary<long, List<DealRate>> supplierDealRatesByZoneId = GetDealRatesByZoneId(swapDealOutbound.Rate, supplierZoneIds);

            DealSupplierZoneGroupTier dealSupplierZoneGroupTier = new DealSupplierZoneGroupTier()
            {
                RatesByZoneId = supplierDealRatesByZoneId,
                RetroActiveFromTierNumber = null,
                TierNumber = 1,
                VolumeInSeconds = swapDealOutbound.Volume * 60
            };
            List<DealSupplierZoneGroupTier> dealSupplierZoneGroupTiers = new List<DealSupplierZoneGroupTier>
            {
                dealSupplierZoneGroupTier
            };

            if (swapDealOutbound.ExtraVolumeRate.HasValue)
            {
                var extraRateByZoneId = GetDealRatesByZoneId(swapDealOutbound.ExtraVolumeRate.Value, supplierZoneIds);
                dealSupplierZoneGroupTiers.Add(new DealSupplierZoneGroupTier
                {
                    RatesByZoneId = extraRateByZoneId,
                    TierNumber = 2
                });
            }

            return dealSupplierZoneGroupTiers.OrderBy(itm => itm.TierNumber);
        }

        private IOrderedEnumerable<DealSupplierZoneGroupTierWithoutRate> BuildSupplierTiersWithoutRate(SwapDealOutbound swapDealOutbound)
        {
            DealSupplierZoneGroupTierWithoutRate dealSupplierZoneGroupTierWithoutRate = new DealSupplierZoneGroupTierWithoutRate
            {
                RetroActiveFromTierNumber = null,
                TierNumber = 1,
                VolumeInSeconds = swapDealOutbound.Volume * 60
            };

            var dealSupplierTiers = new List<DealSupplierZoneGroupTierWithoutRate> { dealSupplierZoneGroupTierWithoutRate };

            if (swapDealOutbound.ExtraVolumeRate.HasValue)
                dealSupplierTiers.Add(new DealSupplierZoneGroupTierWithoutRate
                {
                    TierNumber = 2
                });

            return dealSupplierTiers.OrderBy(itm => itm.TierNumber);
        }

        private Dictionary<long, List<DealRate>> GetDealRatesByZoneId(decimal rate, IEnumerable<long> zoneIds)
        {
            DateTime? EED = EndDate;
            if (Status == DealStatus.Inactive)
                EED = DeActivationDate;

            var dealeRateByZoneId = new Dictionary<long, List<DealRate>>();
            foreach (var zoneId in zoneIds)
            {
                List<DealRate> dealRates = dealeRateByZoneId.GetOrCreateItem(zoneId);
                dealRates.Add(new DealRate
                {
                    ZoneId = zoneId,
                    BED = BeginDate,
                    EED = EED,
                    Rate = rate,
                    CurrencyId = CurrencyId
                });
            }
            return dealeRateByZoneId;
        }

        #endregion
    }
}