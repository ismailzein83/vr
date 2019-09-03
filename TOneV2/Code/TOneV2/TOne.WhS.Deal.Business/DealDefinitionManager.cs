﻿using System;
using System.Collections.Generic;
using System.Linq;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Deal.Entities;
using Vanrise.Common;
using Vanrise.Entities;
using Vanrise.GenericData.Entities;

namespace TOne.WhS.Deal.Business
{
    public class DealDefinitionManager : BaseDealManager
    {
        #region Public Methods

        public IEnumerable<DealDefinitionInfo> GetDealDefinitionInfo(DealDefinitionFilter filter)
        {
            bool getDeletedDeals = filter.SelectedDealDefinitionIds != null;
            var cachedDeals = GetAllCachedDealDefinitions(getDeletedDeals);

            Func<DealDefinition, bool> filterExpression = (dealDefinition) =>
            {
                if (filter == null)
                    return true;

                if (filter.SelectedDealDefinitionIds != null && filter.SelectedDealDefinitionIds.Contains(dealDefinition.DealId))
                    return true;

                if (dealDefinition.IsDeleted)
                    return false;

                if (filter.DealStatuses != null && !filter.DealStatuses.Contains(dealDefinition.Settings.Status))
                    return false;

                if (filter.IncludedDealDefinitionIds != null && !filter.IncludedDealDefinitionIds.Contains(dealDefinition.DealId))
                    return false;

                if (filter.ExcludedDealDefinitionIds != null && filter.ExcludedDealDefinitionIds.Contains(dealDefinition.DealId))
                    return false;

                if (filter.CarrierAccountIds != null && !filter.CarrierAccountIds.Contains(dealDefinition.Settings.GetCarrierAccountId()))
                    return false;

                if (filter.Filters != null)
                {
                    DealDefinitionFilterContext context = new DealDefinitionFilterContext() { DealDefinition = dealDefinition };
                    foreach (IDealDefinitionFilter dealDefinitionFilter in filter.Filters)
                    {
                        if (!dealDefinitionFilter.IsMatched(context))
                            return false;
                    }
                }

                return true;
            };

            Func<DealDefinition, DealDefinitionInfo> mapper = (itm) =>
            {
                return DealDefinitionInfoMapper(itm, filter != null ? filter.DealStatuses : null);
            };

            return cachedDeals.MapRecords(mapper, filterExpression).OrderBy(item => item.Name);
        }
        public bool DeleteDeal(int dealId)
        {
            return base.DeleteDeal(dealId);
        }

        /// <summary>
        /// used in data transformation
        /// </summary>
        /// <param name="record"></param>
        public void FillOrigCDRValues(dynamic record)
        {
            record.OrigSaleRateId = record.SaleRateId;
            record.OrigSaleRateValue = record.SaleRateValue;
            record.OrigSaleNet = record.SaleNet;
            record.OrigSaleExtraChargeRateValue = record.SaleExtraChargeRateValue;
            record.OrigSaleExtraChargeValue = record.SaleExtraChargeValue;
            record.OrigSaleDurationInSeconds = record.SaleDurationInSeconds;
            record.OrigSaleCurrencyId = record.SaleCurrencyId;

            record.OrigCostRateId = record.CostRateId;
            record.OrigCostRateValue = record.CostRateValue;
            record.OrigCostNet = record.CostNet;
            record.OrigCostExtraChargeRateValue = record.CostExtraChargeRateValue;
            record.OrigCostExtraChargeValue = record.CostExtraChargeValue;
            record.OrigCostDurationInSeconds = record.CostDurationInSeconds;
            record.OrigCostCurrencyId = record.CostCurrencyId;
        }

        /// <summary>
        /// used in data transformation
        /// </summary>
        /// <param name="record"></param>
        public void FillOrigSMSValues(dynamic record)
        {
            record.OrigSaleRateId = record.SaleRateId;
            record.OrigSaleRateValue = record.SaleRateValue;
            record.OrigSaleNet = record.SaleNet;
            record.OrigSaleExtraChargeRateValue = record.SaleExtraChargeRateValue;
            record.OrigSaleExtraChargeValue = record.SaleExtraChargeValue;
            record.OrigSaleCurrencyId = record.SaleCurrencyId;

            record.OrigCostRateId = record.CostRateId;
            record.OrigCostRateValue = record.CostRateValue;
            record.OrigCostNet = record.CostNet;
            record.OrigCostExtraChargeRateValue = record.CostExtraChargeRateValue;
            record.OrigCostExtraChargeValue = record.CostExtraChargeValue;
            record.OrigCostCurrencyId = record.CostCurrencyId;
        }

        public void FillOrigSaleDealValues(dynamic record)
        {
            DealSaleZoneGroupWithoutRate dealSaleZoneGroup = GetAccountSaleZoneGroup(record.CustomerId, record.SaleZoneId, record.AttemptDateTime);
            if (dealSaleZoneGroup != null)
            {
                record.OrigSaleDealId = dealSaleZoneGroup.DealId;
                record.OrigSaleDealZoneGroupNb = dealSaleZoneGroup.DealSaleZoneGroupNb;
            }
            else
            {
                record.OrigSaleDealId = null;
                record.OrigSaleDealZoneGroupNb = null;
            }
        }

        public void FillOrigCostDealValues(dynamic record)
        {
            DealSupplierZoneGroupWithoutRate dealSupplierZoneGroup = GetAccountSupplierZoneGroup(record.SupplierId, record.SupplierZoneId, record.AttemptDateTime);
            if (dealSupplierZoneGroup != null)
            {
                record.OrigCostDealId = dealSupplierZoneGroup.DealId;
                record.OrigCostDealZoneGroupNb = dealSupplierZoneGroup.DealSupplierZoneGroupNb;
            }
            else
            {
                record.OrigCostDealId = null;
                record.OrigCostDealZoneGroupNb = null;
            }
        }

