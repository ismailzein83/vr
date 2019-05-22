using System;
using System.Collections.Generic;
using System.Linq;
using TOne.WhS.Deal.Entities;
using Vanrise.Analytic.Business;
using Vanrise.Analytic.Entities;
using Vanrise.Common;
using Vanrise.GenericData.Entities;

namespace TOne.WhS.Deal.Business.Extensions
{
    public class DealMemoryAnalyticDataManager : MemoryAnalyticDataManager
    {
        public override List<RawMemoryRecord> GetRawRecords(AnalyticQuery query, List<string> neededFieldNames)
        {
            throw new NotImplementedException();

            //if (!query.ToTime.HasValue)
            //    throw new NullReferenceException("query.ToTime");

            //Dictionary<PropertyName, string> propertyNames = this.BuildPropertyNames();

            //Dictionary<int, DealDefinition> filteredDeals = this.GetFilteredDeals(query.FromTime, query.ToTime.Value);
            //List<AnalyticRecord> buyDealBillingStatRecords = this.GetBuyDealBillingStatRecords(query, filteredDeals.Values.ToList(), propertyNames);

            //Dictionary<DealZoneGroup, List<CustomerRouteBillingStatRecord>> customerRouteBillingStatRecordsByDealZoneGroup = this.StructureBillingRecords(buyDealBillingStatRecords, propertyNames);
            //Dictionary<DealZoneGroup, DealZoneGroupData> dealZoneGroupDataByDealZoneGroup = this.GetDealZoneGroupData(customerRouteBillingStatRecordsByDealZoneGroup);

            //foreach (var kvp in customerRouteBillingStatRecordsByDealZoneGroup)
            //{
            //    DealZoneGroup dealZoneGroup = kvp.Key;
            //    List<CustomerRouteBillingStatRecord> customerRouteBillingStatRecords = kvp.Value;

            //    DealDefinition dealDefinition = filteredDeals.GetRecord(dealZoneGroup.DealId);
            //    if (!dealDefinition.Settings.RealEED.HasValue)
            //        throw new NullReferenceException(String.Format("deal.Settings.EndDate '{0}'", dealDefinition.DealId));

            //    var getDealZoneGroupDataContext = new GetDealZoneGroupDataContext() { ZoneGroupNb = dealZoneGroup.ZoneGroupNb, IsSale = false };
            //    dealDefinition.Settings.GetDealZoneGroupData(getDealZoneGroupDataContext);

            //    if(getDealZoneGroupDataContext.DealZoneGroupData != null && getDealZoneGroupDataContext.DealZoneGroupData.TotalVolumeInMin.HasValue)
            //    {
            //        int nbOfDealDays = (int)(dealDefinition.Settings.RealEED.Value - dealDefinition.Settings.RealBED).TotalDays;
            //        Decimal dailyEstimatedVolumeInSec = (Decimal)(getDealZoneGroupDataContext.DealZoneGroupData.TotalVolumeInMin.Value * 60) / nbOfDealDays;

            //        int nbOfPeriodDays = (int)(Utilities.Min(dealDefinition.Settings.RealEED.Value, query.ToTime.Value) - Utilities.Max(dealDefinition.Settings.RealBED, query.FromTime)).TotalDays;
            //        Decimal periodEstimatedVolumeInSec = nbOfPeriodDays * dailyEstimatedVolumeInSec;

            //        DealZoneGroupData dealZoneGroupData = dealZoneGroupDataByDealZoneGroup.GetRecord(dealZoneGroup);

            //        Decimal completionPercentage

            //        for (DateTime d = ; d < ; d = d.AddDays(1))
            //        {
            //            dealDays.Add(d);
            //        }
            //    }
            //}


            //List<RawMemoryRecord> rawMemoryRecords = new List<RawMemoryRecord>();

            //foreach (AnalyticRecord buyDealBillingStatRecord in buyDealBillingStatRecords)
            //{
            //    RawMemoryRecord rawMemoryRecord = new RawMemoryRecord { FieldValues = new Dictionary<string, dynamic>() };
            //    rawMemoryRecord.FieldValues.Add("Deal", buyDealBillingStatRecord.DimensionValues[0]);

            //    decimal zoneProfitValue = 0;
            //    MeasureValue zoneProfit;
            //    buyDealBillingStatRecord.MeasureValues.TryGetValue(propertyNames[PropertyName.TotalProfit], out zoneProfit);
            //    if (zoneProfit != null && zoneProfit.Value != null)
            //        zoneProfitValue = Convert.ToDecimal(zoneProfit.Value);

            //    rawMemoryRecord.FieldValues.Add("ZoneProfit", zoneProfitValue);
            //    rawMemoryRecords.Add(rawMemoryRecord);
            //}

            //return rawMemoryRecords;
        }

        //private Dictionary<int, DealDefinition> GetFilteredDeals(DateTime fromTime, DateTime toTime)
        //{
        //    Dictionary<int, DealDefinition> filteredDeals = new Dictionary<int, DealDefinition>();

