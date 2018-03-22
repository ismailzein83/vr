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

            if ((Inbounds != null && Inbounds.Count > 0) && (Outbounds != null && Outbounds.Count > 0))
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

            Func<string, int, IEnumerable<SaleRateHistoryRecord>> getCustomerZoneRatesFunc;
            SetRateLocatorContext(Inbounds.SelectMany(inb => inb.SaleZones.Select(z=>z.ZoneId)), out getCustomerZoneRatesFunc);

            List<DealSaleZoneGroup> saleZoneGroups = new List<DealSaleZoneGroup>();
            foreach (SwapDealInbound swapDealInbound in Inbounds)
            {
                DealSaleZoneGroup dealSaleZoneGroup = new DealSaleZoneGroup
                {
                    DealId = dealId,
                    BED = BeginDate,
                    CustomerId = CarrierAccountId,
                    DealSaleZoneGroupNb = swapDealInbound.ZoneGroupNumber,
                    EED = EndDate,
                    Tiers = BuildSaleTiers(swapDealInbound, getCustomerZoneRatesFunc),
                    Zones = BuildSaleZones(swapDealInbound.SaleZones.Select(z => z.ZoneId))
                };
                saleZoneGroups.Add(dealSaleZoneGroup);
            }
            return saleZoneGroups;
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
        private List<DealSaleZoneGroupZoneItem> BuildSaleZones(IEnumerable<long> saleZoneIds)
        {
            if (saleZoneIds == null || !saleZoneIds.Any())
                throw new NullReferenceException("saleZoneIds");

            return saleZoneIds.Select(zoneId => new DealSaleZoneGroupZoneItem { ZoneId = zoneId }).ToList();
        }

        private IOrderedEnumerable<DealSaleZoneGroupTier> BuildSaleTiers(SwapDealInbound swapDealInbound, Func<string, int, IEnumerable<SaleRateHistoryRecord>> getCustomerZoneRatesFunc)
        {
            var saleEvaluatedRate = swapDealInbound.EvaluatedRate as DealSaleRateEvaluator;

            if (saleEvaluatedRate == null)
                throw new NullReferenceException("Rate");

            var context = new DealSaleRateEvaluatorContext
            {
                GetCustomerZoneRatesFunc = getCustomerZoneRatesFunc,
                DealBED = BeginDate,
                DealEED = EndDate,
                ZoneIds = swapDealInbound.SaleZones.Select(z => z.ZoneId)
            };
            saleEvaluatedRate.EvaluateRate(context);

            Dictionary<long, List<DealRate>> saleRatesByZoneId = StructureDealRateByZoneId(context.SaleRates);
            DealSaleZoneGroupTier dealSaleZoneGroupTier = new DealSaleZoneGroupTier
            {
                ExceptionRates = null,
                RatesByZoneId = saleRatesByZoneId,
                RetroActiveFromTierNumber = null,
                TierNumber = 1,
                VolumeInSeconds = swapDealInbound.Volume * 60,
                CurrencyId = CurrencyId
            };
            return new List<DealSaleZoneGroupTier> { dealSaleZoneGroupTier }.OrderBy(itm => itm.TierNumber);
        }

        private List<DealSupplierZoneGroup> BuildSupplierZoneGroups(int dealId)
        {
            if (Outbounds == null || Outbounds.Count == 0)
                return null;

            var dealSupplierZoneGroups = new List<DealSupplierZoneGroup>();
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
                    Zones = BuildSupplierZones(swapDealOutbound.SupplierZones.Select(z => z.ZoneId))
                };
                dealSupplierZoneGroups.Add(dealSupplierZoneGroup);
            }
            return dealSupplierZoneGroups;
        }

        private List<DealSupplierZoneGroupZoneItem> BuildSupplierZones(IEnumerable<long> supplierZoneIds)
        {
            if (supplierZoneIds == null || !supplierZoneIds.Any())
                throw new NullReferenceException("supplierZoneIds");

            return supplierZoneIds.Select(zoneId => new DealSupplierZoneGroupZoneItem { ZoneId = zoneId }).ToList();
        }
        private IOrderedEnumerable<DealSupplierZoneGroupTier> BuildSupplierTiers(SwapDealOutbound swapDealOutbound)
        {
            SupplierRateManager supplierRateManager = new SupplierRateManager();

            List<long> supplierZoneIds = swapDealOutbound.SupplierZones.Select(z => z.ZoneId).ToList();
            var supplierRates = supplierRateManager.GetSupplierRateByZoneId(supplierZoneIds, BeginDate);

            var context = new DealSupplierRateEvaluatorContext
            {
                DealBED = BeginDate,
                DealEED = EndDate,
                ZoneIds = supplierZoneIds,
                SupplierZoneRateByZoneId = supplierRates
            };
            var supplierEvaluatedRate = swapDealOutbound.EvaluatedRate as DealSupplierRateEvaluator;

            if (supplierEvaluatedRate == null)
                throw new NullReferenceException("DealSupplierRateEvaluator");

            supplierEvaluatedRate.EvaluateRate(context);
            Dictionary<long, List<DealRate>> supplierDealRatesByZoneId = StructureDealRateByZoneId(context.SupplierRates);

            DealSupplierZoneGroupTier dealSupplierZoneGroupTier = new DealSupplierZoneGroupTier()
            {
                ExceptionRates = null,
                RatesByZoneId = supplierDealRatesByZoneId,
                RetroActiveFromTierNumber = null,
                TierNumber = 1,
                VolumeInSeconds = swapDealOutbound.Volume * 60,
                CurrencyId = CurrencyId
            };
            return new List<DealSupplierZoneGroupTier> { dealSupplierZoneGroupTier }.OrderBy(itm => itm.TierNumber);
        }
        private Dictionary<long, List<DealRate>> StructureDealRateByZoneId(IEnumerable<DealRate> dealRates)
        {
            var dealRateByZoneId = new Dictionary<long, List<DealRate>>();
            foreach (var dealRate in dealRates)
            {
                List<DealRate> rates = dealRateByZoneId.GetOrCreateItem(dealRate.ZoneId);
                rates.Add(dealRate);
            }
            return dealRateByZoneId;
        }
    }
}