        public int? IsZoneIncludedInDeal(int carrierId, long zoneId, DateTime effectiveAfter, bool isSale)
        {
            var dealZoneInfoByCarrierId = isSale
                ? GetCachedCustomerDealZoneInfoByCustomerId()
                : GetCachedSupplierDealZoneInfoBySupplierId();

            var carrierDeals = dealZoneInfoByCarrierId.GetRecord(carrierId);
            var dealZoneInfo = carrierDeals.GetRecord(zoneId);

            if (dealZoneInfo == null)
                return null;

            var zoneInfo = dealZoneInfo.First().Value;
            if (zoneInfo.EED.VRLessThan(effectiveAfter))
                return null;

            return zoneInfo.DealId;
        }
        public int? IsZoneIncludedInEffectiveDeal(int carrierId, long zoneId, DateTime effectiveAfter, bool isSale)
        {
            var dealZoneInfoByCarrierId = isSale
                ? GetCachedCustomerDealZoneInfoByCustomerId()
                : GetCachedSupplierDealZoneInfoBySupplierId();

            var carrierDeals = dealZoneInfoByCarrierId.GetRecord(carrierId);
            var dealZoneInfo = carrierDeals.GetRecord(zoneId);

            if (dealZoneInfo == null)
                return null;

            var zoneInfos = dealZoneInfo.Where(deal => deal.Value.DealStatus != DealStatus.Draft);

            if (zoneInfos == null || zoneInfos.Count() == 0)
                return null;

            foreach (var zoneInfo in zoneInfos)
            {
                var zoneInfoValue = zoneInfo.Value;
                if (Utilities.IsEffective(effectiveAfter, zoneInfoValue.BED, zoneInfoValue.EED))
                    return zoneInfoValue.DealId;
            }
            return null;
        }
        public DealDefinition GetLastDealByCarrierAccountId(int carrierAccountId)
        {
            var allDealDefinitions = GetAllCachedDealDefinitions(false);

            if (allDealDefinitions == null || allDealDefinitions.Values == null)
                return null;

            var lastDealDate = DateTime.MinValue;
            int? lastDealId = null;

            foreach (var dealDefinition in allDealDefinitions.Values)
            {
                if (dealDefinition != null && dealDefinition.Settings != null && dealDefinition.Settings.GetCarrierAccountId() == carrierAccountId && dealDefinition.Settings.BeginDate > lastDealDate)
                {
                    lastDealDate = dealDefinition.Settings.BeginDate;
                    lastDealId = dealDefinition.DealId;
                }
            }

            if (lastDealId.HasValue)
                return allDealDefinitions.GetRecord(lastDealId.Value);
            return null;
        }
        public DealInfo GetLastDealInfo(int carrierAccountId)
        {
            var dealDefinition = GetLastDealByCarrierAccountId(carrierAccountId);
            if (dealDefinition == null)
                return null;
            var dealInfo = new DealInfo
            {
                DealId = dealDefinition.DealId,
                DealName = dealDefinition.Name,
                IsSwapDeal = false
            };
            if (dealDefinition.Settings is SwapDealSettings)
                dealInfo.IsSwapDeal = true;
            return dealInfo;
        }
        public List<SwapDealProgressDetail> GetLastSwapDealProgress(int dealId)
        {
            var swapDealProgressDetails = new List<SwapDealProgressDetail>();
            var dealDefinitionManager = new DealDefinitionManager();

            DealDefinition dealDefinition = dealDefinitionManager.GetDealDefinition(dealId);
            SwapDealSettings swapDealSettings = dealDefinition.Settings as SwapDealSettings;
            swapDealSettings.ThrowIfNull("swapDealSettings");

            foreach (var inbound in swapDealSettings.Inbounds)
            {
                swapDealProgressDetails.Add(new SwapDealProgressDetail
                {
                    ZoneGroupNumber = inbound.ZoneGroupNumber,
                    GroupName = inbound.Name,
                    Reached = 0,
                    PercentageCompleted = 0,
                    IsSale = true
                });
            }
            foreach (var outbound in swapDealSettings.Outbounds)
            {
                swapDealProgressDetails.Add(new SwapDealProgressDetail
                {
                    ZoneGroupNumber = outbound.ZoneGroupNumber,
                    GroupName = outbound.Name,
                    Reached = 0,
                    PercentageCompleted = 0,
                    IsSale = false
                });
            }

            var dealProgressManager = new DealProgressManager();
            var dealProgresses = dealProgressManager.GetDealProgessesByDealId(dealId);

            if (dealProgresses == null)
                return swapDealProgressDetails;

            foreach (var progress in dealProgresses)
            {
                decimal reachedProgress = progress.ReachedDurationInSeconds.HasValue ? progress.ReachedDurationInSeconds.Value : 0;

                //In case the TargetDurationInSeconds has not a value (null) that means deal is in extra rate phase so target is completed
                decimal percentageCompleted = progress.TargetDurationInSeconds.HasValue ? (reachedProgress * 100) / progress.TargetDurationInSeconds.Value : 100;

                var swapDealProgressDetail = swapDealProgressDetails.Find(x => x.ZoneGroupNumber == progress.ZoneGroupNb);
                swapDealProgressDetail.ThrowIfNull("swapDealProgressDetail");
                swapDealProgressDetail.Reached = progress.ReachedDurationInSeconds.HasValue ? progress.ReachedDurationInSeconds.Value / 60 : 0;
                swapDealProgressDetail.PercentageCompleted = percentageCompleted;
            }
            return swapDealProgressDetails;
        }

        public List<VolumeCommitmentProgressDetail> GetLastVolumeCommitmentProgress(int dealId)
        {
            var volumeCommitmentProgressDetails = new List<VolumeCommitmentProgressDetail>();
            var dealProgressManager = new DealProgressManager();
            var dealDefinitionManager = new DealDefinitionManager();

            DealDefinition dealDefinition = dealDefinitionManager.GetDealDefinition(dealId);
            VolCommitmentDealSettings volCommitmentSettings = dealDefinition.Settings as VolCommitmentDealSettings;
            volCommitmentSettings.ThrowIfNull("volCommitmentSettings");

            foreach (var volumeItem in volCommitmentSettings.Items)
            {
                volumeCommitmentProgressDetails.Add(new VolumeCommitmentProgressDetail()
                {
                    ZoneGroupNumber = volumeItem.ZoneGroupNumber,
                    GroupName = volumeItem.Name,
                    Reached = 0,
                    LastTierReached = 1,
                    IsSale = volCommitmentSettings.DealType == VolCommitmentDealType.Sell
                });
            }

            List<DealProgress> dealProgresses = dealProgressManager.GetDealProgessesByDealId(dealId);
            if (dealProgresses == null)
                return volumeCommitmentProgressDetails;

            foreach (var progress in dealProgresses)
            {
                decimal reachedProgress = progress.ReachedDurationInSeconds.HasValue ? progress.ReachedDurationInSeconds.Value : 0;
                var volCommitmentProgressDetail = volumeCommitmentProgressDetails.Find(x => x.ZoneGroupNumber == progress.ZoneGroupNb);
                volCommitmentProgressDetail.ThrowIfNull("swapDealProgressDetail");
                volCommitmentProgressDetail.Reached = progress.ReachedDurationInSeconds.HasValue ? progress.ReachedDurationInSeconds.Value / 60 : 0;
                volCommitmentProgressDetail.LastTierReached = progress.CurrentTierNb;
            }
            return volumeCommitmentProgressDetails;
        }
        public DealSaleZoneGroupWithoutRate GetAccountSaleZoneGroup(int? customerId, long? saleZoneId, DateTime effectiveDate)
        {
            if (!customerId.HasValue || !saleZoneId.HasValue)
                return null;

            var cachedAccountSaleZoneGroups = GetCachedAccountSaleZoneGroups();
            IOrderedEnumerable<DealSaleZoneGroupWithoutRate> dealSaleZoneGroups = cachedAccountSaleZoneGroups.GetRecord(new AccountZoneGroup { AccountId = customerId.Value, ZoneId = saleZoneId.Value });
            if (dealSaleZoneGroups != null && dealSaleZoneGroups.Count() > 0)
            {
                foreach (DealSaleZoneGroupWithoutRate dealSaleZoneGroup in dealSaleZoneGroups)
                {
                    if (!dealSaleZoneGroup.IsEffective(effectiveDate))
                        continue;

                    if (dealSaleZoneGroup.Zones != null && dealSaleZoneGroup.Zones.Any(item => item.ZoneId == saleZoneId && item.IsEffective(effectiveDate)))
                        return dealSaleZoneGroup;
                }
            }
            return null;
        }