        //    Dictionary<int, DealDefinition> dealDefinitions = new DealDefinitionManager().GetAllCachedDealDefinitions();

        //    foreach (var dealDefinition in dealDefinitions.Values)
        //    {
        //        dealDefinition.Settings.ThrowIfNull("dealDefinition.Settings");

        //        if (dealDefinition.Settings.Status != DealStatus.Active)
        //            continue;

        //        if (dealDefinition.Settings.GetDealZoneGroupPart() == DealZoneGroupPart.Sale)
        //            continue;

        //        if (!Utilities.IsEffective(toTime, dealDefinition.Settings.RealBED, dealDefinition.Settings.RealEED))
        //            continue;

        //        filteredDeals.Add(dealDefinition.DealId, dealDefinition);
        //    }

        //    return filteredDeals;
        //}

        //private List<AnalyticRecord> GetBuyDealBillingStatRecords(AnalyticQuery query, List<DealDefinition> filteredDeals, Dictionary<PropertyName, string> propertyNames)
        //{
        //    if (!query.ToTime.HasValue)
        //        throw new NullReferenceException("query.ToTime");

        //    List<string> dimensionFields = new List<string>() { propertyNames[PropertyName.CostDeal], propertyNames[PropertyName.CostDealZoneGroupNb] };
        //    dimensionFields.AddRange(query.DimensionFields);

        //    RecordFilter buyDealsRecordFilter = new NumberListRecordFilter()
        //    {
        //        FieldName = propertyNames[PropertyName.CostDeal],
        //        CompareOperator = ListRecordFilterOperator.In,
        //        Values = filteredDeals.Select(itm => (decimal)itm.DealId).ToList()
        //    };

        //    RecordFilterGroup filterGroup = new RecordFilterGroup();
        //    filterGroup.Filters.Add(buyDealsRecordFilter);

        //    AnalyticQuery costAnalyticQuery = new AnalyticQuery
        //    {
        //        TableId = Guid.Parse("4C1AAA1B-675B-420F-8E60-26B0747CA79B"),
        //        DimensionFields = dimensionFields,
        //        MeasureFields = new List<string> { propertyNames[PropertyName.CostDealDurInSec], propertyNames[PropertyName.TotalProfit] },
        //        FromTime = query.FromTime,
        //        ToTime = query.ToTime.Value,
        //        FilterGroup = filterGroup
        //    };

        //    AnalyticRecord analyticRecordSummary;
        //    List<AnalyticRecord> buyDealBillingStatRecords = new AnalyticManager().GetAllFilteredRecords(costAnalyticQuery, out analyticRecordSummary);
        //    return buyDealBillingStatRecords;
        //}

        ////private Dictionary<string, List<AnalyticRecord>> StructureBillingRecordByDealZone(List<AnalyticRecord> analyticRecords)
        ////{
        ////    Dictionary<PropertyName, string> propertyNames = BuildPropertyNames(isSale);
        ////    var analyticRecordByDeal = new Dictionary<int, List<AnalyticRecord>>();

        ////    foreach (var analyticRecord in analyticRecords)
        ////    {
        ////        var costDealId = analyticRecord.DimensionValues[0];
        ////        int costDealIdValue = (int)costDealId.Value;

        ////        string key = string.Format("{0}_{1}", origDealIdValue, origDealZoneIdValue);
        ////        List<BillingRecord> billingRecords = analyticRecordByDeal.GetOrCreateItem(key);

        ////        decimal durationValue = 0;
        ////        MeasureValue duration;
        ////        analyticRecord.MeasureValues.TryGetValue(propertyNames[PropertyName.Duration], out duration);
        ////        if (duration != null && duration.Value != null)
        ////            durationValue = Convert.ToDecimal(duration.Value ?? 0.0);


        ////        decimal netValue = 0;
        ////        MeasureValue net;
        ////        analyticRecord.MeasureValues.TryGetValue(propertyNames[PropertyName.Net], out net);
        ////        if (net != null && net.Value != null)
        ////            netValue = Convert.ToDecimal(net.Value ?? 0.0);

        ////        decimal netNotNullValue = 0;
        ////        MeasureValue netNotNull;
        ////        analyticRecord.MeasureValues.TryGetValue(propertyNames[PropertyName.NetNotNull], out netNotNull);
        ////        if (netNotNull != null && netNotNull.Value != null)
        ////            netNotNullValue = Convert.ToDecimal(netNotNull.Value ?? 0.0);

        ////        billingRecords.Add(new BillingRecord
        ////        {
        ////            HalfHourDateTime = halfHourDateTime,
        ////            Duration = durationValue,
        ////            Net = netValue,
        ////            NetNotNull = netNotNullValue
        ////        });
        ////    }

        ////    return analyticRecordByDeal;
        ////}

