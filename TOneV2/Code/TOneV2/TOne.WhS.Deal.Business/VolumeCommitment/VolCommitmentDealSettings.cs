﻿using System;
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
    public class VolCommitmentDealSettings : DealSettings
    {
        public static Guid VolCommitmentDealSettingsConfigId = new Guid("B606E88C-4AE5-4BF0-BCE5-10D456A092F5");
        public override Guid ConfigId { get { return VolCommitmentDealSettingsConfigId; } }

        public VolCommitmentDealType DealType { get; set; }

        public int CarrierAccountId { get; set; }

        public List<VolCommitmentDealItem> Items { get; set; }

        public int LastGroupNumber { get; set; }

        public int CurrencyId { get; set; }

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
        private List<int> ValidateVolumeCommitmentCountries(int customerId, DateTime? effectiveOn, bool isEffectiveInFuture)
        {
            List<int> invalidCountries = new List<int>();
            CustomerCountryManager customerCountryManager = new CustomerCountryManager();
            var customerCountries = customerCountryManager.GetCustomerCountryIds(customerId, effectiveOn, isEffectiveInFuture);
            foreach (var item in Items)
            {
                if (!customerCountries.Contains(item.CountryId))
                {
                    invalidCountries.Add(item.CountryId);
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
            if (Status == DealStatus.Draft || BeginDate == EndDate)
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
                            BaseDealSupplierZoneGroup dealSupplierZoneGroup;

                            if (context.EvaluateRates)
                                dealSupplierZoneGroup = new DealSupplierZoneGroup() { Tiers = BuildSupplierTiers(volCommitmentDealItem.Tiers, zoneIds) };
                            else
                                dealSupplierZoneGroup = new DealSupplierZoneGroupWithoutRate() { Tiers = BuildSupplierTiersWithoutRate(volCommitmentDealItem.Tiers) };

                            dealSupplierZoneGroup.DealId = context.DealId;
                            dealSupplierZoneGroup.DealSupplierZoneGroupNb = volCommitmentDealItem.ZoneGroupNumber;
                            dealSupplierZoneGroup.SupplierId = CarrierAccountId;
                            dealSupplierZoneGroup.Zones = BuildSupplierZones(zoneIds);
                            dealSupplierZoneGroup.Status = base.Status;
                            dealSupplierZoneGroup.DeActivationDate = base.DeActivationDate;
                            dealSupplierZoneGroup.BED = BeginDate;
                            dealSupplierZoneGroup.EED = EndDate;

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

                            BaseDealSaleZoneGroup dealSaleZoneGroup;

                            if (context.EvaluateRates)
                                dealSaleZoneGroup = new DealSaleZoneGroup() { Tiers = BuildSaleTiers(volCommitmentDealItem.Tiers, zoneIds) };
                            else
                                dealSaleZoneGroup = new DealSaleZoneGroupWithoutRate() { Tiers = BuildSaleTiersWithoutRate(volCommitmentDealItem.Tiers) };

                            dealSaleZoneGroup.DealId = context.DealId;
                            dealSaleZoneGroup.DealSaleZoneGroupNb = volCommitmentDealItem.ZoneGroupNumber;
                            dealSaleZoneGroup.CustomerId = CarrierAccountId;
                            dealSaleZoneGroup.Zones = BuildSaleZones(zoneIds);
                            dealSaleZoneGroup.Status = base.Status;
                            dealSaleZoneGroup.DeActivationDate = base.DeActivationDate;
                            dealSaleZoneGroup.BED = BeginDate;
                            dealSaleZoneGroup.EED = EndDate;

                            dealSaleZoneGroups.Add(dealSaleZoneGroup);
                        }
                        context.SaleZoneGroups = dealSaleZoneGroups;
                    }
                    break;
            }
        }

        private List<string> GetSaleZoneNames(List<long> saleZoneIds)
        {
            List<string> zoneNames = new List<string>();
            SaleZoneManager saleZoneManager = new SaleZoneManager();
            foreach (var zoneId in saleZoneIds)
            {
                var zoneName = saleZoneManager.GetSaleZoneName(zoneId);
                if (zoneName != null)
                    zoneNames.Add(zoneName);
            }
            return zoneNames;
        }

        private List<string> GetSupplierZoneNames(List<long> supplierZoneIds)
        {
            List<string> zoneNames = new List<string>();
            SupplierZoneManager supplierZoneManager = new SupplierZoneManager();
            foreach (var zoneId in supplierZoneIds)
            {
                var zoneName = supplierZoneManager.GetSupplierZoneName(zoneId);
                if (zoneName != null)
                    zoneNames.Add(zoneName);
            }
            return zoneNames;
        }

        #region Sale Methods

        private List<DealSaleZoneGroupZoneItem> BuildSaleZones(IEnumerable<long> zoneIds)
        {
            if (zoneIds == null || !zoneIds.Any())
                throw new NullReferenceException("zoneIds");

            return zoneIds.Select(zoneId => new DealSaleZoneGroupZoneItem
            {
                ZoneId = zoneId,
                BED = BeginDate,
                EED = EndDate
            }).ToList();
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
                DealEED = DeActivationDate ?? EndDate,
                ZoneIds = zoneIds,
                CurrencyId = CurrencyId
            };

            foreach (VolCommitmentDealItemTier volCommitmentDealItemTier in orderedVolCommitmentDealItemTiers)
            {
                var saleEvaluatedRate = volCommitmentDealItemTier.EvaluatedRate as DealSaleRateEvaluator;

                if (saleEvaluatedRate == null)
                    throw new NullReferenceException("DealSaleRateEvaluator");

                context.ZoneIds = zoneIds;

                Dictionary<long, List<DealRate>> exceptionDealRates = null;
                if (volCommitmentDealItemTier.ExceptionZoneRates != null && volCommitmentDealItemTier.ExceptionZoneRates.Any())
                {
                    exceptionDealRates = GetExceptionSaleDealRate(volCommitmentDealItemTier.ExceptionZoneRates);
                    context.ZoneIds = zoneIds.Except(exceptionDealRates.Keys); //exclude exception from zones to evaluat
                }

                Dictionary<long, List<DealRate>> dealRatesByZoneId = null;
                if (context.ZoneIds != null && context.ZoneIds.Any())
                {
                    saleEvaluatedRate.EvaluateRate(context);
                    dealRatesByZoneId = Business.Helper.StructureDealRateByZoneId(context.SaleRates);
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

        #endregion

        #region Supplier Methods

        private List<DealSupplierZoneGroupZoneItem> BuildSupplierZones(IEnumerable<long> zoneIds)
        {
            if (zoneIds == null || !zoneIds.Any())
                throw new NullReferenceException("zoneIds");

            return zoneIds.Select(zoneId => new DealSupplierZoneGroupZoneItem
            {
                ZoneId = zoneId,
                BED = BeginDate,
                EED = EndDate
            }).ToList();
        }

        private IOrderedEnumerable<DealSupplierZoneGroupTier> BuildSupplierTiers(List<VolCommitmentDealItemTier> volCommitmentDealItemTiers, IEnumerable<long> zoneIds)
        {
            if (volCommitmentDealItemTiers == null || volCommitmentDealItemTiers.Count == 0)
                throw new NullReferenceException("volCommitmentDealItemTiers");

            int tierNumber = 1;
            IOrderedEnumerable<VolCommitmentDealItemTier> orderedVolCommitmentDealItemTiers = volCommitmentDealItemTiers.OrderBy(itm => itm.UpToVolume ?? Int32.MaxValue);

            var supplierRateManager = new SupplierRateManager();
            var supplierRatesByZoneId = supplierRateManager.GetSupplierRateByZoneId(zoneIds.ToList(), BeginDate, EndDate);

            var context = new DealSupplierRateEvaluatorContext
            {
                DealBED = BeginDate,
                DealEED = DeActivationDate ?? EndDate,
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

                context.ZoneIds = zoneIds;

                Dictionary<long, List<DealRate>> exceptionDealRates = null;
                if (volCommitmentDealItemTier.ExceptionZoneRates != null)
                {
                    exceptionDealRates = GetExceptionSupplierDealRate(volCommitmentDealItemTier.ExceptionZoneRates, supplierRatesByZoneId);
                    context.ZoneIds = context.ZoneIds.Except(exceptionDealRates.Keys);  //exclude exception from zones to evaluate
                }

                Dictionary<long, List<DealRate>> supplierRateByZoneId = null;
                if (context.ZoneIds != null && context.ZoneIds.Any())
                {
                    supplierEvaluatedRate.EvaluateRate(context);
                    supplierRateByZoneId = Business.Helper.StructureDealRateByZoneId(context.SupplierRates);
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

        private Dictionary<long, List<DealRate>> GetExceptionSaleDealRate(IEnumerable<VolCommitmentDealItemTierZoneRate> exceptionZoneRates)
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
                    DealEED = EndDate,
                    ZoneIds = exceptionZoneRate.Zones.Select(z => z.ZoneId),
                    GetCustomerZoneRatesFunc = getCustomerZoneRatesFunc,
                    CurrencyId = CurrencyId
                };
                salerEvaluatedRate.EvaluateRate(zoneExceptioncontext);
                if (zoneExceptioncontext.SaleRates != null)
                    dealRates.AddRange(zoneExceptioncontext.SaleRates);
            }
            Dictionary<long, List<DealRate>> dealRateByZoneId = Business.Helper.StructureDealRateByZoneId(dealRates);
            return dealRateByZoneId;
        }

        private Dictionary<long, List<DealRate>> GetExceptionSupplierDealRate(IEnumerable<VolCommitmentDealItemTierZoneRate> exceptionZoneRates, Dictionary<long, SupplierRate> supplierRatesByZoneId)
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
                    DealEED = EndDate,
                    SupplierZoneRateByZoneId = supplierRatesByZoneId,
                    ZoneIds = exceptionZoneRate.Zones.Select(z => z.ZoneId)
                };
                supplierEvaluatedRate.EvaluateRate(zoneExceptioncontext);

                if (zoneExceptioncontext.SupplierRates != null)
                    dealRates.AddRange(zoneExceptioncontext.SupplierRates);
            }
            Dictionary<long, List<DealRate>> dealRateByZoneId = Business.Helper.StructureDealRateByZoneId(dealRates);
            return dealRateByZoneId;
        }

        #endregion
    }
}