        public DealSubstituteRate GetSubstitueRate(int dealId, int dealZoneGroupNb, bool isSale, Dictionary<DealZoneGroup, DealProgress> dealProgresses, out bool isDealZoneGroupCompleted)
        {
            isDealZoneGroupCompleted = true;
            int? tierNb = this.GetCurrentDealZoneGroupTierNb(dealId, dealZoneGroupNb, isSale, dealProgresses);
            if (!tierNb.HasValue)
                return null;

            isDealZoneGroupCompleted = false;
            Dictionary<DealZoneGroupTierNb, DealSubstituteRate> substituteRatesByDealZoneGroupTierNb = this.GetSubstituteRateByDealZoneGroupTierNb(dealId, isSale);
            if (substituteRatesByDealZoneGroupTierNb == null)
                return null;

            DealZoneGroupTierNb dealZoneGroupTierNb = new DealZoneGroupTierNb() { DealZoneGroupNb = dealZoneGroupNb, TierNb = tierNb.Value };

            DealSubstituteRate substituteRate;
            if (substituteRatesByDealZoneGroupTierNb.TryGetValue(dealZoneGroupTierNb, out substituteRate))
                return substituteRate;

            return null;
        }

        public DealSupplierZoneGroupWithoutRate GetAccountSupplierZoneGroup(int? supplierId, long? supplierZoneId, DateTime effectiveDate)
        {
            if (!supplierId.HasValue || !supplierZoneId.HasValue)
                return null;

            var cachedAccountSupplierZoneGroups = GetCachedAccountSupplierZoneGroups();
            IOrderedEnumerable<DealSupplierZoneGroupWithoutRate> dealSupplierZoneGroups = cachedAccountSupplierZoneGroups.GetRecord(new AccountZoneGroup { AccountId = supplierId.Value, ZoneId = supplierZoneId.Value });
            if (dealSupplierZoneGroups != null && dealSupplierZoneGroups.Count() > 0)
            {
                foreach (DealSupplierZoneGroupWithoutRate dealSupplierZoneGroup in dealSupplierZoneGroups)
                {
                    if (!dealSupplierZoneGroup.IsEffective(effectiveDate))
                        continue;

                    if (dealSupplierZoneGroup.Zones != null && dealSupplierZoneGroup.Zones.Any(item => item.ZoneId == supplierZoneId && item.IsEffective(effectiveDate)))
                        return dealSupplierZoneGroup;
                }
            }
            return null;
        }

        public DealZoneGroupTierDetailsWithoutRate GetDealZoneGroupTierDetailsWithoutRate(bool isSale, int dealId, int zoneGroupNb, int tierNb)
        {
            return isSale
                ? GetSaleDealZoneGroupTierDetailsWithoutRate(dealId, zoneGroupNb, tierNb)
                : GetSupplierDealZoneGroupTierDetailsWithoutRate(dealId, zoneGroupNb, tierNb);
        }

        public DealZoneGroupTierDetailsWithoutRate GetSaleDealZoneGroupTierDetailsWithoutRate(int dealId, int zoneGroupNb, int tierNb)
        {
            DealSaleZoneGroupWithoutRate dealSaleZoneGroup = GetDealSaleZoneGroup(dealId, zoneGroupNb);
            if (dealSaleZoneGroup == null || dealSaleZoneGroup.Tiers == null)
                return null;

            DealSaleZoneGroupTierWithoutRate dealZoneGroupTier = dealSaleZoneGroup.Tiers.FirstOrDefault(itm => itm.TierNumber == tierNb);
            if (dealZoneGroupTier == null)
                return null;

            return BuildDealZoneGroupTierDetailsWithoutRate(dealZoneGroupTier);
        }

        public bool IsZoneExcluded(long zoneId, DateTime dealBED, DateTime? dealEED, int carrierAccountId, int? dealId, bool isSale)
        {
            Dictionary<int, DealZoneInfoByZoneId> cachedDealInfo = isSale
                ? GetCachedCustomerDealZoneInfoByCustomerId()
                : GetCachedSupplierDealZoneInfoBySupplierId();

            if (cachedDealInfo == null)
                return false;

            DealZoneInfoByZoneId dealZoneInfo = cachedDealInfo.GetRecord(carrierAccountId);
            if (dealZoneInfo != null)
            {
                var zoneInfos = dealZoneInfo.GetRecord(zoneId);
                if (zoneInfos != null)
                {
                    foreach (var zoneInfo in zoneInfos)
                    {
                        if ((!dealId.HasValue || dealId.Value != zoneInfo.Value.DealId) && Utilities.AreTimePeriodsOverlapped(dealBED, dealEED, zoneInfo.Value.BED, zoneInfo.Value.EED))
                            return true;
                    }
                }
            }
            else
            {
                if (isSale)
                {
                    var saleZone = new SaleZoneManager().GetSaleZone(zoneId);
                    if (saleZone == null)
                        throw new NullReferenceException("saleZone");

                    if (dealBED < saleZone.BED && saleZone.EED.VRGreaterThan(dealEED) || saleZone.EED.HasValue && dealBED > saleZone.BED && saleZone.EED.VRLessThan(dealEED))
                        return true;
                }
                else
                {
                    var supplierZone = new SupplierZoneManager().GetSupplierZone(zoneId);
                    if (supplierZone == null)
                        throw new NullReferenceException("supplierZone");
                    if (supplierZone.BED > dealBED || supplierZone.EED.VRLessThan(dealEED))
                        return true;
                }
            }
            return false;
        }

        public List<long> AreZonesExcluded(OverlappedZonesContext context, bool isSale)
        {
            var excludedSaleZones = new List<long>();
            if (context.ZoneIds != null)
                foreach (var saleZoneId in context.ZoneIds)
                {
                    if (IsZoneExcluded(saleZoneId, context.BED, context.EED, context.CarrierAccountId, context.DealId, isSale))
                        excludedSaleZones.Add(saleZoneId);
                }
            return excludedSaleZones;
        }

        public List<long> GetExcludedSaleZones(int? dealId, int carrierAccountId, List<long> saleZoneIds, DateTime beginDate, DateTime? endDate)
        {
            var overlappedSaleZonesContext = new OverlappedZonesContext
            {
                ZoneIds = saleZoneIds,
                BED = beginDate,
                EED = endDate,
                CarrierAccountId = carrierAccountId,
                DealId = dealId,
            };
            return AreZonesExcluded(overlappedSaleZonesContext, true);
        }

        public List<long> GetExcludedSupplierZones(int? dealId, int carrierAccountId, List<long> supplierZoneIds, DateTime beginDate, DateTime? endDate)
        {
            var overlappedSupplierZonesContext = new OverlappedZonesContext
            {
                ZoneIds = supplierZoneIds,
                BED = beginDate,
                EED = endDate,
                CarrierAccountId = carrierAccountId,
                DealId = dealId,
            };
            return AreZonesExcluded(overlappedSupplierZonesContext, false);
        }

        public DealZoneGroupTierDetailsWithoutRate GetSupplierDealZoneGroupTierDetailsWithoutRate(int dealId, int zoneGroupNb, int tierNb)
        {
            DealSupplierZoneGroupWithoutRate dealSupplierZoneGroup = GetDealSupplierZoneGroupWithoutRate(dealId, zoneGroupNb);
            if (dealSupplierZoneGroup == null || dealSupplierZoneGroup.Tiers == null)
                return null;

            DealSupplierZoneGroupTierWithoutRate dealZoneGroupTier = dealSupplierZoneGroup.Tiers.Where(itm => itm.TierNumber == tierNb).FirstOrDefault();
            if (dealZoneGroupTier == null)
                return null;

            return BuildDealZoneGroupTierDetailsWithoutRate(dealZoneGroupTier);
        }

        public bool IsDealZoneGroupTierAvailable(int dealId, int zoneGroupNb, int tierNb)
        {
            DealSaleZoneGroupWithoutRate dealSaleZoneGroup = GetDealSaleZoneGroup(dealId, zoneGroupNb);
            if (dealSaleZoneGroup == null || dealSaleZoneGroup.Tiers == null)
                return false;

            DealSaleZoneGroupTierWithoutRate dealZoneGroupTier = dealSaleZoneGroup.Tiers.Where(itm => itm.TierNumber == tierNb).FirstOrDefault();
            if (dealZoneGroupTier == null)
                return false;

            return true;
        }

