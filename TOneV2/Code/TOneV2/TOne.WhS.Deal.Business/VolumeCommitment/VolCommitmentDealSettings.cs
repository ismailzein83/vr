using System;
using System.Linq;
using Vanrise.Common;
using TOne.WhS.Deal.Entities;
using Vanrise.Common.Business;
using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;

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

        public override DateTime? RealEED
        {
            get
            {
                if (DeActivationDate.HasValue)
                    return DeActivationDate < EndDate ? DeActivationDate : EndDate;
                return EndDate;
            }
        }

        public override int GetCarrierAccountId()
        {
            return CarrierAccountId;
        }

        public override string GetSaleZoneGroupName(int dealGroupNumber)
        {
            if (DealType == VolCommitmentDealType.Buy)
                return null;
            var item = Items.FindRecord(x => x.ZoneGroupNumber == dealGroupNumber);
            if (item == null)
                throw new NullReferenceException();
            return item.Name;
        }

        public override string GetSupplierZoneGroupName(int dealGroupNumber)
        {
            if (DealType == VolCommitmentDealType.Sell)
                return null;
            var item = Items.FindRecord(x => x.ZoneGroupNumber == dealGroupNumber);
            if (item == null)
                throw new NullReferenceException();
            return item.Name;
        }

        public override bool ValidateDataBeforeSave(IValidateBeforeSaveContext validateBeforeSaveContext)
        {
            DealDefinitionManager dealDefinitionManager = new DealDefinitionManager();
            SaleZoneManager saleZoneManager = new SaleZoneManager();
            SupplierZoneManager supplierZoneManager = new SupplierZoneManager();

            validateBeforeSaveContext.ValidateMessages = new List<string>();
            bool validationResult = true;

            var excludedSaleZones = dealDefinitionManager.GetExcludedSaleZones(validateBeforeSaveContext.DealId, this.CarrierAccountId, validateBeforeSaveContext.DealSaleZoneIds, this.BeginDate, this.EndDate);
            var excludedSupplierZones = dealDefinitionManager.GetExcludedSupplierZones(validateBeforeSaveContext.DealId, this.CarrierAccountId, validateBeforeSaveContext.DealSupplierZoneIds, this.BeginDate, this.EndDate);

            if (excludedSaleZones.Count() > 0 || excludedSupplierZones.Count > 0)
            {
                var excludedSaleZoneNames = saleZoneManager.GetSaleZoneNames(excludedSaleZones);
                var excludedSupplierZoneNames = supplierZoneManager.GetSupplierZoneNames(excludedSupplierZones);
                validationResult = false;
                if (excludedSaleZoneNames.Count() > 0)
                    validateBeforeSaveContext.ValidateMessages.Add(string.Format("The following sale zone(s) {0} are overlapping with zones in other deals", string.Join(",", excludedSaleZoneNames)));
                if (excludedSupplierZoneNames.Count() > 0)
                    validateBeforeSaveContext.ValidateMessages.Add(string.Format("The following supplier zone(s) {0} are overlapping with zones in other deals", string.Join(",", excludedSupplierZoneNames)));
            }
            if (DealType == VolCommitmentDealType.Sell)
            {
                var invalidCountryIds = ValidateVolumeCommitmentCountries(CarrierAccountId, BeginDate, false);
                if (invalidCountryIds.Count() > 0)
                {
                    CountryManager countrymanager = new CountryManager();
                    var invalidCountryNames = countrymanager.GetCountryNames(invalidCountryIds);
                    validationResult = false;
                    validateBeforeSaveContext.ValidateMessages.Add(string.Format("The following countries {0} are not sold at {1}", string.Join(",", invalidCountryNames), BeginDate));
                }
            }

            return validationResult;
        }

        public override List<long> GetDealSaleZoneIds()
        {
            if (DealType == VolCommitmentDealType.Buy)
                return null;

            var zoneIds = new List<long>();
            foreach (var item in Items)
            {
                var zones = item.SaleZones;
                foreach (var zone in zones)
                {
                    zoneIds.Add(zone.ZoneId);
                }
            }
            return zoneIds;
        }
        private List<int> ValidateVolumeCommitmentCountries(int customerId, DateTime? effectiveOn, bool isEffectiveInFuture)
        {
            List<int> invalidCountries = new List<int>();
            CustomerCountryManager customerCountryManager = new CustomerCountryManager();
            var customerCountries = customerCountryManager.GetCustomerCountryIds(customerId, effectiveOn, isEffectiveInFuture);
            foreach (var item in Items)
            {
                if (item.CountryIds != null)
                {
                    foreach (var countryId in item.CountryIds)
                    {
                        if (!customerCountries.Contains(countryId))
                        {
                            invalidCountries.Add(countryId);
                        }
                    }
                }
            }
            return invalidCountries;
        }

        public override List<long> GetDealSupplierZoneIds()
        {
            if (DealType == VolCommitmentDealType.Sell)
                return null;
            else
            {
                List<long> zoneIds = new List<long>();
                foreach (var item in Items)
                {
                    var zones = item.SaleZones;
                    foreach (var zone in zones)
                    {
                        zoneIds.Add(zone.ZoneId);
                    }
                }
                return zoneIds;
            }
        }

        public override void GetZoneGroups(IDealGetZoneGroupsContext context)
        {
            if (Status == DealStatus.Draft || (RealEED.HasValue && BeginDate == RealEED))
                return;

            switch (DealType)
            {
                case VolCommitmentDealType.Buy:
                    if (Items != null && (context.DealZoneGroupPart == DealZoneGroupPart.Both || context.DealZoneGroupPart == DealZoneGroupPart.Cost))
                    {
                        List<BaseDealSupplierZoneGroup> dealSupplierZoneGroups = new List<BaseDealSupplierZoneGroup>();
                        foreach (VolCommitmentDealItem volCommitmentDealItem in Items)
                        {
                            var zoneIds = volCommitmentDealItem.SaleZones.Select(z => z.ZoneId);
                            List<DealSupplierZoneGroupZoneItem> supplierZones = Helper.BuildSupplierZones(zoneIds, BeginDate, RealEED);
                            BaseDealSupplierZoneGroup dealSupplierZoneGroup;

                            if (context.EvaluateRates)
                                dealSupplierZoneGroup = new DealSupplierZoneGroup { Tiers = BuildSupplierTiers(volCommitmentDealItem.Tiers, supplierZones, RealEED) };
                            else
                                dealSupplierZoneGroup = new DealSupplierZoneGroupWithoutRate { Tiers = BuildSupplierTiersWithoutRate(volCommitmentDealItem.Tiers) };

                            dealSupplierZoneGroup.DealId = context.DealId;
                            dealSupplierZoneGroup.DealSupplierZoneGroupNb = volCommitmentDealItem.ZoneGroupNumber;
                            dealSupplierZoneGroup.SupplierId = CarrierAccountId;
                            dealSupplierZoneGroup.Zones = supplierZones;
                            dealSupplierZoneGroup.BED = BeginDate;
                            dealSupplierZoneGroup.EED = RealEED;

                            dealSupplierZoneGroups.Add(dealSupplierZoneGroup);
                        }
                        context.SupplierZoneGroups = dealSupplierZoneGroups;
                    }
                    break;

                case VolCommitmentDealType.Sell:
                    if (Items != null && (context.DealZoneGroupPart == DealZoneGroupPart.Both || context.DealZoneGroupPart == DealZoneGroupPart.Sale))
                    {
                        List<BaseDealSaleZoneGroup> dealSaleZoneGroups = new List<BaseDealSaleZoneGroup>();
                        foreach (VolCommitmentDealItem volCommitmentDealItem in Items)
                        {
                            var zoneIds = volCommitmentDealItem.SaleZones.Select(z => z.ZoneId);
                            List<DealSaleZoneGroupZoneItem> saleZones = Helper.BuildSaleZones(zoneIds, BeginDate, RealEED);
                            BaseDealSaleZoneGroup dealSaleZoneGroup;

                            if (context.EvaluateRates)
                                dealSaleZoneGroup = new DealSaleZoneGroup { Tiers = BuildSaleTiers(volCommitmentDealItem.Tiers, saleZones, RealEED) };
                            else
                                dealSaleZoneGroup = new DealSaleZoneGroupWithoutRate { Tiers = BuildSaleTiersWithoutRate(volCommitmentDealItem.Tiers) };

                            dealSaleZoneGroup.DealId = context.DealId;
                            dealSaleZoneGroup.DealSaleZoneGroupNb = volCommitmentDealItem.ZoneGroupNumber;
                            dealSaleZoneGroup.CustomerId = CarrierAccountId;
                            dealSaleZoneGroup.Zones = saleZones;
                            dealSaleZoneGroup.BED = BeginDate;
                            dealSaleZoneGroup.EED = RealEED;

                            dealSaleZoneGroups.Add(dealSaleZoneGroup);
                        }
                        context.SaleZoneGroups = dealSaleZoneGroups;
                    }
                    break;
            }
        }

        public override void GetRoutingZoneGroups(IDealGetRoutingZoneGroupsContext context)
        {
            if (Status == DealStatus.Draft && Items == null)
                return;

            switch (DealType)
            {
                case VolCommitmentDealType.Buy:
                    if (context.DealZoneGroupPart == DealZoneGroupPart.Both || context.DealZoneGroupPart == DealZoneGroupPart.Cost)
                    {
                        List<DealRoutingSupplierZoneGroup> supplierZoneGroups = new List<DealRoutingSupplierZoneGroup>();
                        foreach (VolCommitmentDealItem volCommitmentDealItem in Items)
                        {
                            supplierZoneGroups.Add(new DealRoutingSupplierZoneGroup
                            {
                                Tiers = BuildRoutingSupplierTiers(volCommitmentDealItem),
                                DealSupplierZoneGroupNb = volCommitmentDealItem.ZoneGroupNumber
                            });
                        }
                        context.SupplierZoneGroups = supplierZoneGroups;
                    }
                    break;

                case VolCommitmentDealType.Sell:
                    if (context.DealZoneGroupPart == DealZoneGroupPart.Both || context.DealZoneGroupPart == DealZoneGroupPart.Sale)
                    {
                        List<DealRoutingSaleZoneGroup> saleZoneGroups = new List<DealRoutingSaleZoneGroup>();
                        foreach (VolCommitmentDealItem volCommitmentDealItem in Items)
                        {
                            saleZoneGroups.Add(new DealRoutingSaleZoneGroup
                            {
                                Tiers = BuildRoutingSaleTiers(volCommitmentDealItem),
                                DealSaleZoneGroupNb = volCommitmentDealItem.ZoneGroupNumber
                            });
                        }
                        context.SaleZoneGroups = saleZoneGroups;
                    }
                    break;
            }

        }

        #region Sale Methods

        private List<DealRoutingSaleZoneGroupTier> BuildRoutingSaleTiers(VolCommitmentDealItem item)
        {
            var dealSaleTiers = new List<DealRoutingSaleZoneGroupTier> { };

            for (int i = 0; i < item.Tiers.Count(); i++)
            {
                DealRoutingSaleZoneGroupTier dealRoutingSaleZoneGroupTier = new DealRoutingSaleZoneGroupTier
                {
                    TierNumber = i + 1,
                    SubstituteRate = null
                };
                dealSaleTiers.Add(dealRoutingSaleZoneGroupTier);
            }

            return dealSaleTiers;
        }
        private List<DealSaleZoneGroupZoneItem> BuildSaleZones(IEnumerable<long> zoneIds, DateTime? dealEED)
        {
            if (zoneIds == null || !zoneIds.Any())
                throw new NullReferenceException("zoneIds");

            var saleZoneItems = new List<DealSaleZoneGroupZoneItem>();
            var saleZoneManager = new SaleZoneManager();
            foreach (var zoneId in zoneIds)
            {
                var saleZone = saleZoneManager.GetSaleZone(zoneId);
                DateTime? zoneEED = (saleZone.EED.HasValue && dealEED.HasValue)
                   ? Utilities.Min(saleZone.EED.Value, dealEED.Value)
                   : (saleZone.EED ?? dealEED);
                saleZoneItems.Add(new DealSaleZoneGroupZoneItem
                {
                    ZoneId = zoneId,
                    BED = BeginDate,
                    EED = zoneEED
                });
            }
            return saleZoneItems;
        }

        private void SetRateLocatorContext(IEnumerable<long> zoneIds, out Func<string, int, IEnumerable<SaleRateHistoryRecord>> getCustomerZoneRatesFunc)
        {
            var carrierAccountManager = new CarrierAccountManager();
            int sellingProductId = carrierAccountManager.GetSellingProductId(CarrierAccountId);

            var customerZoneRateHistoryLocator =
               new CustomerZoneRateHistoryLocator(new CustomerZoneRateHistoryReader(new List<int> { CarrierAccountId }
                   , new List<int> { sellingProductId }, zoneIds, BeginDate, EndDate, false));

            getCustomerZoneRatesFunc = (zoneName, countryId) => customerZoneRateHistoryLocator.GetCustomerZoneRateHistory(CarrierAccountId, sellingProductId, zoneName, null, countryId, null, null);
        }


        private IOrderedEnumerable<DealSaleZoneGroupTier> BuildSaleTiers(List<VolCommitmentDealItemTier> volCommitmentDealItemTiers, List<DealSaleZoneGroupZoneItem> saleZones, DateTime? dealEED)
        {
            if (volCommitmentDealItemTiers == null || volCommitmentDealItemTiers.Count == 0)
                throw new NullReferenceException("volCommitmentDealItemTiers");

            int tierNumber = 1;
            IOrderedEnumerable<VolCommitmentDealItemTier> orderedVolCommitmentDealItemTiers = volCommitmentDealItemTiers.OrderBy(itm => itm.UpToVolume ?? Int32.MaxValue);

            var dealSaleZoneGroupTiers = new List<DealSaleZoneGroupTier>();
            int previousVolumeAssigned = 0;
            IEnumerable<long> saleZoneIds = saleZones.Select(z => z.ZoneId);

            Func<string, int, IEnumerable<SaleRateHistoryRecord>> getCustomerZoneRatesFunc;
            SetRateLocatorContext(saleZoneIds, out getCustomerZoneRatesFunc);

            var context = new DealSaleRateEvaluatorContext
            {
                GetCustomerZoneRatesFunc = getCustomerZoneRatesFunc,
                DealBED = BeginDate,
                DealEED = dealEED,
                SaleZoneGroupItem = saleZones,
                CurrencyId = CurrencyId
            };

            foreach (VolCommitmentDealItemTier volCommitmentDealItemTier in orderedVolCommitmentDealItemTiers)
            {
                var saleEvaluatedRate = volCommitmentDealItemTier.EvaluatedRate as DealSaleRateEvaluator;

                if (saleEvaluatedRate == null)
                    throw new NullReferenceException("DealSaleRateEvaluator");

                context.SaleZoneGroupItem = saleZones;

                Dictionary<long, List<DealRate>> exceptionDealRates = null;
                if (volCommitmentDealItemTier.ExceptionZoneRates != null && volCommitmentDealItemTier.ExceptionZoneRates.Any())
                {
                    exceptionDealRates = GetExceptionSaleDealRate(volCommitmentDealItemTier.ExceptionZoneRates, dealEED);
                    var zoneIds = saleZoneIds.Except(exceptionDealRates.Keys);
                    context.SaleZoneGroupItem = Helper.BuildSaleZones(zoneIds, BeginDate, dealEED);
                }

                Dictionary<long, List<DealRate>> dealRatesByZoneId = null;
                if (context.SaleZoneGroupItem != null && context.SaleZoneGroupItem.Any())
                {
                    saleEvaluatedRate.EvaluateRate(context);
                    dealRatesByZoneId = Helper.StructureDealRateByZoneId(context.SaleRates);
                }

                Dictionary<long, List<DealRate>> supplierDealRatesByZoneId = MergeExceptionAndDealRate(exceptionDealRates, dealRatesByZoneId);

                DealSaleZoneGroupTier dealSaleZoneGroupTier = new DealSaleZoneGroupTier
                {
                    RetroActiveFromTierNumber = volCommitmentDealItemTier.RetroActiveFromTierNumber,
                    VolumeInSeconds = volCommitmentDealItemTier.UpToVolume.HasValue ? (volCommitmentDealItemTier.UpToVolume.Value * 60) - previousVolumeAssigned : (int?)null,
                    TierNumber = tierNumber,
                    RatesByZoneId = supplierDealRatesByZoneId
                };
                dealSaleZoneGroupTiers.Add(dealSaleZoneGroupTier);
                tierNumber++;
                previousVolumeAssigned += dealSaleZoneGroupTier.VolumeInSeconds ?? 0;
            }
            return dealSaleZoneGroupTiers.OrderBy(itm => itm.TierNumber);
        }

        private IOrderedEnumerable<DealSaleZoneGroupTierWithoutRate> BuildSaleTiersWithoutRate(List<VolCommitmentDealItemTier> volCommitmentDealItemTiers)
        {
            if (volCommitmentDealItemTiers == null || volCommitmentDealItemTiers.Count == 0)
                throw new NullReferenceException("volCommitmentDealItemTiers");

            int tierNumber = 1;
            IOrderedEnumerable<VolCommitmentDealItemTier> orderedVolCommitmentDealItemTiers = volCommitmentDealItemTiers.OrderBy(itm => itm.UpToVolume ?? Int32.MaxValue);

            var dealSaleZoneGroupTiers = new List<DealSaleZoneGroupTierWithoutRate>();
            int previousVolumeAssigned = 0;

            foreach (VolCommitmentDealItemTier volCommitmentDealItemTier in orderedVolCommitmentDealItemTiers)
            {
                DealSaleZoneGroupTierWithoutRate dealSaleZoneGroupTier = new DealSaleZoneGroupTierWithoutRate
                {
                    RetroActiveFromTierNumber = volCommitmentDealItemTier.RetroActiveFromTierNumber,
                    VolumeInSeconds = volCommitmentDealItemTier.UpToVolume.HasValue ? (volCommitmentDealItemTier.UpToVolume.Value * 60) - previousVolumeAssigned : (int?)null,
                    TierNumber = tierNumber,
                };
                dealSaleZoneGroupTiers.Add(dealSaleZoneGroupTier);
                tierNumber++;
                previousVolumeAssigned += dealSaleZoneGroupTier.VolumeInSeconds ?? 0;
            }
            return dealSaleZoneGroupTiers.OrderBy(itm => itm.TierNumber);
        }

        private Dictionary<long, List<DealRate>> GetExceptionSaleDealRate(IEnumerable<VolCommitmentDealItemTierZoneRate> exceptionZoneRates, DateTime? dealEED)
        {
            Func<string, int, IEnumerable<SaleRateHistoryRecord>> getCustomerZoneRatesFunc;
            SetRateLocatorContext(exceptionZoneRates.SelectMany(exc => exc.Zones.Select(z => z.ZoneId)), out getCustomerZoneRatesFunc);

            var dealRates = new List<DealRate>();
            foreach (var exceptionZoneRate in exceptionZoneRates)
            {
                var salerEvaluatedRate = exceptionZoneRate.EvaluatedRate as DealSaleRateEvaluator;

                if (salerEvaluatedRate == null)
                    continue;

                var zoneExceptioncontext = new DealSaleRateEvaluatorContext
                {
                    DealBED = BeginDate,
                    DealEED = dealEED,
                    SaleZoneGroupItem = Helper.BuildSaleZones(exceptionZoneRate.Zones.Select(z => z.ZoneId), BeginDate, dealEED),
                    GetCustomerZoneRatesFunc = getCustomerZoneRatesFunc,
                    CurrencyId = CurrencyId
                };
                salerEvaluatedRate.EvaluateRate(zoneExceptioncontext);
                if (zoneExceptioncontext.SaleRates != null)
                    dealRates.AddRange(zoneExceptioncontext.SaleRates);
            }
            Dictionary<long, List<DealRate>> dealRateByZoneId = Helper.StructureDealRateByZoneId(dealRates);
            return dealRateByZoneId;
        }

        #endregion

        #region Supplier Methods

        private List<DealRoutingSupplierZoneGroupTier> BuildRoutingSupplierTiers(VolCommitmentDealItem item)
        {
            var dealSupplierTiers = new List<DealRoutingSupplierZoneGroupTier> { };

            for (int i = 0; i < item.Tiers.Count(); i++)
            {
                DealRoutingSupplierZoneGroupTier dealRoutingSupplierZoneGroupTier = new DealRoutingSupplierZoneGroupTier
                {
                    TierNumber = i + 1,
                    SubstituteRate = null
                };
                dealSupplierTiers.Add(dealRoutingSupplierZoneGroupTier);
            }
            return dealSupplierTiers;
        }
        private IOrderedEnumerable<DealSupplierZoneGroupTier> BuildSupplierTiers(List<VolCommitmentDealItemTier> volCommitmentDealItemTiers, List<DealSupplierZoneGroupZoneItem> supplierZones, DateTime? dealEED)
        {
            if (volCommitmentDealItemTiers == null || volCommitmentDealItemTiers.Count == 0)
                throw new NullReferenceException("volCommitmentDealItemTiers");

            int tierNumber = 1;
            IOrderedEnumerable<VolCommitmentDealItemTier> orderedVolCommitmentDealItemTiers = volCommitmentDealItemTiers.OrderBy(itm => itm.UpToVolume ?? Int32.MaxValue);

            var supplierRateManager = new SupplierRateManager();
            List<long> supplierZoneIds = supplierZones.Select(z => z.ZoneId).ToList();
            var supplierRatesByZoneId = supplierRateManager.GetSupplierRateByZoneId(supplierZoneIds, BeginDate, EndDate);

            var context = new DealSupplierRateEvaluatorContext
            {
                DealBED = BeginDate,
                DealEED = dealEED,
                SupplierZoneRateByZoneId = supplierRatesByZoneId,
                CurrencyId = CurrencyId
            };

            List<DealSupplierZoneGroupTier> dealSupplierZoneGroupTiers = new List<DealSupplierZoneGroupTier>();
            int previousVolumeAssigned = 0;
            foreach (VolCommitmentDealItemTier volCommitmentDealItemTier in orderedVolCommitmentDealItemTiers)
            {
                var supplierEvaluatedRate = volCommitmentDealItemTier.EvaluatedRate as DealSupplierRateEvaluator;

                if (supplierEvaluatedRate == null)
                    throw new NullReferenceException("DealSupplierRateEvaluator");

                context.SupplierZoneItem = supplierZones;

                Dictionary<long, List<DealRate>> exceptionDealRates = null;
                if (volCommitmentDealItemTier.ExceptionZoneRates != null)
                {
                    exceptionDealRates = GetExceptionSupplierDealRate(volCommitmentDealItemTier.ExceptionZoneRates, supplierRatesByZoneId, dealEED);
                    var zoneIds = supplierZoneIds.Except(exceptionDealRates.Keys);
                    context.SupplierZoneItem = Helper.BuildSupplierZones(zoneIds, BeginDate, dealEED);
                }

                Dictionary<long, List<DealRate>> supplierRateByZoneId = null;
                if (context.SupplierZoneItem != null && context.SupplierZoneItem.Any())
                {
                    supplierEvaluatedRate.EvaluateRate(context);
                    supplierRateByZoneId = Helper.StructureDealRateByZoneId(context.SupplierRates);
                }

                Dictionary<long, List<DealRate>> supplierDealRatesByZoneId = MergeExceptionAndDealRate(exceptionDealRates, supplierRateByZoneId);
                DealSupplierZoneGroupTier dealSupplierZoneGroupTier = new DealSupplierZoneGroupTier
                {
                    RetroActiveFromTierNumber = volCommitmentDealItemTier.RetroActiveFromTierNumber,
                    VolumeInSeconds = volCommitmentDealItemTier.UpToVolume.HasValue ? (volCommitmentDealItemTier.UpToVolume.Value * 60) - previousVolumeAssigned : (int?)null,
                    RatesByZoneId = supplierDealRatesByZoneId,
                    TierNumber = tierNumber
                };
                dealSupplierZoneGroupTiers.Add(dealSupplierZoneGroupTier);
                tierNumber++;
                previousVolumeAssigned += dealSupplierZoneGroupTier.VolumeInSeconds ?? 0;
            }
            return dealSupplierZoneGroupTiers.OrderBy(itm => itm.TierNumber);
        }

        private IOrderedEnumerable<DealSupplierZoneGroupTierWithoutRate> BuildSupplierTiersWithoutRate(List<VolCommitmentDealItemTier> volCommitmentDealItemTiers)
        {
            if (volCommitmentDealItemTiers == null || volCommitmentDealItemTiers.Count == 0)
                throw new NullReferenceException("volCommitmentDealItemTiers");

            int tierNumber = 1;
            IOrderedEnumerable<VolCommitmentDealItemTier> orderedVolCommitmentDealItemTiers = volCommitmentDealItemTiers.OrderBy(itm => itm.UpToVolume ?? Int32.MaxValue);

            List<DealSupplierZoneGroupTierWithoutRate> dealSupplierZoneGroupTiers = new List<DealSupplierZoneGroupTierWithoutRate>();
            int previousVolumeAssigned = 0;
            foreach (VolCommitmentDealItemTier volCommitmentDealItemTier in orderedVolCommitmentDealItemTiers)
            {
                DealSupplierZoneGroupTierWithoutRate dealSupplierZoneGroupTier = new DealSupplierZoneGroupTierWithoutRate
                {
                    RetroActiveFromTierNumber = volCommitmentDealItemTier.RetroActiveFromTierNumber,
                    VolumeInSeconds = volCommitmentDealItemTier.UpToVolume.HasValue ? (volCommitmentDealItemTier.UpToVolume.Value * 60) - previousVolumeAssigned : (int?)null,
                    TierNumber = tierNumber
                };
                dealSupplierZoneGroupTiers.Add(dealSupplierZoneGroupTier);
                tierNumber++;
                previousVolumeAssigned += dealSupplierZoneGroupTier.VolumeInSeconds ?? 0;
            }
            return dealSupplierZoneGroupTiers.OrderBy(itm => itm.TierNumber);
        }

        private Dictionary<long, List<DealRate>> MergeExceptionAndDealRate(Dictionary<long, List<DealRate>> exceptionDealRates, Dictionary<long, List<DealRate>> dealRatesByZoneId)
        {
            if (exceptionDealRates == null && dealRatesByZoneId == null)
                return null;

            if (exceptionDealRates == null)
                return dealRatesByZoneId;

            if (dealRatesByZoneId == null)
                return exceptionDealRates;

            return exceptionDealRates.Union(dealRatesByZoneId).ToDictionary(k => k.Key, v => v.Value);
        }

        private Dictionary<long, List<DealRate>> GetExceptionSupplierDealRate(IEnumerable<VolCommitmentDealItemTierZoneRate> exceptionZoneRates, Dictionary<long, SupplierRate> supplierRatesByZoneId, DateTime? dealEED)
        {
            var dealRates = new List<DealRate>();
            foreach (var exceptionZoneRate in exceptionZoneRates)
            {
                var supplierEvaluatedRate = exceptionZoneRate.EvaluatedRate as DealSupplierRateEvaluator;

                if (supplierEvaluatedRate == null)
                    continue;

                var zoneExceptioncontext = new DealSupplierRateEvaluatorContext
                {
                    DealBED = BeginDate,
                    DealEED = dealEED,
                    SupplierZoneRateByZoneId = supplierRatesByZoneId,
                    SupplierZoneItem = Helper.BuildSupplierZones(exceptionZoneRate.Zones.Select(z => z.ZoneId), BeginDate, dealEED)
                };
                supplierEvaluatedRate.EvaluateRate(zoneExceptioncontext);

                if (zoneExceptioncontext.SupplierRates != null)
                    dealRates.AddRange(zoneExceptioncontext.SupplierRates);
            }
            Dictionary<long, List<DealRate>> dealRateByZoneId = Helper.StructureDealRateByZoneId(dealRates);
            return dealRateByZoneId;
        }

        #endregion
    }
}