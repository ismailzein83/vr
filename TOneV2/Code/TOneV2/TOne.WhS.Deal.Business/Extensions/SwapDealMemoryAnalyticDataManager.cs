using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.Deal.Entities;
using Vanrise.Analytic.Business;
using Vanrise.Analytic.Entities;
using Vanrise.Common;
using Vanrise.Entities;

namespace TOne.WhS.Deal.Business
{
    public class SwapDealMemoryAnalyticDataManager : MemoryAnalyticDataManager
    {
        public override List<RawMemoryRecord> GetRawRecords(Vanrise.Analytic.Entities.AnalyticQuery query, List<string> neededFieldNames)
        {
            List<RawMemoryRecord> rawMemoryRecords = new List<RawMemoryRecord>();
            List<int> filteredDealIds = null;
            if (query.Filters != null)
            {
                var dealDimensionFilter = query.Filters.FirstOrDefault(itm => itm.Dimension == "Deal");
                if (dealDimensionFilter != null)
                    filteredDealIds = dealDimensionFilter.FilterValues.Select(itm => Convert.ToInt32(itm)).ToList();
            }
            var swapDealManager = new SPDealManager();
            var filteredDeals = swapDealManager.GetAllSwapDeals().FindAllRecords(deal => (filteredDealIds == null || filteredDealIds.Contains(deal.DealId))
                && Utilities.AreTimePeriodsOverlapped(deal.Settings.BeginDate, deal.Settings.RealEED, query.FromTime, query.ToTime));

            DateTime fromTime = query.FromTime;
            DateTime? toTime = query.ToTime;

            foreach (var dealDefinition in filteredDeals)
            {
                TimeSpan? offSet = dealDefinition.Settings.OffSet;
                DateTime? dealEED = dealDefinition.Settings.EEDToStore.HasValue
                    ? dealDefinition.Settings.EEDToStore.Value.Add((new TimeSpan(0, 23, 59, 59, 998)))
                    : dealDefinition.Settings.EEDToStore;

                if (dealDefinition.Settings.BeginDate == query.FromTime && dealEED == query.ToTime && offSet.HasValue)
                {
                    DateTime fromTimeToCompare = query.FromTime.Subtract(offSet.Value);
                    fromTime = fromTime < fromTimeToCompare ? fromTime : fromTimeToCompare;
                    if (query.ToTime.HasValue)
                    {
                        DateTime? toTimeToCompare = query.ToTime.Value.Subtract(offSet.Value);
                        toTime = toTime < toTimeToCompare ? toTime : toTimeToCompare;
                    }
                }
            }

            bool estimationPerZone = query.DimensionFields.Contains("Zone") || query.DimensionFields.Contains("ZoneGroup")
                || (query.Filters != null && (query.Filters.Any(filter => filter.Dimension == "Zone" || filter.Dimension == "ZoneGroup")));

            List<AnalyticRecord> saleAnalyticRecords = GetSaleBillingRecords(fromTime, toTime);
            List<AnalyticRecord> costAnalyticRecords = GetCostBillingRecords(fromTime, toTime);

            Dictionary<string, List<BillingRecord>> saleAnalyticRecordByDealZone = StructureBillingRecordByDealZone(saleAnalyticRecords, true);
            Dictionary<string, List<BillingRecord>> costAnalyticRecordByDealZone = StructureBillingRecordByDealZone(costAnalyticRecords, false);

            if (filteredDeals != null && filteredDeals.Count() > 0)
            {
                foreach (var deal in filteredDeals)
                {
                    if (!deal.Settings.EndDate.HasValue)
                        throw new NullReferenceException(String.Format("deal.Settings.EndDate '{0}'", deal.DealId));
                    int nbOfDealDays = (int)(deal.Settings.EndDate.Value - deal.Settings.BeginDate).TotalDays;
                    List<DateTime> dealDays = new List<DateTime>();

                    DateTime queryToDate = toTime.HasValue ? toTime.Value.Date.AddDays(1) : DateTime.Today.AddDays(1);
                    for (DateTime d = Utilities.Max(deal.Settings.RealBED, fromTime.Date); d < Utilities.Min(deal.Settings.RealEED.Value, queryToDate); d = d.AddDays(1))
                    {
                        dealDays.Add(d);
                    }
                    SwapDealSettings swapDealSettings = deal.Settings.CastWithValidate<SwapDealSettings>("deal.Settings", deal.DealId);
                    if (swapDealSettings.Inbounds != null && swapDealSettings.Inbounds.Count > 0)
                    {
                        int totalVolume = swapDealSettings.Inbounds.Sum(item => item.Volume);
                        Decimal dailyTotalVolume = (decimal)totalVolume / nbOfDealDays;

                        foreach (var inbound in swapDealSettings.Inbounds)
                        {
                            Decimal dailyGroupVolume = (decimal)inbound.Volume / nbOfDealDays;
                            inbound.SaleZones.ThrowIfNull("inbound.SaleZones", deal.DealId);
                            foreach (var saleZone in inbound.SaleZones)
                            {
                                List<RawMemoryRecord> saleRawMemoryRecords = GetRawMemoryRecords(swapDealSettings.CarrierAccountId, deal.DealId,
                                    saleZone.ZoneId, inbound.Rate, inbound.Name, true, dealDays,
                                    saleAnalyticRecordByDealZone, dailyTotalVolume, dailyGroupVolume, estimationPerZone);
                                rawMemoryRecords.AddRange(saleRawMemoryRecords);
                            }
                        }
                    }
                    if (swapDealSettings.Outbounds != null && swapDealSettings.Outbounds.Count > 0)
                    {
                        int totalVolume = swapDealSettings.Outbounds.Sum(item => item.Volume);
                        Decimal dailyTotalVolume = (decimal)totalVolume / nbOfDealDays;

                        foreach (var outbound in swapDealSettings.Outbounds)
                        {
                            Decimal dailyGroupVolume = (decimal)outbound.Volume / nbOfDealDays;
                            outbound.SupplierZones.ThrowIfNull("outbound.SupplierZones", deal.DealId);
                            foreach (var supplierZone in outbound.SupplierZones)
                            {
                                List<RawMemoryRecord> costRawMemoryRecords = GetRawMemoryRecords(swapDealSettings.CarrierAccountId, deal.DealId,
                                    supplierZone.ZoneId, outbound.Rate, outbound.Name, false, dealDays,
                                    costAnalyticRecordByDealZone, dailyTotalVolume, dailyGroupVolume, estimationPerZone);
                                rawMemoryRecords.AddRange(costRawMemoryRecords);
                            }
                        }
                    }
                }
            }

            return rawMemoryRecords;
        }