        public DealZoneGroupTierDetails GetSaleDealZoneGroupTierDetails(int dealId, int zoneGroupNb, int tierNb, long zoneId, DateTime effectiveTime)
        {
            DealSaleZoneGroupWithoutRate dealSaleZoneGroup = GetDealSaleZoneGroup(dealId, zoneGroupNb);
            if (dealSaleZoneGroup == null || dealSaleZoneGroup.Tiers == null)
                return null;

            DealSaleZoneGroupTierWithoutRate dealZoneGroupTier = dealSaleZoneGroup.Tiers.Where(itm => itm.TierNumber == tierNb).FirstOrDefault();
            if (dealZoneGroupTier == null)
                return null;

            var dealZoneRate = new DealZoneRateManager().GetDealZoneRate(dealId, zoneGroupNb, zoneId, tierNb, true, effectiveTime);
            if (dealZoneRate == null)
                return null;

            return BuildDealZoneGroupTierDetails(dealZoneGroupTier, dealZoneRate.Rate, dealZoneRate.CurrencyId);
        }

        public DealZoneGroupTierDetails GetSupplierDealZoneGroupTierDetails(int dealId, int zoneGroupNb, int tierNb, long zoneId, DateTime effectiveTime)
        {
            DealSupplierZoneGroupWithoutRate dealSupplierZoneGroup = GetDealSupplierZoneGroupWithoutRate(dealId, zoneGroupNb);
            if (dealSupplierZoneGroup == null || dealSupplierZoneGroup.Tiers == null)
                return null;

            DealSupplierZoneGroupTierWithoutRate dealZoneGroupTier = dealSupplierZoneGroup.Tiers.Where(itm => itm.TierNumber == tierNb).FirstOrDefault();
            if (dealZoneGroupTier == null)
                return null;

            var dealZoneRate = new DealZoneRateManager().GetDealZoneRate(dealId, zoneGroupNb, zoneId, tierNb, false, effectiveTime);
            if (dealZoneRate == null)
                return null;

            return BuildDealZoneGroupTierDetails(dealZoneGroupTier, dealZoneRate.Rate, dealZoneRate.CurrencyId);
        }

        public override BaseDealManager.BaseDealLoggableEntity GetLoggableEntity()
        {
            throw new NotImplementedException();
        }

        public Dictionary<int, DealDefinition> GetAllCachedDealDefinitions(bool getDeletedDeals = false)
        {
            return !getDeletedDeals ? GetCachedDeals() : GetCachedDealsWithDeleted();
        }

        public IEnumerable<DealSaleZoneGroup> GetDealSaleZoneGroupsByDealDefinitions(IEnumerable<DealDefinition> dealDefinitions, bool skipDeleted = true)
        {
            if (dealDefinitions == null)
                return null;

            var result = new List<DealSaleZoneGroup>();

            foreach (DealDefinition dealDefinition in dealDefinitions)
            {
                if (dealDefinition.IsDeleted && skipDeleted)
                    continue;

                DealGetZoneGroupsContext context = new DealGetZoneGroupsContext(dealDefinition.DealId, DealZoneGroupPart.Sale, true);
                dealDefinition.Settings.GetZoneGroups(context);
                if (context.SaleZoneGroups != null && context.SaleZoneGroups.Any())
                {
                    foreach (var baseDealSaleZoneGroup in context.SaleZoneGroups)
                    {
                        var dealSaleZoneGroup = baseDealSaleZoneGroup as DealSaleZoneGroup;
                        if (dealSaleZoneGroup == null)
                            throw new Exception($"baseDealSaleZoneGroup should be of type 'Deal.Entities.DealSaleZoneGroup' and not of type '{baseDealSaleZoneGroup.GetType()}'");

                        result.Add(dealSaleZoneGroup);
                    }
                }
            }
            return result.Count > 0 ? result : null;
        }
        public bool IsRateChangeDifferentThanDealZoneGroupRate(DealSaleZoneGroup dealSaleZoneGroup, decimal newRate)
        {
            return false;
        }

        public IEnumerable<DealSupplierZoneGroup> GetDealSupplierZoneGroupsByDealDefinitions(IEnumerable<DealDefinition> dealDefinitions, bool skipDeleted = true)
        {
            if (dealDefinitions == null)
                return null;

            var result = new List<DealSupplierZoneGroup>();

            foreach (DealDefinition dealDefinition in dealDefinitions)
            {
                if (dealDefinition.IsDeleted && skipDeleted)
                    continue;

                DealGetZoneGroupsContext context = new DealGetZoneGroupsContext(dealDefinition.DealId, DealZoneGroupPart.Cost, true);
                dealDefinition.Settings.GetZoneGroups(context);
                if (context.SupplierZoneGroups != null && context.SupplierZoneGroups.Any())
                {
                    foreach (var baseDealSupplierZoneGroups in context.SupplierZoneGroups)
                    {
                        var dealSupplierZoneGroup = baseDealSupplierZoneGroups as DealSupplierZoneGroup;
                        if (dealSupplierZoneGroup == null)
                            throw new Exception($"baseDealSupplierZoneGroups should be of type 'Deal.Entities.DealSupplierZoneGroup' and not of type '{baseDealSupplierZoneGroups.GetType()}'");

                        result.Add(dealSupplierZoneGroup);
                    }
                }
            }
            return result.Count > 0 ? result : null;
        }

        public DealDefinition GetDealDefinition(int dealId, bool getDeletedDeals = false)
        {
            var cachedDealInfos = GetAllCachedDealDefinitions(getDeletedDeals);
            return cachedDealInfos.GetRecord(dealId);
        }

        public string GetDealSaleZoneGroupName(int dealId, int groupNumber)
        {
            var dealDefinition = GetDealDefinition(dealId);
            if (dealDefinition == null)
                throw new DataIntegrityValidationException(string.Format("Cannot find the deal with Id {0}", dealId));
            return dealDefinition.Settings.GetSaleZoneGroupName(groupNumber);
        }

        public string GetDealCostZoneGroupName(int dealId, int groupNumber)
        {
            var dealDefinition = GetDealDefinition(dealId);
            if (dealDefinition == null)
                throw new DataIntegrityValidationException(string.Format("Cannot find the deal with Id {0}", dealId));
            return dealDefinition.Settings.GetSupplierZoneGroupName(groupNumber);
        }