        //private Dictionary<DealZoneGroup, List<CustomerRouteBillingStatRecord>> StructureBillingRecords(List<AnalyticRecord> analyticRecords, Dictionary<PropertyName, string> propertyNames)
        //{
        //    var analyticRecordsByDealZoneGroup = new Dictionary<DealZoneGroup, List<CustomerRouteBillingStatRecord>>();

        //    foreach (var analyticRecord in analyticRecords)
        //    {
        //        int costDeal = (int)analyticRecord.DimensionValues[0].Value;
        //        int costDealZoneGroupNb = (int)analyticRecord.DimensionValues[1].Value;
        //        long supplierZoneId = (long)analyticRecord.DimensionValues[2].Value;

        //        decimal costDealDurInSecValue = 0;
        //        MeasureValue costDealDurInSec;
        //        analyticRecord.MeasureValues.TryGetValue(propertyNames[PropertyName.TotalProfit], out costDealDurInSec);
        //        if (costDealDurInSec != null && costDealDurInSec.Value != null)
        //            costDealDurInSecValue = Convert.ToDecimal(costDealDurInSec.Value);

        //        decimal zoneProfitValue = 0;
        //        MeasureValue zoneProfit;
        //        analyticRecord.MeasureValues.TryGetValue(propertyNames[PropertyName.TotalProfit], out zoneProfit);
        //        if (zoneProfit != null && zoneProfit.Value != null)
        //            zoneProfitValue = Convert.ToDecimal(zoneProfit.Value);

        //        DealZoneGroup dealZoneGroup = new DealZoneGroup() { DealId = costDeal, ZoneGroupNb = costDealZoneGroupNb };
        //        List<CustomerRouteBillingStatRecord> customerRouteBillingStatRecords = analyticRecordsByDealZoneGroup.GetOrCreateItem(dealZoneGroup);

        //        var customerRouteBillingStatRecord = new CustomerRouteBillingStatRecord()
        //        {
        //            CostDeal = costDeal,
        //            CostDealZoneGroupNb = costDealZoneGroupNb,
        //            SupplierZoneId = supplierZoneId,
        //            CostDealDurInSec = costDealDurInSecValue,
        //            TotalProfit = zoneProfitValue,
        //            AnalyticRecord = analyticRecord
        //        };
        //        customerRouteBillingStatRecords.Add(customerRouteBillingStatRecord);
        //    }

        //    return analyticRecordsByDealZoneGroup.Count > 0 ? analyticRecordsByDealZoneGroup : null;
        //}

        //private Dictionary<DealZoneGroup, DealZoneGroupData> GetDealZoneGroupData(Dictionary<DealZoneGroup, List<CustomerRouteBillingStatRecord>> customerRouteBillingStatRecordsByDealZoneGroup)
        //{
        //    Dictionary<DealZoneGroup, DealZoneGroupData> dealZoneGroupDataByDealZoneGroup = new Dictionary<DealZoneGroup, DealZoneGroupData>();

        //    foreach (var kvp in customerRouteBillingStatRecordsByDealZoneGroup)
        //    {
        //        DealZoneGroup dealZoneGroup = kvp.Key;
        //        List<CustomerRouteBillingStatRecord> customerRouteBillingStatRecords = kvp.Value;

        //        Decimal reachedDurationInSec = 0;

        //        foreach (var customerRouteBillingStatRecord in customerRouteBillingStatRecords)
        //            reachedDurationInSec += customerRouteBillingStatRecord.CostDealDurInSec;

        //        dealZoneGroupDataByDealZoneGroup.Add(dealZoneGroup, new DealZoneGroupData() { ReachedVolumeInSec = reachedDurationInSec });
        //    }

        //    return dealZoneGroupDataByDealZoneGroup.Count > 0 ? dealZoneGroupDataByDealZoneGroup : null;
        //}

        //private Dictionary<PropertyName, string> BuildPropertyNames()
        //{
        //    Dictionary<PropertyName, string> propertyNames = new Dictionary<PropertyName, string>
        //    {
        //        { PropertyName.SupplierZoneId, "SupplierZoneId" },
        //        { PropertyName.CostDeal, "CostDeal" },
        //        { PropertyName.TotalProfit, "TotalProfit" },
        //    };

        //    return propertyNames;
        //}

        //private enum PropertyName { CostDeal, CostDealZoneGroupNb, SupplierZoneId, CostDealDurInSec, TotalProfit }

        //private class CustomerRouteBillingStatRecord
        //{
        //    public int CostDeal { get; set; }
        //    public int CostDealZoneGroupNb { get; set; }
        //    public long SupplierZoneId { get; set; }
        //    public Decimal CostDealDurInSec { get; set; }
        //    public Decimal TotalProfit { get; set; }
        //    public AnalyticRecord AnalyticRecord { get; set; }
        //}

        //private class DealZoneGroupData
        //{
        //    public Decimal ReachedVolumeInSec { get; set; }
        //}
    }
}