        #region Private Methods

        private List<RawMemoryRecord> GetRawMemoryRecords(int carrierAccountId, long dealId, long zoneId, decimal rate, string zoneGroupName, bool isSale, List<DateTime> dealDays, Dictionary<string, List<BillingRecord>> billingRecordByDealZone
                 , Decimal dailyTotalVolume, Decimal dailyGroupVolume, bool estimationPerZone)
        {
            Dictionary<PropertyName, string> propertyNames = BuildPropertyNames(isSale);
            List<RawMemoryRecord> rawMemoryRecords = new List<RawMemoryRecord>();
            SaleZoneManager saleZoneManager = new SaleZoneManager();
            SupplierZoneManager supplierZoneManager = new SupplierZoneManager();

            string saleZoneName = isSale
                ? saleZoneManager.GetSaleZoneName(zoneId)
                : supplierZoneManager.GetSupplierZoneName(zoneId);

            saleZoneName.ThrowIfNull(propertyNames[PropertyName.Zone], zoneId);
            string key = string.Format("{0}_{1}", dealId, zoneId);

            List<BillingRecord> billingRecords;
            if (billingRecordByDealZone.TryGetValue(key, out billingRecords))
            {
                billingRecords = billingRecords.OrderBy(item => item.HalfHourDateTime).ToList();
            }

            int analyticIndex = 0;
            foreach (var day in dealDays)
            {
                RawMemoryRecord rawMemoryRecord = new RawMemoryRecord { FieldValues = new Dictionary<string, dynamic>() };
                rawMemoryRecord.FieldValues.Add("Day", day);
                rawMemoryRecord.FieldValues.Add("Deal", dealId);
                rawMemoryRecord.FieldValues.Add("IsSale", isSale);
                rawMemoryRecord.FieldValues.Add("ZoneGroup", zoneGroupName);
                rawMemoryRecord.FieldValues.Add("Zone", saleZoneName);

                decimal estimatedVolume = estimationPerZone ? dailyGroupVolume : dailyTotalVolume;
                decimal estimatedAmount = estimatedVolume * rate;

                decimal sumDuration = 0;
                decimal sumNet = 0;
                decimal sumNetNotNull = 0;

                if (billingRecords != null && billingRecords.Count > 0)
                {
                    for (int i = analyticIndex; i < billingRecords.Count; i++)
                    {
                        var record = billingRecords[i];
                        DateTime halfHourDateTimeShifted = record.HalfHourDateTime;

                        if (halfHourDateTimeShifted < day)
                            continue;

                        if (halfHourDateTimeShifted >= day.AddDays(1))
                        {
                            analyticIndex = i;
                            break;
                        }
                        sumDuration += record.Duration;
                        sumNet += record.Net;
                        sumNetNotNull += record.NetNotNull;
                    }
                }

                decimal saleDuration = isSale ? sumDuration : 0;
                decimal costDuration = !isSale ? sumDuration : 0;
                decimal saleNet = isSale ? sumNet : 0;
                decimal costNet = !isSale ? sumNet : 0;
                decimal saleNetNotNull = isSale ? sumNetNotNull : 0;
                decimal costNetNotNull = !isSale ? sumNetNotNull : 0;

                rawMemoryRecord.FieldValues.Add("SaleDuration", saleDuration);
                rawMemoryRecord.FieldValues.Add("SaleNet", saleNet);
                rawMemoryRecord.FieldValues.Add("CostDuration", costDuration);
                rawMemoryRecord.FieldValues.Add("CostNet", costNet);
                rawMemoryRecord.FieldValues.Add("CostNetNotNull", costNetNotNull);
                rawMemoryRecord.FieldValues.Add("SaleNetNotNull", saleNetNotNull);
                rawMemoryRecord.FieldValues.Add("DailyEstimatedVolume", estimatedVolume);
                rawMemoryRecord.FieldValues.Add("DailyEstimatedAmount", estimatedAmount);
                rawMemoryRecords.Add(rawMemoryRecord);
            }
            return rawMemoryRecords;
        }