        public List<DealDefinition> GetRecurredDeals(DealDefinition deal, int recurringNumber, RecurringType recurringType)
        {
            var recurredDeals = new List<DealDefinition>();
            if (!deal.Settings.EEDToStore.HasValue)//we cannot reccur a deal without EED
                return recurredDeals;

            DateTime EEDToStore = deal.Settings.EEDToStore.Value;
            DateTime beginDealDate = deal.Settings.BeginDate;
            var dealLifeSpan = EEDToStore.Subtract(beginDealDate);
            TimeSpan offSet = deal.Settings.OffSet ?? new TimeSpan();
            DateTime endDealDate = deal.Settings.RealEED.Value.Add(offSet);
            var monthsDifference = (endDealDate.Year - beginDealDate.Year) * 12 + (endDealDate.Month - beginDealDate.Month);
            bool isLastDayOfMonth = EEDToStore.GetLastDayOfMonth().Day == EEDToStore.Day;

            for (int i = 0; i < recurringNumber; i++)
            {
                DealDefinition recurredDeal = new DealDefinition();
                recurredDeal.Settings = deal.Settings.VRDeepCopy();
                switch (recurringType)
                {
                    case RecurringType.Daily:
                        recurredDeal.Settings.BeginDate = endDealDate;
                        recurredDeal.Settings.EEDToStore = recurredDeal.Settings.BeginDate.Add(dealLifeSpan);
                        endDealDate = recurredDeal.Settings.RealEED.Value.Add(offSet);
                        break;

                    case RecurringType.Monthly:
                        int monthsToAdd = (monthsDifference + 1) * (i + 1);
                        recurredDeal.Settings.BeginDate = deal.Settings.BeginDate.AddMonths(monthsToAdd);
                        recurredDeal.Settings.EEDToStore = isLastDayOfMonth
                                                            ? deal.Settings.EEDToStore.Value.AddMonths(monthsToAdd).GetLastDayOfMonth()
                                                            : deal.Settings.EEDToStore.Value.AddMonths(monthsToAdd);
                        break;
                }

                string dateFormat = Utilities.GetDateTimeFormat(DateTimeType.Date);
                recurredDeal.Name = $"{deal.Name}-({recurredDeal.Settings.BeginDate.ToString(dateFormat)}-{recurredDeal.Settings.EEDToStore.Value.ToString(dateFormat)})";
                recurredDeal.Settings.IsRecurrable = false;
                recurredDeals.Add(recurredDeal);
            }
            recurredDeals.Last().Settings.IsRecurrable = true;
            return recurredDeals;
        }

        public List<string> ValidatingDeals(List<DealDefinition> recurredDeals)
        {
            var errorMessages = new List<string>();
            foreach (var recurredDeal in recurredDeals)
            {
                ValidateBeforeSaveContext contextDeal = new ValidateBeforeSaveContext()
                {
                    DealSaleZoneIds = recurredDeal.Settings.GetDealSaleZoneIds(),
                    DealSupplierZoneIds = recurredDeal.Settings.GetDealSupplierZoneIds(),
                    BED = recurredDeal.Settings.BEDToDisplay,
                    EED = recurredDeal.Settings.EEDToDisplay,
                    DealId = recurredDeal.DealId,
                    CustomerId = recurredDeal.Settings.GetCarrierAccountId(),
                };

                if (!recurredDeal.Settings.ValidateDataBeforeSave(contextDeal))
                {
                    foreach (var message in contextDeal.ValidateMessages)
                    {
                        errorMessages.Add($"{message}. Corresponding Deal: {recurredDeal.Name}");
                    }
                }
            }
            return errorMessages;
        }

        public bool IsOverlapped(DealZoneInfo dealZoneInfo, DateTime beginEffectiveDate, DateTime? endEffectiveDate)
        {
            return endEffectiveDate.VRGreaterThan(dealZoneInfo.BED) && dealZoneInfo.EED > beginEffectiveDate;
        }

        public DealSettingsDetail GetDealSettingsDetail(int dealId)
        {
            var deal = this.GetDeal(dealId);
            deal.ThrowIfNull("Deal", dealId);

            var settings = deal.Settings;
            settings.ThrowIfNull("Deal Settings", dealId);
            var carrierAccountId = settings.GetCarrierAccountId();

            var sellingNumberPlanId = new CarrierAccountManager().GetSellingNumberPlanId(carrierAccountId);

            return new DealSettingsDetail
            {
                DealId = dealId,
                CarrierAccountId = carrierAccountId,
                SellingNumberPlanId = sellingNumberPlanId,
                SaleZoneIds = settings.GetDealSaleZoneIds(),
                SupplierZoneIds = settings.GetDealSupplierZoneIds(),
                BED = settings.BEDToDisplay,
                EED = settings.EEDToDisplay
            };
        }

        public int GetDealPriority(int dealId)
        {
            DealDefinition dealDefinition = this.GetDeal(dealId);
            dealDefinition.ThrowIfNull("dealDefinition", dealId);
            dealDefinition.Settings.ThrowIfNull("dealDefinition.Settings", dealId);
            return dealDefinition.Settings.Priority;
        }

        #endregion

        #region Private Methods

        private DealZoneGroupTierDetails BuildDealZoneGroupTierDetails(DealSaleZoneGroupTierWithoutRate dealSaleZoneGroupTier, decimal rate, int currencyId)
        {
            return new DealZoneGroupTierDetails()
            {
                TierNb = dealSaleZoneGroupTier.TierNumber,
                VolumeInSeconds = dealSaleZoneGroupTier.VolumeInSeconds,
                CurrencyId = currencyId,
                Rate = rate,
                RetroActiveFromTierNb = dealSaleZoneGroupTier.RetroActiveFromTierNumber
            };
        }

        private DealZoneGroupTierDetails BuildDealZoneGroupTierDetails(DealSupplierZoneGroupTierWithoutRate dealSupplierZoneGroupTier, decimal rate, int currencyId)
        {
            return new DealZoneGroupTierDetails()
            {
                TierNb = dealSupplierZoneGroupTier.TierNumber,
                VolumeInSeconds = dealSupplierZoneGroupTier.VolumeInSeconds,
                CurrencyId = currencyId,
                Rate = rate,
                RetroActiveFromTierNb = dealSupplierZoneGroupTier.RetroActiveFromTierNumber
            };
        }

        private DealZoneGroupTierDetailsWithoutRate BuildDealZoneGroupTierDetailsWithoutRate(DealSaleZoneGroupTierWithoutRate dealSaleZoneGroupTier)
        {
            return new DealZoneGroupTierDetailsWithoutRate()
            {
                TierNb = dealSaleZoneGroupTier.TierNumber,
                VolumeInSeconds = dealSaleZoneGroupTier.VolumeInSeconds,
                RetroActiveFromTierNb = dealSaleZoneGroupTier.RetroActiveFromTierNumber
            };
        }

        private int? GetCurrentDealZoneGroupTierNb(int dealId, int dealZoneGroupNb, bool isSale, Dictionary<DealZoneGroup, DealProgress> dealProgresses)
        {
            int? tierNb = null;

            DealZoneGroup dealZoneGroup = new DealZoneGroup() { DealId = dealId, ZoneGroupNb = dealZoneGroupNb };

            DealProgress dealProgress;
            if (dealProgresses == null || !dealProgresses.TryGetValue(dealZoneGroup, out dealProgress))
            {
                if (isSale)
                {
                    DealSaleZoneGroupWithoutRate dealSaleZoneGroup = GetDealSaleZoneGroup(dealZoneGroup.DealId, dealZoneGroup.ZoneGroupNb);
                    var dealSaleZoneGroupTierWithoutRate = dealSaleZoneGroup.Tiers;
                    if (dealSaleZoneGroupTierWithoutRate == null || dealSaleZoneGroupTierWithoutRate.Count() == 0)
                        throw new NullReferenceException(string.Format("One Tier should be defined at least for DealId: {0}, DealZoneGroupNb: {1}", dealZoneGroup.DealId, dealZoneGroup.ZoneGroupNb));

                    tierNb = dealSaleZoneGroup.Tiers.First().TierNumber;
                }
                else
                {
                    DealSupplierZoneGroupWithoutRate dealSupplierZoneGroup = GetDealSupplierZoneGroupWithoutRate(dealZoneGroup.DealId, dealZoneGroup.ZoneGroupNb);
                    var dealSupplierZoneGroupTierWithoutRate = dealSupplierZoneGroup.Tiers;
                    if (dealSupplierZoneGroupTierWithoutRate == null || dealSupplierZoneGroupTierWithoutRate.Count() == 0)
                        throw new NullReferenceException(string.Format("One Tier should be defined at least for DealId: {0}, DealZoneGroupNb: {1}", dealZoneGroup.DealId, dealZoneGroup.ZoneGroupNb));

                    tierNb = dealSupplierZoneGroup.Tiers.First().TierNumber;
                }
            }
            else
            {
                int currentTierNb = dealProgress.CurrentTierNb;
                if (!dealProgress.IsComplete)
                {
                    tierNb = currentTierNb;
                }
                else
                {
                    int nextTierNb = currentTierNb + 1;
                    if (IsDealZoneGroupTierAvailable(dealZoneGroup.DealId, dealZoneGroup.ZoneGroupNb, nextTierNb))
                        tierNb = nextTierNb;
                }
            }

            return tierNb;
        }

