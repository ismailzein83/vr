using System;
using System.Collections.Generic;
using System.Linq;
using Vanrise.Common;
using TOne.WhS.Deal.Entities;

namespace TOne.WhS.Deal.Business
{
    public class DealDefinitionManager : BaseDealManager
    {
        #region Public Methods

        public IEnumerable<DealDefinitionInfo> GetDealDefinitionInfo(DealDefinitionFilter filter)
        {
            var cachedDeals = base.GetCachedDeals();

            Func<DealDefinition, bool> filterExpression = (dealDefinition) =>
            {
                if (filter == null)
                    return true;

                if (filter.IncludedDealDefinitionIds != null && !filter.IncludedDealDefinitionIds.Contains(dealDefinition.DealId))
                    return false;

                if (filter.ExcludedDealDefinitionIds != null && filter.ExcludedDealDefinitionIds.Contains(dealDefinition.DealId))
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

            return cachedDeals.MapRecords(DealDefinitionInfoMapper, filterExpression).OrderBy(item => item.Name);
        }

        public void FillOrigSaleValues(dynamic record)
        {
            record.OrigSaleRateId = record.SaleRateId;
            record.OrigSaleRateValue = record.SaleRateValue;
            record.OrigSaleNet = record.SaleNet;
            record.OrigSaleExtraChargeRateValue = record.SaleExtraChargeRateValue;
            record.OrigSaleExtraChargeValue = record.SaleExtraChargeValue;
            record.OrigSaleDurationInSeconds = record.SaleDurationInSeconds;

            DealSaleZoneGroup dealSaleZoneGroup = GetAccountSaleZoneGroup(record.CustomerId, record.SaleZoneId, record.AttemptDateTime);
            if (dealSaleZoneGroup != null)
            {
                record.OrigSaleDealId = dealSaleZoneGroup.DealId;
                record.OrigSaleDealZoneGroupNb = dealSaleZoneGroup.DealSaleZoneGroupNb;
            }
        }

        public DealSaleZoneGroup GetDealSaleZoneGroup(int dealId, int zoneGroupNb)
        {
            var cachedDealSaleZoneGroups = GetCachedDealSaleZoneGroups();
            return cachedDealSaleZoneGroups.GetRecord(new DealZoneGroup() { DealId = dealId, ZoneGroupNb = zoneGroupNb });
        }

        public DealSaleZoneGroup GetAccountSaleZoneGroup(int? customerId, long? saleZoneId, DateTime effectiveDate)
        {
            if (!customerId.HasValue || !saleZoneId.HasValue)
                return null;

            var cachedAccountSaleZoneGroups = GetCachedAccountSaleZoneGroups();
            IOrderedEnumerable<DealSaleZoneGroup> dealSaleZoneGroups = cachedAccountSaleZoneGroups.GetRecord(new AccountZoneGroup() { AccountId = customerId.Value, ZoneId = saleZoneId.Value });
            if (dealSaleZoneGroups != null && dealSaleZoneGroups.Count() > 0)
            {
                foreach (DealSaleZoneGroup dealSaleZoneGroup in dealSaleZoneGroups)
                {
                    if (dealSaleZoneGroup.IsEffective(effectiveDate))
                        return dealSaleZoneGroup;
                }
            }
            return null;
        }

        public DealZoneGroupTier GetSaleDealZoneGroupTier(int dealId, int zoneGroupNb, int tierNb)
        {
            DealSaleZoneGroup dealSaleZoneGroup = GetDealSaleZoneGroup(dealId, zoneGroupNb);

            DealSaleZoneGroupTier dealZoneGroupTier = dealSaleZoneGroup.Tiers.Where(itm => itm.TierNumber == tierNb).FirstOrDefault();
            if (dealZoneGroupTier == null)
                return null;

            return BuildDealZoneGroupTier(dealZoneGroupTier);
        }

        public DealZoneGroupTier GetUpToVolumeDealSaleZoneGroupTier(int dealId, int zoneGroupNb, decimal totalReachedDurationInSec, out decimal tierReachedDurationInSec)
        {
            DealSaleZoneGroup dealSaleZoneGroup = GetDealSaleZoneGroup(dealId, zoneGroupNb);
            dealSaleZoneGroup.Tiers.ThrowIfNull("dealSaleZoneGroup.Tiers");

            decimal remainingDurationInSec = totalReachedDurationInSec;
            DealZoneGroupTier dealZoneGroupTier = null;

            foreach (var dealSaleZoneGroupTier in dealSaleZoneGroup.Tiers)
            {
                if (!dealSaleZoneGroupTier.VolumeInSeconds.HasValue)
                {
                    dealZoneGroupTier = BuildDealZoneGroupTier(dealSaleZoneGroupTier);
                    break;
                }

                remainingDurationInSec -= dealSaleZoneGroupTier.VolumeInSeconds.Value;

                if (remainingDurationInSec < 0)
                {
                    dealZoneGroupTier = BuildDealZoneGroupTier(dealSaleZoneGroupTier);
                    break;
                }
            }

            tierReachedDurationInSec = Math.Abs(remainingDurationInSec);
            return dealZoneGroupTier;
        }

        public void FillOrigSupplierValues(dynamic record)
        {
            record.OrigCostRateId = record.CostRateId;
            record.OrigCostRateValue = record.CostRateValue;
            record.OrigCostNet = record.CostNet;
            record.OrigCostExtraChargeRateValue = record.CostExtraChargeRateValue;
            record.OrigCostExtraChargeValue = record.CostExtraChargeValue;
            record.OrigCostDurationInSeconds = record.CostDurationInSeconds;

            DealSupplierZoneGroup dealSupplierZoneGroup = GetAccountSupplierZoneGroup(record.SupplierId, record.SupplierZoneId, record.AttemptDateTime);
            if (dealSupplierZoneGroup != null)
            {
                record.OrigCostDealId = dealSupplierZoneGroup.DealId;
                record.OrigCostDealZoneGroupNb = dealSupplierZoneGroup.DealSupplierZoneGroupNb;
            }
        }

        public DealSupplierZoneGroup GetDealSupplierZoneGroup(int dealId, int zoneGroupNb)
        {
            var cachedDealSupplierZoneGroups = GetCachedDealSupplierZoneGroups();
            return cachedDealSupplierZoneGroups.GetRecord(new DealZoneGroup() { DealId = dealId, ZoneGroupNb = zoneGroupNb });
        }

        public DealSupplierZoneGroup GetAccountSupplierZoneGroup(int? supplierId, long? supplierZoneId, DateTime effectiveDate)
        {
            if (!supplierId.HasValue || !supplierZoneId.HasValue)
                return null;

            var cachedAccountSupplierZoneGroups = GetCachedAccountSupplierZoneGroups();
            IOrderedEnumerable<DealSupplierZoneGroup> dealSupplierZoneGroups = cachedAccountSupplierZoneGroups.GetRecord(new AccountZoneGroup() { AccountId = supplierId.Value, ZoneId = supplierZoneId.Value });
            if (dealSupplierZoneGroups != null && dealSupplierZoneGroups.Count() > 0)
            {
                foreach (DealSupplierZoneGroup dealSupplierZoneGroup in dealSupplierZoneGroups)
                {
                    if (dealSupplierZoneGroup.IsEffective(effectiveDate))
                        return dealSupplierZoneGroup;
                }
            }
            return null;
        }

        public DealZoneGroupTier GetSupplierDealZoneGroupTier(int dealId, int zoneGroupNb, int tierNb)
        {
            DealSupplierZoneGroup dealSupplierZoneGroup = GetDealSupplierZoneGroup(dealId, zoneGroupNb);

            DealSupplierZoneGroupTier dealZoneGroupTier = dealSupplierZoneGroup.Tiers.Where(itm => itm.TierNumber == tierNb).FirstOrDefault();
            if (dealZoneGroupTier == null)
                return null;

            return BuildDealZoneGroupTier(dealZoneGroupTier);
        }

        public DealZoneGroupTier GetUpToVolumeDealSupplierZoneGroupTier(int dealId, int zoneGroupNb, decimal totalReachedDurationInSec, out decimal tierReachedDurationInSec)
        {
            DealSupplierZoneGroup dealSupplierZoneGroup = GetDealSupplierZoneGroup(dealId, zoneGroupNb);
            dealSupplierZoneGroup.Tiers.ThrowIfNull("dealSupplierZoneGroup.Tiers");

            decimal remainingDurationInSec = totalReachedDurationInSec;
            DealZoneGroupTier dealZoneGroupTier = null;

            foreach (var dealSupplierZoneGroupTier in dealSupplierZoneGroup.Tiers)
            {
                if (!dealSupplierZoneGroupTier.VolumeInSeconds.HasValue)
                {
                    dealZoneGroupTier = BuildDealZoneGroupTier(dealSupplierZoneGroupTier);
                    break;
                }

                remainingDurationInSec -= dealSupplierZoneGroupTier.VolumeInSeconds.Value;

                if (remainingDurationInSec < 0)
                {
                    dealZoneGroupTier = BuildDealZoneGroupTier(dealSupplierZoneGroupTier);
                    break;
                }
            }

            tierReachedDurationInSec = Math.Abs(remainingDurationInSec);
            return dealZoneGroupTier;
        }

        public override BaseDealManager.BaseDealLoggableEntity GetLoggableEntity()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Private Methods

        Dictionary<DealZoneGroup, DealSaleZoneGroup> GetCachedDealSaleZoneGroups()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetCachedDealSaleZoneGroups", () =>
            {
                Dictionary<DealZoneGroup, DealSaleZoneGroup> result = new Dictionary<DealZoneGroup, DealSaleZoneGroup>();
                var cachedDeals = base.GetCachedDeals();
                foreach (DealDefinition dealDefinition in cachedDeals.Values)
                {
                    DealGetZoneGroupsContext context = new DealGetZoneGroupsContext() { DealId = dealDefinition.DealId };
                    dealDefinition.Settings.GetZoneGroups(context);
                    if (context.SaleZoneGroups != null)
                    {
                        foreach (DealSaleZoneGroup dealSaleZoneGroup in context.SaleZoneGroups)
                        {
                            result.Add(new DealZoneGroup()
                            {
                                DealId = dealDefinition.DealId,
                                ZoneGroupNb = dealSaleZoneGroup.DealSaleZoneGroupNb
                            }
                                , dealSaleZoneGroup);
                        }
                    }
                }
                return result;
            });
        }