        private List<AnalyticRecord> GetSaleBillingRecords(DateTime fromTime, DateTime? toTime)
        {
            AnalyticManager analyticManager = new AnalyticManager();
            AnalyticRecord analyticRecordSummary;

            AnalyticQuery costAnalyticQuery = new AnalyticQuery
            {
                TableId = Guid.Parse("4C1AAA1B-675B-420F-8E60-26B0747CA79B"),
                DimensionFields = new List<string> { "OrigSaleDeal", "SaleZone", "OrigSaleDealZoneGroupNb", "SaleDeal", "HalfHour" },
                MeasureFields = new List<string> { "SaleNetNotNULL", "SaleDuration" },
                FromTime = fromTime,
                ToTime = toTime.Value
            };

            List<AnalyticRecord> saleRecords = analyticManager.GetAllFilteredRecords(costAnalyticQuery, out analyticRecordSummary);

            return saleRecords;
        }
        private List<AnalyticRecord> GetCostBillingRecords(DateTime fromTime, DateTime? toTime)
        {
            AnalyticManager analyticManager = new AnalyticManager();
            AnalyticRecord analyticRecordSummary;

            AnalyticQuery costAnalyticQuery = new AnalyticQuery
            {
                TableId = Guid.Parse("4C1AAA1B-675B-420F-8E60-26B0747CA79B"),
                DimensionFields = new List<string> { "OrigCostDeal", "CostZone", "OrigCostDealZoneGroupNb", "CostDeal", "HalfHour" },
                MeasureFields = new List<string> { "CostNetNotNULL", "CostDuration" },
                FromTime = fromTime,
                ToTime = toTime.Value
            };

            List<AnalyticRecord> costRecords = analyticManager.GetAllFilteredRecords(costAnalyticQuery, out analyticRecordSummary);

            return costRecords;
        }