        private bool CheckDealStatus(DealStatus dealStatus, DateTime? deActivationDate, DateTime effectiveDate)
        {
            switch (dealStatus)
            {
                case DealStatus.Draft: return false;

                case DealStatus.Active: return true;

                case DealStatus.Inactive:
                    if (!deActivationDate.HasValue)
                        throw new NullReferenceException("deActivationDate");

                    if (effectiveDate < deActivationDate.Value)
                        return true;

                    return false;

                default: throw new NotSupportedException(string.Format("dealStatus '{0}' is not supported", dealStatus));
            }
        }

        private Dictionary<int, DealZoneInfoByZoneId> GetCachedCustomerDealZoneInfoByCustomerId()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetCachedCustomerDealZoneInfoByZoneId", () =>
            {
                var customerDealZoneInfo = new Dictionary<int, DealZoneInfoByZoneId>();
                var cachedDeals = GetCachedDeals();
                var carrierAccountManager = new CarrierAccountManager();

                foreach (var dealDef in cachedDeals.Values)
                {
                    var zoneIds = dealDef.Settings.GetDealSaleZoneIds();
                    if (zoneIds != null)
                    {
                        var carrierAccountId = dealDef.Settings.GetCarrierAccountId();
                        var carrierAccount = carrierAccountManager.GetCarrierAccount(carrierAccountId);

                        if (CarrierAccountManager.IsCustomer(carrierAccount.AccountType))
                        {
                            DealZoneInfoByZoneId dealZoneInfoByZoneId = customerDealZoneInfo.GetOrCreateItem(carrierAccountId);
                            foreach (var zoneId in zoneIds)
                            {
                                SortedList<DateTime, DealZoneInfo> dealZoneInfos = dealZoneInfoByZoneId.GetOrCreateItem(zoneId, () => { return new SortedList<DateTime, DealZoneInfo>(new DecsendingDateTimeComparer()); });

                                var overlappedItem = dealZoneInfos.FindRecord(x => IsOverlapped(x, dealDef.Settings.RealBED, dealDef.Settings.RealEED));
                                if (overlappedItem == null)
                                {
                                    dealZoneInfos.Add(dealDef.Settings.RealBED, new DealZoneInfo
                                    {
                                        DealId = dealDef.DealId,
                                        ZoneId = zoneId,
                                        BED = dealDef.Settings.RealBED,
                                        EED = dealDef.Settings.RealEED,
                                        DealStatus = dealDef.Settings.Status
                                    });
                                }
                            }
                        }
                    }
                }
                return customerDealZoneInfo;
            });
        }

        private Dictionary<int, DealZoneInfoByZoneId> GetCachedSupplierDealZoneInfoBySupplierId()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetCachedSupplierDealZoneInfoByZoneId", () =>
            {
                var supplierDealZoneInfo = new Dictionary<int, DealZoneInfoByZoneId>();
                var cachedDeals = GetCachedDealsByConfigId();
                var carrierAccountManager = new CarrierAccountManager();
                IEnumerable<DealDefinition> dealDefinitions = cachedDeals.Values.SelectMany(item => item);

                foreach (var deal in dealDefinitions)
                {
                    var zoneIds = deal.Settings.GetDealSupplierZoneIds();
                    if (zoneIds != null)
                    {
                        var carrierAccountId = deal.Settings.GetCarrierAccountId();
                        var carrierAccount = carrierAccountManager.GetCarrierAccount(carrierAccountId);

                        if (carrierAccount.AccountType == CarrierAccountType.Supplier || carrierAccount.AccountType == CarrierAccountType.Exchange)
                        {
                            var dealZoneInfoByZoneId = supplierDealZoneInfo.GetOrCreateItem(carrierAccountId);

                            foreach (var zoneId in zoneIds)
                            {
                                SortedList<DateTime, DealZoneInfo> dealZoneInfos = dealZoneInfoByZoneId.GetOrCreateItem(zoneId, () => { return new SortedList<DateTime, DealZoneInfo>(new DecsendingDateTimeComparer()); });
                                var overlappedItem = dealZoneInfos.FindRecord(x => IsOverlapped(x, deal.Settings.RealBED, deal.Settings.RealEED));
                                if (overlappedItem == null)
                                {
                                    dealZoneInfos.Add(deal.Settings.RealBED, new DealZoneInfo
                                    {
                                        DealId = deal.DealId,
                                        ZoneId = zoneId,
                                        BED = deal.Settings.RealBED,
                                        EED = deal.Settings.RealEED,
                                        DealStatus = deal.Settings.Status
                                    });
                                }
                            }
                        }
                    }
                }
                return supplierDealZoneInfo;
            });
        }

        private DealZoneGroupTierDetailsWithoutRate BuildDealZoneGroupTierDetailsWithoutRate(DealSupplierZoneGroupTierWithoutRate dealSupplierZoneGroupTier)
        {
            return new DealZoneGroupTierDetailsWithoutRate()
            {
                TierNb = dealSupplierZoneGroupTier.TierNumber,
                VolumeInSeconds = dealSupplierZoneGroupTier.VolumeInSeconds,
                RetroActiveFromTierNb = dealSupplierZoneGroupTier.RetroActiveFromTierNumber
            };
        }

        private DealSaleZoneGroupWithoutRate GetDealSaleZoneGroup(int dealId, int zoneGroupNb)
        {
            var cachedDealSaleZoneGroups = GetCachedDealSaleZoneGroupsWithoutRate();
            return cachedDealSaleZoneGroups.GetRecord(new DealZoneGroup() { DealId = dealId, ZoneGroupNb = zoneGroupNb });
        }

        private DealSupplierZoneGroupWithoutRate GetDealSupplierZoneGroupWithoutRate(int dealId, int zoneGroupNb)
        {
            var cachedDealSupplierZoneGroups = GetCachedDealSupplierZoneGroupsWithoutRate();
            return cachedDealSupplierZoneGroups.GetRecord(new DealZoneGroup() { DealId = dealId, ZoneGroupNb = zoneGroupNb });
        }

        private Dictionary<DealZoneGroup, DealSaleZoneGroupWithoutRate> GetCachedDealSaleZoneGroupsWithoutRate()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetCachedDealSaleZoneGroups", () =>
            {
                Dictionary<DealZoneGroup, DealSaleZoneGroupWithoutRate> result = new Dictionary<DealZoneGroup, DealSaleZoneGroupWithoutRate>();
                var cachedDeals = GetCachedDeals();
                foreach (DealDefinition dealDefinition in cachedDeals.Values)
                {
                    DealGetZoneGroupsContext context = new DealGetZoneGroupsContext(dealDefinition.DealId, DealZoneGroupPart.Sale, false);
                    dealDefinition.Settings.GetZoneGroups(context);
                    if (context.SaleZoneGroups != null)
                    {
                        foreach (var baseDealSaleZoneGroup in context.SaleZoneGroups)
                        {
                            var dealSaleZoneGroupWithoutRate = baseDealSaleZoneGroup as DealSaleZoneGroupWithoutRate;
                            if (dealSaleZoneGroupWithoutRate == null)
                                throw new Exception(String.Format("baseDealSaleZoneGroup should be of type 'Deal.Entities.DealSaleZoneGroupWithoutRate' and not of type '{0}'", baseDealSaleZoneGroup.GetType()));

                            result.Add(new DealZoneGroup()
                            {
                                DealId = dealDefinition.DealId,
                                ZoneGroupNb = dealSaleZoneGroupWithoutRate.DealSaleZoneGroupNb
                            }
                                , dealSaleZoneGroupWithoutRate);
                        }
                    }
                }
                return result;
            });
        }

        private Dictionary<DealZoneGroup, DealSupplierZoneGroupWithoutRate> GetCachedDealSupplierZoneGroupsWithoutRate()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetCachedDealSupplierZoneGroups", () =>
            {
                Dictionary<DealZoneGroup, DealSupplierZoneGroupWithoutRate> result = new Dictionary<DealZoneGroup, DealSupplierZoneGroupWithoutRate>();
                var cachedDeals = GetCachedDeals();
                foreach (DealDefinition dealDefinition in cachedDeals.Values)
                {
                    DealGetZoneGroupsContext context = new DealGetZoneGroupsContext(dealDefinition.DealId, DealZoneGroupPart.Cost, false);
                    dealDefinition.Settings.GetZoneGroups(context);
                    if (context.SupplierZoneGroups != null)
                    {
                        foreach (var baseDealSupplierZoneGroup in context.SupplierZoneGroups)
                        {
                            var dealSupplierZoneGroupWithoutRate = baseDealSupplierZoneGroup as DealSupplierZoneGroupWithoutRate;
                            if (dealSupplierZoneGroupWithoutRate == null)
                                throw new Exception(String.Format("baseDealSupplierZoneGroup should be of type 'Deal.Entities.DealSupplierZoneGroupWithoutRate'. and not of type '{0}'", baseDealSupplierZoneGroup.GetType()));

                            result.Add(new DealZoneGroup()
                            {
                                DealId = dealDefinition.DealId,
                                ZoneGroupNb = dealSupplierZoneGroupWithoutRate.DealSupplierZoneGroupNb
                            }
                                , dealSupplierZoneGroupWithoutRate);
                        }
                    }
                }
                return result;
            });
        }

        private Dictionary<AccountZoneGroup, IOrderedEnumerable<DealSaleZoneGroupWithoutRate>> GetCachedAccountSaleZoneGroups()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetCachedAccountSaleZoneGroups", () =>
            {
                Dictionary<AccountZoneGroup, List<DealSaleZoneGroupWithoutRate>> result = new Dictionary<AccountZoneGroup, List<DealSaleZoneGroupWithoutRate>>();
                Dictionary<DealZoneGroup, DealSaleZoneGroupWithoutRate> cachedDealZoneGroups = GetCachedDealSaleZoneGroupsWithoutRate();
                if (cachedDealZoneGroups != null)
                {
                    foreach (var dealZoneGroupKvp in cachedDealZoneGroups)
                    {
                        DealSaleZoneGroupWithoutRate dealSaleZoneGroupWithoutRate = dealZoneGroupKvp.Value;
                        if (dealSaleZoneGroupWithoutRate.Zones != null)
                        {
                            foreach (DealSaleZoneGroupZoneItem dealSaleZoneGroupZoneItem in dealSaleZoneGroupWithoutRate.Zones)
                            {
                                AccountZoneGroup accountZoneGroup = new AccountZoneGroup()
                                {
                                    AccountId = dealSaleZoneGroupWithoutRate.CustomerId,
                                    ZoneId = dealSaleZoneGroupZoneItem.ZoneId
                                };
                                List<DealSaleZoneGroupWithoutRate> dealSaleZoneGroups = result.GetOrCreateItem(accountZoneGroup);
                                dealSaleZoneGroups.Add(dealSaleZoneGroupWithoutRate);
                            }
                        }
                    }
                }

                return result.ToDictionary(itm => itm.Key, itm => itm.Value.OrderByDescending(item => item.BED));
            });
        }

        private Dictionary<AccountZoneGroup, IOrderedEnumerable<DealSupplierZoneGroupWithoutRate>> GetCachedAccountSupplierZoneGroups()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetCachedAccountSupplierZoneGroups", () =>
            {
                Dictionary<AccountZoneGroup, List<DealSupplierZoneGroupWithoutRate>> result = new Dictionary<AccountZoneGroup, List<DealSupplierZoneGroupWithoutRate>>();
                Dictionary<DealZoneGroup, DealSupplierZoneGroupWithoutRate> cachedDealZoneGroups = GetCachedDealSupplierZoneGroupsWithoutRate();
                if (cachedDealZoneGroups != null)
                {
                    foreach (var dealZoneGroupKvp in cachedDealZoneGroups)
                    {
                        DealSupplierZoneGroupWithoutRate dealSupplierZoneGroupWithoutRate = dealZoneGroupKvp.Value;
                        if (dealSupplierZoneGroupWithoutRate.Zones != null)
                        {
                            foreach (DealSupplierZoneGroupZoneItem dealSupplierZoneGroupZoneItem in dealSupplierZoneGroupWithoutRate.Zones)
                            {
                                AccountZoneGroup accountZoneGroup = new AccountZoneGroup()
                                {
                                    AccountId = dealSupplierZoneGroupWithoutRate.SupplierId,
                                    ZoneId = dealSupplierZoneGroupZoneItem.ZoneId
                                };
                                List<DealSupplierZoneGroupWithoutRate> dealSupplierZoneGroups = result.GetOrCreateItem(accountZoneGroup);
                                dealSupplierZoneGroups.Add(dealSupplierZoneGroupWithoutRate);
                            }
                        }
                    }
                }
                return result.ToDictionary(itm => itm.Key, itm => itm.Value.OrderByDescending(item => item.BED));
            });
        }

        private Dictionary<DealZoneGroupTierNb, DealSubstituteRate> GetSubstituteRateByDealZoneGroupTierNb(int dealId, bool isSale)
        {
            DealDefinition deal = GetDealDefinition(dealId);
            deal.Settings.ThrowIfNull("deal.Settings", dealId);

            DealGetRoutingZoneGroupsContext dealGetRoutingZoneGroupsContext = new DealGetRoutingZoneGroupsContext();
            dealGetRoutingZoneGroupsContext.DealZoneGroupPart = isSale ? DealZoneGroupPart.Sale : DealZoneGroupPart.Cost;
            deal.Settings.GetRoutingZoneGroups(dealGetRoutingZoneGroupsContext);

            if (isSale)
            {
                List<DealRoutingSaleZoneGroup> saleZoneGroups = dealGetRoutingZoneGroupsContext.SaleZoneGroups;
                return GetCachedSubstituteRateByDealZoneGroupTierNb(saleZoneGroups, dealId);
            }
            else
            {
                List<DealRoutingSupplierZoneGroup> supplierZoneGroups = dealGetRoutingZoneGroupsContext.SupplierZoneGroups;
                return GetCachedSubstituteRateByDealZoneGroupTierNb(supplierZoneGroups, dealId);
            }
        }

        /// <summary>
        /// This function will return null for each DealZoneGroupTierNb with NoSubstituteRate or NormalSubstituteRate 
        /// </summary>
        private Dictionary<DealZoneGroupTierNb, DealSubstituteRate> GetCachedSubstituteRateByDealZoneGroupTierNb(List<DealRoutingSaleZoneGroup> saleZoneGroups, int dealId)
        {
            string cacheName = string.Format("GetCachedDeal{0}SaleZoneGroups", dealId);
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject(cacheName, () =>
            {
                if (saleZoneGroups == null || saleZoneGroups.Count == 0)
                    return null;

                Dictionary<DealZoneGroupTierNb, DealSubstituteRate> dealRoutingSaleZoneGroups = new Dictionary<DealZoneGroupTierNb, DealSubstituteRate>();
                foreach (var saleZoneGroup in saleZoneGroups)
                {
                    var dealSaleZoneGroupTiers = saleZoneGroup.Tiers;
                    if (dealSaleZoneGroupTiers != null)
                    {
                        foreach (var dealSaleZoneGroupTier in dealSaleZoneGroupTiers)
                        {
                            if (!dealSaleZoneGroupTier.SubstituteRate.HasValue)
                                continue;

                            DealZoneGroupTierNb dealZoneGroupTierNb = new DealZoneGroupTierNb()
                            {
                                DealZoneGroupNb = saleZoneGroup.DealSaleZoneGroupNb,
                                TierNb = dealSaleZoneGroupTier.TierNumber
                            };
                            DealSubstituteRate substituteRate = new DealSubstituteRate()
                            {
                                Rate = dealSaleZoneGroupTier.SubstituteRate.Value,
                                CurrencyId = saleZoneGroup.CurrencyId
                            };
                            dealRoutingSaleZoneGroups.Add(dealZoneGroupTierNb, substituteRate);
                        }
                    }
                }
                return dealRoutingSaleZoneGroups;
            });
        }

        /// <summary>
        /// This function will return null for each DealZoneGroupTierNb with NoSubstituteRate or NormalSubstituteRate 
        /// </summary>
        private Dictionary<DealZoneGroupTierNb, DealSubstituteRate> GetCachedSubstituteRateByDealZoneGroupTierNb(List<DealRoutingSupplierZoneGroup> supplierZoneGroups, int dealId)
        {
            string cacheName = string.Format("GetCachedDeal{0}SupplierZoneGroups", dealId);
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject(cacheName, () =>
            {
                if (supplierZoneGroups == null || supplierZoneGroups.Count == 0)
                    return null;

                Dictionary<DealZoneGroupTierNb, DealSubstituteRate> dealRoutingSupplierZoneGroups = new Dictionary<DealZoneGroupTierNb, DealSubstituteRate>();
                foreach (var supplierZoneGroup in supplierZoneGroups)
                {
                    var dealSupplierZoneGroupTiers = supplierZoneGroup.Tiers;
                    if (dealSupplierZoneGroupTiers != null)
                    {
                        foreach (var dealSupplierZoneGroupTier in dealSupplierZoneGroupTiers)
                        {
                            if (!dealSupplierZoneGroupTier.SubstituteRate.HasValue)
                                continue;

                            DealZoneGroupTierNb dealZoneGroupTierNb = new DealZoneGroupTierNb()
                            {
                                DealZoneGroupNb = supplierZoneGroup.DealSupplierZoneGroupNb,
                                TierNb = dealSupplierZoneGroupTier.TierNumber
                            };
                            DealSubstituteRate substituteRate = new DealSubstituteRate()
                            {
                                Rate = dealSupplierZoneGroupTier.SubstituteRate.Value,
                                CurrencyId = supplierZoneGroup.CurrencyId
                            };
                            dealRoutingSupplierZoneGroups.Add(dealZoneGroupTierNb, substituteRate);
                        }
                    }
                }
                return dealRoutingSupplierZoneGroups;
            });
        }

        #endregion

        #region Private Classes

        private class DecsendingDateTimeComparer : IComparer<DateTime>
        {
            public int Compare(DateTime firstDate, DateTime secondDate)
            {
                return firstDate < secondDate ? 1 : -1;
            }
        }

        private struct DealZoneGroupTierNb
        {
            public int DealZoneGroupNb { get; set; }
            public int TierNb { get; set; }
        }

        #endregion

        #region Mappers

        private DealDefinitionInfo DealDefinitionInfoMapper(DealDefinition dealDefinition, List<DealStatus> dealStatuses)
        {
            int carrierAccountId = dealDefinition.Settings.GetCarrierAccountId();

            DealDefinitionInfoStatus dealDefinitionInfoStatus;

            if (!dealDefinition.IsDeleted)
            {
                switch (dealDefinition.Settings.Status)
                {
                    case DealStatus.Active: dealDefinitionInfoStatus = DealDefinitionInfoStatus.Active; break;
                    case DealStatus.Inactive: dealDefinitionInfoStatus = DealDefinitionInfoStatus.Inactive; break;
                    case DealStatus.Draft: dealDefinitionInfoStatus = DealDefinitionInfoStatus.Draft; break;
                    default: throw new NotSupportedException(string.Format("DealStatus '{0}' is not supported", dealDefinition.Settings.Status));
                }
            }
            else
            {
                dealDefinitionInfoStatus = DealDefinitionInfoStatus.Deleted;
            }

            return new DealDefinitionInfo()
            {
                DealId = dealDefinition.DealId,
                Name = dealDefinition.Name,
                ConfigId = dealDefinition.Settings.ConfigId,
                SellingNumberPlanId = new CarrierAccountManager().GetCustomerSellingNumberPlanId(carrierAccountId),
                DealDefinitionInfoStatus = dealDefinitionInfoStatus,
                IsForced = dealDefinition.IsDeleted || (dealStatuses != null && !dealStatuses.Contains(dealDefinition.Settings.Status))
            };
        }

        public override DealDefinitionDetail DealDefinitionDetailMapper(DealDefinition deal)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IBusinessEntityManager

        public override List<dynamic> GetAllEntities(IBusinessEntityGetAllContext context)
        {
            throw new NotImplementedException();
        }

        public override dynamic GetEntity(IBusinessEntityGetByIdContext context)
        {
            int dealId = Convert.ToInt32(context.EntityId);
            return GetDeal(dealId);
        }

        public override string GetEntityDescription(IBusinessEntityDescriptionContext context)
        {
            int dealId = Convert.ToInt32(context.EntityId);
            DealDefinition deal = GetDeal(dealId);
            if (deal != null)
                return deal.Name;
            return null;
        }

        public override dynamic GetEntityId(IBusinessEntityIdContext context)
        {
            var deal = context.Entity as DealDefinition;
            return deal.DealId;
        }

        public override IEnumerable<dynamic> GetIdsByParentEntityId(IBusinessEntityGetIdsByParentEntityIdContext context)
        {
            throw new NotImplementedException();
        }

        public override dynamic GetParentEntityId(IBusinessEntityGetParentEntityIdContext context)
        {
            throw new NotImplementedException();
        }

        public override bool IsCacheExpired(IBusinessEntityIsCacheExpiredContext context, ref DateTime? lastCheckTime)
        {
            throw new NotImplementedException();
        }

        public override dynamic MapEntityToInfo(IBusinessEntityMapToInfoContext context)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    public class RecurredDealItem
    {
        public DealDefinitionDetail UpdatedItem { get; set; }
        public List<DealDefinitionDetail> InsertedItems { get; set; }
    }
}