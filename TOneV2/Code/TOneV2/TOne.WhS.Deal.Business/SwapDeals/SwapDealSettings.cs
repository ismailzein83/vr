using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Deal.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;

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

        private DateTime? DealEEDWithGracePeriod
        {
            get { return EndDate.HasValue ? EndDate.Value.AddDays(GracePeriod) : EndDate; }
        }
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
            DealDefinitionManager dealDefinitionManager = new DealDefinitionManager();
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
            var excludedSaleZones = dealDefinitionManager.GetExcludedSaleZones(validateBeforeSaveContext.DealId, this.CarrierAccountId, validateBeforeSaveContext.DealSaleZoneIds, this.BeginDate, this.EndDate);
            var excludedSupplierZones = dealDefinitionManager.GetExcludedSupplierZones(validateBeforeSaveContext.DealId, this.CarrierAccountId, validateBeforeSaveContext.DealSupplierZoneIds, this.BeginDate, this.EndDate);
            if (excludedSaleZones.Count() > 0)
            {
                validationResult = false;
                var excludedSaleZoneNames = saleZoneManager.GetSaleZoneNames(excludedSaleZones);
                validateBeforeSaveContext.ValidateMessages.Add(string.Format("The following sale zone(s) {0} are overlapping with zones in other deals", string.Join(",", excludedSaleZoneNames)));
            }

            if (excludedSupplierZones.Count > 0)
            {
                validationResult = false;
                var excludedSupplierZoneNames = supplierZoneManager.GetSupplierZoneNames(excludedSupplierZones);
                validateBeforeSaveContext.ValidateMessages.Add(string.Format("The following supplier zone(s) {0} are overlapping with zones in other deals", string.Join(",", excludedSupplierZoneNames)));
            }

            if (GracePeriod > (EndDate.Value - BeginDate).Days)
            {
                validationResult = false;
                validateBeforeSaveContext.ValidateMessages.Add("Grace Period should be less than the difference between BED and EED");
            }

            if ((Inbounds != null && Inbounds.Count > 0) && (Outbounds != null && Outbounds.Count > 0))
            {
                ValidateSaleAndCost(validateBeforeSaveContext, ref validationResult);
            }
            var invalidCountryIds = ValidateSwapDealCountries(CarrierAccountId, BeginDate, false);
            if (invalidCountryIds.Count() > 0)
            {
                CountryManager countrymanager = new CountryManager();
                var invalidCountryNames = countrymanager.GetCountryNames(invalidCountryIds.Distinct());
                validationResult = false;
                validateBeforeSaveContext.ValidateMessages.Add(string.Format("The following countries {0} are not sold at {1}", string.Join(",", invalidCountryNames), BeginDate));
            }

            return validationResult;
        }

        public override void GetZoneGroups(IDealGetZoneGroupsContext context)
        {
            DateTime? realEED = GetRealEED();

            if (Status == DealStatus.Draft || (realEED.HasValue && BeginDate == realEED))
                return;

            switch (context.DealZoneGroupPart)
            {
                case DealZoneGroupPart.Both:
                    context.SaleZoneGroups = BuildSaleZoneGroups(context.DealId, context.EvaluateRates, realEED);
                    context.SupplierZoneGroups = BuildSupplierZoneGroups(context.DealId, context.EvaluateRates, realEED);
                    break;

                case DealZoneGroupPart.Sale:
                    context.SaleZoneGroups = BuildSaleZoneGroups(context.DealId, context.EvaluateRates, realEED);
                    break;

                case DealZoneGroupPart.Cost:
                    context.SupplierZoneGroups = BuildSupplierZoneGroups(context.DealId, context.EvaluateRates, realEED);
                    break;

                default: throw new NotSupportedException(string.Format("DealZoneGroupPart {0} not supported.", context.DealZoneGroupPart));
            }
        }

        public override void GetRoutingZoneGroups(IDealGetRoutingZoneGroupsContext context)
        {
            if (Status == DealStatus.Draft)
                return;

            switch (context.DealZoneGroupPart)
            {
                case DealZoneGroupPart.Both:
                    context.SaleZoneGroups = BuildSaleRoutingZoneGroups();
                    context.SupplierZoneGroups = BuildSupplierRoutingZoneGroups();
                    break;

                case DealZoneGroupPart.Sale:
                    context.SaleZoneGroups = BuildSaleRoutingZoneGroups();
                    break;

                case DealZoneGroupPart.Cost:
                    context.SupplierZoneGroups = BuildSupplierRoutingZoneGroups();
                    break;

                default: throw new NotSupportedException(string.Format("DealZoneRoutingGroupPart {0} not supported.", context.DealZoneGroupPart));
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

        public decimal? GetSaleVolume()
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
        public decimal? GetCostVolume()
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

        private List<DealRoutingSupplierZoneGroup> BuildSupplierRoutingZoneGroups()
        {
            List<DealRoutingSupplierZoneGroup> supplierZoneGroups = new List<DealRoutingSupplierZoneGroup>();

            if (Outbounds == null || Outbounds.Count == 0)
                return null;
            foreach (SwapDealOutbound swapDealOutbound in Outbounds)
            {
                supplierZoneGroups.Add(new DealRoutingSupplierZoneGroup
                {
                    Tiers = BuildRoutingSupplierTiers(swapDealOutbound)
                });
            }
            return supplierZoneGroups;
        }

        private List<DealRoutingSaleZoneGroup> BuildSaleRoutingZoneGroups()
        {
            List<DealRoutingSaleZoneGroup> saleZoneGroups = new List<DealRoutingSaleZoneGroup>();

            if (Inbounds == null || Inbounds.Count == 0)
                return null;
            foreach (SwapDealInbound swapDealInbound in Inbounds)
            {
                saleZoneGroups.Add(new DealRoutingSaleZoneGroup
                {
                    Tiers = BuildRoutingSaleTiers(swapDealInbound)
                });
            }

            return saleZoneGroups;
        }

        private void ValidateSaleAndCost(IValidateBeforeSaveContext validateBeforeSaveContext, ref bool validationResult)
        {
            if (DealContract == DealContract.BalancedAmount)
            {
                var saleAmount = GetSaleAmount();
                var costAmount = GetCostAmount();
                if (saleAmount.HasValue && costAmount.HasValue)
                {
                    decimal balancedAmount = saleAmount > costAmount
                        ? Math.Abs((saleAmount.Value - costAmount.Value) / saleAmount.Value) * 100
                        : Math.Abs((saleAmount.Value - costAmount.Value) / costAmount.Value) * 100;
                    if (balancedAmount > Difference.Value)
                    {
                        validationResult = false;
                        validateBeforeSaveContext.ValidateMessages.Add("Amounts should be balanced");
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
                    if (balancedVolume > Difference.Value)
                    {
                        validationResult = false;
                        validateBeforeSaveContext.ValidateMessages.Add("Durations should be balanced");
                    }
                }

            }
        }

        private DateTime? GetRealEED()
        {
            DateTime? EED = EndDate;
            if (Status == DealStatus.Inactive)
                EED = DeActivationDate;
            return EED < DealEEDWithGracePeriod ? EED : DealEEDWithGracePeriod;
        }
        private List<int> ValidateSwapDealCountries(int customerId, DateTime? effectiveOn, bool isEffectiveInFuture)
        {
            List<int> invalidCountries = new List<int>();
            CustomerCountryManager customerCountryManager = new CustomerCountryManager();
            var customerCountries = customerCountryManager.GetCustomerCountryIds(customerId, effectiveOn, isEffectiveInFuture);
            foreach (var inbound in Inbounds)
            {
                if (!customerCountries.Contains(inbound.CountryId))
                {
                    invalidCountries.Add(inbound.CountryId);
                }
            }
            return invalidCountries;
        }
        private List<BaseDealSaleZoneGroup> BuildSaleZoneGroups(int dealId, bool evaluateRates, DateTime? dealEED)
        {
            if (Inbounds == null || Inbounds.Count == 0)
                return null;

            List<BaseDealSaleZoneGroup> saleZoneGroups = new List<BaseDealSaleZoneGroup>();
            foreach (SwapDealInbound swapDealInbound in Inbounds)
            {
                List<DealSaleZoneGroupZoneItem> zones = Helper.BuildSaleZones(swapDealInbound.SaleZones.Select(z => z.ZoneId), BeginDate, dealEED);

                BaseDealSaleZoneGroup dealSaleZoneGroup;
                if (evaluateRates)
                    dealSaleZoneGroup = new DealSaleZoneGroup { Tiers = BuildSaleTiers(swapDealInbound, zones) };
                else
                    dealSaleZoneGroup = new DealSaleZoneGroupWithoutRate { Tiers = BuildSaleTiersWithoutRate(swapDealInbound) };

                dealSaleZoneGroup.DealId = dealId;
                dealSaleZoneGroup.DealSaleZoneGroupNb = swapDealInbound.ZoneGroupNumber;
                dealSaleZoneGroup.CustomerId = CarrierAccountId;
                dealSaleZoneGroup.Zones = zones;
                dealSaleZoneGroup.BED = BeginDate;
                dealSaleZoneGroup.EED = dealEED;

                saleZoneGroups.Add(dealSaleZoneGroup);
            }
            return saleZoneGroups;
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

        private IOrderedEnumerable<DealSaleZoneGroupTier> BuildSaleTiers(SwapDealInbound swapDealInbound, List<DealSaleZoneGroupZoneItem> zones)
        {
            Dictionary<long, List<DealRate>> saleRatesByZoneId = GetDealSaleRatesByZoneId(swapDealInbound.Rate, zones);
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
                Dictionary<long, List<DealRate>> extraRateByZoneId = GetDealSaleRatesByZoneId(swapDealInbound.ExtraVolumeRate.Value, zones);
                dealSaleZoneGroupTiers.Add(new DealSaleZoneGroupTier
                {
                    RatesByZoneId = extraRateByZoneId,
                    TierNumber = 2
                });
            }
            return dealSaleZoneGroupTiers.OrderBy(itm => itm.TierNumber);
        }
        private Dictionary<long, List<DealRate>> GetDealSaleRatesByZoneId(decimal rate, List<DealSaleZoneGroupZoneItem> zones)
        {
            var dealeRateByZoneId = new Dictionary<long, List<DealRate>>();
            foreach (var zone in zones)
            {
                List<DealRate> dealRates = dealeRateByZoneId.GetOrCreateItem(zone.ZoneId);
                DealRate dealRate = new DealRate
                {
                    ZoneId = zone.ZoneId,
                    BED = BeginDate,
                    EED = zone.EED,
                    Rate = rate,
                    CurrencyId = CurrencyId
                };
                dealRates.Add(dealRate);
            }
            return dealeRateByZoneId;
        }
        private List<BaseDealSupplierZoneGroup> BuildSupplierZoneGroups(int dealId, bool evaluateRates, DateTime? dealEED)
        {
            if (Outbounds == null || Outbounds.Count == 0)
                return null;

            var dealSupplierZoneGroups = new List<BaseDealSupplierZoneGroup>();
            foreach (SwapDealOutbound swapDealOutbound in Outbounds)
            {
                List<DealSupplierZoneGroupZoneItem> zones = Helper.BuildSupplierZones(swapDealOutbound.SupplierZones.Select(z => z.ZoneId), BeginDate, dealEED);
                BaseDealSupplierZoneGroup dealSupplierZoneGroup;
                if (evaluateRates)
                    dealSupplierZoneGroup = new DealSupplierZoneGroup { Tiers = BuildSupplierTiers(swapDealOutbound, zones) };
                else
                    dealSupplierZoneGroup = new DealSupplierZoneGroupWithoutRate { Tiers = BuildSupplierTiersWithoutRate(swapDealOutbound) };

                dealSupplierZoneGroup.DealId = dealId;
                dealSupplierZoneGroup.DealSupplierZoneGroupNb = swapDealOutbound.ZoneGroupNumber;
                dealSupplierZoneGroup.SupplierId = CarrierAccountId;
                dealSupplierZoneGroup.Zones = zones;
                dealSupplierZoneGroup.BED = BeginDate;
                dealSupplierZoneGroup.EED = dealEED;

                dealSupplierZoneGroups.Add(dealSupplierZoneGroup);
            }
            return dealSupplierZoneGroups;
        }

        private IOrderedEnumerable<DealSupplierZoneGroupTier> BuildSupplierTiers(SwapDealOutbound swapDealOutbound, List<DealSupplierZoneGroupZoneItem> zones)
        {
            Dictionary<long, List<DealRate>> supplierDealRatesByZoneId = GetDealSupplierRatesByZoneId(swapDealOutbound.Rate, zones);

            DealSupplierZoneGroupTier dealSupplierZoneGroupTier = new DealSupplierZoneGroupTier
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
                var extraRateByZoneId = GetDealSupplierRatesByZoneId(swapDealOutbound.ExtraVolumeRate.Value, zones);
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

        private List<DealRoutingSupplierZoneGroupTier> BuildRoutingSupplierTiers(SwapDealOutbound swapDealOutbound)
        {
            Decimal? substituteRate = null;
            Decimal? substituteExtraRate = null;

            switch (swapDealOutbound.SubstituteRateType)
            {
                case SubstituteRateType.DealRate:
                    substituteRate = swapDealOutbound.Rate;
                    substituteExtraRate = swapDealOutbound.ExtraVolumeRate;
                    break;
                case SubstituteRateType.FixedRate:
                    substituteRate = swapDealOutbound.FixedRate;
                    substituteExtraRate = swapDealOutbound.FixedRate;
                    break;
                case SubstituteRateType.NormalRate:
                    substituteRate = null;
                    substituteExtraRate = null;
                    break;
            }

            DealRoutingSupplierZoneGroupTier dealRoutingSupplierZoneGroupTier = new DealRoutingSupplierZoneGroupTier
            {
                TierNumber = 1,
                SubstituteRate = substituteRate
            };

            var dealSupplierTiers = new List<DealRoutingSupplierZoneGroupTier> { dealRoutingSupplierZoneGroupTier };

            if (swapDealOutbound.ExtraVolumeRate.HasValue)
                dealSupplierTiers.Add(new DealRoutingSupplierZoneGroupTier
                {
                    TierNumber = 2,
                    SubstituteRate = substituteExtraRate
                });

            return dealSupplierTiers;
        }

        private List<DealRoutingSaleZoneGroupTier> BuildRoutingSaleTiers(SwapDealInbound swapDealInbound)
        {
            Decimal? substituteRate = null;
            Decimal? substituteExtraRate = null;

            switch (swapDealInbound.SubstituteRateType)
            {
                case SubstituteRateType.DealRate:
                    substituteRate = swapDealInbound.Rate;
                    substituteExtraRate = swapDealInbound.ExtraVolumeRate;
                    break;
                case SubstituteRateType.FixedRate:
                    substituteRate = swapDealInbound.FixedRate;
                    substituteExtraRate = swapDealInbound.FixedRate;
                    break;
                case SubstituteRateType.NormalRate:
                    substituteRate = null;
                    substituteExtraRate = null;
                    break;
            }

            DealRoutingSaleZoneGroupTier dealRoutingSaleZoneGroupTier = new DealRoutingSaleZoneGroupTier
            {
                TierNumber = 1,
                SubstituteRate = substituteRate
            };

            var dealSaleTiers = new List<DealRoutingSaleZoneGroupTier> { dealRoutingSaleZoneGroupTier };

            if (swapDealInbound.ExtraVolumeRate.HasValue)
                dealSaleTiers.Add(new DealRoutingSaleZoneGroupTier
                {
                    TierNumber = 2,
                    SubstituteRate = substituteExtraRate
                });

            return dealSaleTiers;
        }

        private Dictionary<long, List<DealRate>> GetDealSupplierRatesByZoneId(decimal rate, List<DealSupplierZoneGroupZoneItem> zones)
        {
            var dealeRateByZoneId = new Dictionary<long, List<DealRate>>();
            foreach (var zone in zones)
            {
                List<DealRate> dealRates = dealeRateByZoneId.GetOrCreateItem(zone.ZoneId);
                DealRate dealRate = new DealRate
                {
                    ZoneId = zone.ZoneId,
                    BED = BeginDate,
                    EED = zone.EED,
                    Rate = rate,
                    CurrencyId = CurrencyId
                };
                dealRates.Add(dealRate);
            }
            return dealeRateByZoneId;
        }
        #endregion
    }
}