        private Dictionary<string, List<BillingRecord>> StructureBillingRecordByDealZone(List<AnalyticRecord> analyticRecords, bool isSale)
        {
            Dictionary<PropertyName, string> propertyNames = BuildPropertyNames(isSale);
            var analyticRecordByDealZone = new Dictionary<string, List<BillingRecord>>();

            foreach (var analyticRecord in analyticRecords)
            {
                var origDealId = analyticRecord.DimensionValues[0];
                var origDealZoneId = analyticRecord.DimensionValues[1];
                var halfHour = analyticRecord.DimensionValues[4];
                DateTime halfHourDateTime = (DateTime)halfHour.Value;

                if (origDealId.Value == null)
                    continue;

                int origDealIdValue = (int)origDealId.Value;
                long origDealZoneIdValue = (long)origDealZoneId.Value;

                string key = string.Format("{0}_{1}", origDealIdValue, origDealZoneIdValue);
                List<BillingRecord> billingRecords = analyticRecordByDealZone.GetOrCreateItem(key);

                decimal durationValue = 0;
                MeasureValue duration;
                analyticRecord.MeasureValues.TryGetValue(propertyNames[PropertyName.Duration], out duration);
                if (duration != null && duration.Value != null)
                    durationValue = Convert.ToDecimal(duration.Value ?? 0.0);


                decimal netValue = 0;
                MeasureValue net;
                analyticRecord.MeasureValues.TryGetValue(propertyNames[PropertyName.Net], out net);
                if (net != null && net.Value != null)
                    netValue = Convert.ToDecimal(net.Value ?? 0.0);

                decimal netNotNullValue = 0;
                MeasureValue netNotNull;
                analyticRecord.MeasureValues.TryGetValue(propertyNames[PropertyName.NetNotNull], out netNotNull);
                if (netNotNull != null && netNotNull.Value != null)
                    netNotNullValue = Convert.ToDecimal(netNotNull.Value ?? 0.0);

                billingRecords.Add(new BillingRecord
                {
                    HalfHourDateTime = halfHourDateTime,
                    Duration = durationValue,
                    Net = netValue,
                    NetNotNull = netNotNullValue
                });
            }
            return analyticRecordByDealZone;
        }
        private class SPDealManager : SwapDealManager
        {
            public IEnumerable<DealDefinition> GetAllSwapDeals()
            {
                return GetCachedSwapDeals();
            }
        }
        private Dictionary<PropertyName, string> BuildPropertyNames(bool isSale)
        {
            string prefix = isSale ? "Sale" : "Cost";
            Dictionary<PropertyName, string> propertyNames = new Dictionary<PropertyName, string>
            {
                {PropertyName.Duration, string.Format("{0}Duration", prefix)},
                {PropertyName.Net, string.Format("{0}Net", prefix)},
                {PropertyName.NetNotNull, string.Format("{0}NetNotNULL", prefix)},
                {PropertyName.Zone, string.Format("{0}Zone", prefix)}
            };
            return propertyNames;
        }
        private enum PropertyName
        {
            Duration,
            Net,
            Zone,
            NetNotNull
        }

        #endregion
        #region Public Classes

        public class BillingRecord
        {
            public DateTime HalfHourDateTime { get; set; }
            public decimal Duration { get; set; }
            public decimal Net { get; set; }
            public decimal NetNotNull { get; set; }
        }
        #endregion
    }
}