        Dictionary<DealZoneGroup, DealSupplierZoneGroup> GetCachedDealSupplierZoneGroups()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetCachedDealSupplierZoneGroups", () =>
            {
                Dictionary<DealZoneGroup, DealSupplierZoneGroup> result = new Dictionary<DealZoneGroup, DealSupplierZoneGroup>();
                var cachedDeals = base.GetCachedDeals();
                foreach (DealDefinition dealDefinition in cachedDeals.Values)
                {
                    DealGetZoneGroupsContext context = new DealGetZoneGroupsContext() { DealId = dealDefinition.DealId };
                    dealDefinition.Settings.GetZoneGroups(context);
                    if (context.SupplierZoneGroups != null)
                    {
                        foreach (DealSupplierZoneGroup dealSupplierZoneGroup in context.SupplierZoneGroups)
                        {
                            result.Add(new DealZoneGroup()
                            {
                                DealId = dealDefinition.DealId,
                                ZoneGroupNb = dealSupplierZoneGroup.DealSupplierZoneGroupNb
                            }
                                , dealSupplierZoneGroup);
                        }
                    }
                }
                return result;
            });
        }

        Dictionary<AccountZoneGroup, IOrderedEnumerable<DealSaleZoneGroup>> GetCachedAccountSaleZoneGroups()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetCachedAccountSaleZoneGroups", () =>
            {
                Dictionary<AccountZoneGroup, List<DealSaleZoneGroup>> result = new Dictionary<AccountZoneGroup, List<DealSaleZoneGroup>>();
                var cachedDeals = base.GetCachedDeals();
                foreach (DealDefinition dealDefinition in cachedDeals.Values)
                {
                    DealGetZoneGroupsContext context = new DealGetZoneGroupsContext() { DealId = dealDefinition.DealId };
                    dealDefinition.Settings.GetZoneGroups(context);
                    if (context.SaleZoneGroups != null)
                    {
                        foreach (DealSaleZoneGroup dealSaleZoneGroup in context.SaleZoneGroups)
                        {
                            foreach (DealSaleZoneGroupZoneItem dealSaleZoneGroupZoneItem in dealSaleZoneGroup.Zones)
                            {
                                AccountZoneGroup accountZoneGroup = new AccountZoneGroup()
                                {
                                    AccountId = dealSaleZoneGroup.CustomerId,
                                    ZoneId = dealSaleZoneGroupZoneItem.ZoneId
                                };
                                List<DealSaleZoneGroup> dealSaleZoneGroups = result.GetOrCreateItem(accountZoneGroup);
                                dealSaleZoneGroups.Add(dealSaleZoneGroup);
                            }
                        }
                    }
                }
                return result.ToDictionary(itm => itm.Key, itm => itm.Value.OrderByDescending(item => item.BED));
            });
        }

        Dictionary<AccountZoneGroup, IOrderedEnumerable<DealSupplierZoneGroup>> GetCachedAccountSupplierZoneGroups()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetCachedAccountSupplierZoneGroups", () =>
            {
                Dictionary<AccountZoneGroup, List<DealSupplierZoneGroup>> result = new Dictionary<AccountZoneGroup, List<DealSupplierZoneGroup>>();
                var cachedDeals = base.GetCachedDeals();
                foreach (DealDefinition dealDefinition in cachedDeals.Values)
                {
                    DealGetZoneGroupsContext context = new DealGetZoneGroupsContext() { DealId = dealDefinition.DealId };
                    dealDefinition.Settings.GetZoneGroups(context);
                    if (context.SupplierZoneGroups != null)
                    {
                        foreach (DealSupplierZoneGroup dealSupplierZoneGroup in context.SupplierZoneGroups)
                        {
                            foreach (DealSupplierZoneGroupZoneItem dealSupplierZoneGroupZoneItem in dealSupplierZoneGroup.Zones)
                            {
                                AccountZoneGroup accountZoneGroup = new AccountZoneGroup()
                                {
                                    AccountId = dealSupplierZoneGroup.SupplierId,
                                    ZoneId = dealSupplierZoneGroupZoneItem.ZoneId
                                };
                                List<DealSupplierZoneGroup> dealSupplierZoneGroups = result.GetOrCreateItem(accountZoneGroup);
                                dealSupplierZoneGroups.Add(dealSupplierZoneGroup);
                            }
                        }
                    }
                }
                return result.ToDictionary(itm => itm.Key, itm => itm.Value.OrderByDescending(item => item.BED));
            });
        }

        Dictionary<long, decimal> BuildSupplierExceptionRates(List<DealSupplierZoneGroupTierZoneRate> exceptionRates)
        {
            if (exceptionRates == null || exceptionRates.Count == 0)
                return null;

            Dictionary<long, decimal> exceptionalRates = new Dictionary<long, decimal>();
            foreach (DealSupplierZoneGroupTierZoneRate exceptionRate in exceptionRates)
            {
                exceptionalRates.Add(exceptionRate.ZoneId, exceptionRate.Rate);
            }
            return exceptionalRates;
        }

        Dictionary<long, decimal> BuildSaleExceptionRates(List<DealSaleZoneGroupTierZoneRate> exceptionRates)
        {
            if (exceptionRates == null || exceptionRates.Count == 0)
                return null;

            Dictionary<long, decimal> exceptionalRates = new Dictionary<long, decimal>();
            foreach (DealSaleZoneGroupTierZoneRate exceptionRate in exceptionRates)
            {
                exceptionalRates.Add(exceptionRate.ZoneId, exceptionRate.Rate);
            }
            return exceptionalRates;
        }

        DealZoneGroupTier BuildDealZoneGroupTier(DealSaleZoneGroupTier dealSaleZoneGroupTier)
        {
            return new DealZoneGroupTier()
            {
                TierNumber = dealSaleZoneGroupTier.TierNumber,
                VolumeInSeconds = dealSaleZoneGroupTier.VolumeInSeconds,
                Rate = dealSaleZoneGroupTier.Rate,
                ExceptionRates = BuildSaleExceptionRates(dealSaleZoneGroupTier.ExceptionRates)
            };
        }

        DealZoneGroupTier BuildDealZoneGroupTier(DealSupplierZoneGroupTier dealSupplierZoneGroupTier)
        {
            return new DealZoneGroupTier()
            {
                TierNumber = dealSupplierZoneGroupTier.TierNumber,
                VolumeInSeconds = dealSupplierZoneGroupTier.VolumeInSeconds,
                Rate = dealSupplierZoneGroupTier.Rate,
                ExceptionRates = BuildSupplierExceptionRates(dealSupplierZoneGroupTier.ExceptionRates)
            };
        }

        #endregion

        #region Mappers

        DealDefinitionInfo DealDefinitionInfoMapper(DealDefinition dealDefinition)
        {
            return new DealDefinitionInfo()
            {
                DealId = dealDefinition.DealId,
                Name = dealDefinition.Name
            };
        }

        public override DealDefinitionDetail DealDeinitionDetailMapper(DealDefinition deal)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}