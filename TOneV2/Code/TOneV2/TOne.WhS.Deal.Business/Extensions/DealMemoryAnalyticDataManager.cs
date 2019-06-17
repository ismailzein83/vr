using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TOne.WhS.Deal.Entities;
using Vanrise.Analytic.Business;
using Vanrise.Analytic.Entities;
using Vanrise.Common;
using Vanrise.GenericData.Entities;
using Vanrise.GenericData.MainExtensions.DataRecordFields;

namespace TOne.WhS.Deal.Business
{
    public class DealMemoryAnalyticDataManager : MemoryAnalyticDataManager
    {
        public override List<RawMemoryRecord> GetRawRecords(AnalyticQuery query, List<string> neededFieldNames)
        {
            if (!query.ToTime.HasValue)
                throw new NullReferenceException("query.ToTime");

            List<RawMemoryRecord> rawMemoryRecords = new List<RawMemoryRecord>();

            Dictionary<PropertyName, string> propertyNames = BuildPropertyNames();
            Dictionary<int, DealDefinition> costDealDefinitions = GetCostDealDefinitions(query.ToTime.Value);
            if (costDealDefinitions == null || costDealDefinitions.Count == 0)
                return null;

            List<DealZoneGroup> costDealZoneGroups = BuildCostDealZoneGroups(costDealDefinitions);

            List<AnalyticRecord> analyticRecords = GetAnalyticRecords(query, costDealDefinitions.Keys, propertyNames);
            if (analyticRecords == null || analyticRecords.Count == 0)
                return null;

            Dictionary<string, RawMemoryRecordData> rawMemoryRecordDataByIdentifier;
            Dictionary<DealZoneGroup, List<CostDealBillingStatRecord>> costDealBillingStatRecordsByDealZoneGroup = StructureBillingRecords(analyticRecords, propertyNames,
                query.DimensionFields, costDealZoneGroups, out rawMemoryRecordDataByIdentifier);

            Dictionary<DealZoneGroup, DealZoneGroupTrafficSummary> dealZoneGroupTrafficSummaryDict = GetDealZoneGroupTrafficSummary(costDealBillingStatRecordsByDealZoneGroup, costDealDefinitions,
                query.FromTime, query.ToTime.Value);

            foreach (var rawMemoryRecordDataKvp in rawMemoryRecordDataByIdentifier)
            {
                string serializedRawMemoryRecordIdentifier = rawMemoryRecordDataKvp.Key;
                RawMemoryRecordData rawMemoryRecordData = rawMemoryRecordDataKvp.Value;

                Dictionary<string, object> rawMemoryRecordIdentifier = DeserializeRawMemoryRecordIdentifier(serializedRawMemoryRecordIdentifier);

                Decimal zoneProfit = 0;
                Decimal reachedVolumeInSec = 0;
                Decimal? estimatedVolumeInSec = null;

                foreach (var dealZoneGroup in rawMemoryRecordData.DealZoneGroups)
                {
                    DealZoneGroupTrafficSummary dealZoneGroupTrafficSummary;
                    if (!dealZoneGroupTrafficSummaryDict.TryGetValue(dealZoneGroup, out dealZoneGroupTrafficSummary))
                        continue;

                    zoneProfit += dealZoneGroupTrafficSummary.TotalProfit;
                    reachedVolumeInSec += dealZoneGroupTrafficSummary.ReachedVolumeInSec;

                    if (dealZoneGroupTrafficSummary.EstimatedVolumeInSec.HasValue)
                        estimatedVolumeInSec = estimatedVolumeInSec.HasValue ? estimatedVolumeInSec.Value + dealZoneGroupTrafficSummary.EstimatedVolumeInSec.Value : dealZoneGroupTrafficSummary.EstimatedVolumeInSec.Value;
                }

                Decimal completionPercentage = 0;
                if (estimatedVolumeInSec.HasValue)
                    completionPercentage = (reachedVolumeInSec / estimatedVolumeInSec.Value) * 100;

                RawMemoryRecord rawMemoryRecord = new RawMemoryRecord { FieldValues = new Dictionary<string, dynamic>() };

                foreach (var itm in rawMemoryRecordIdentifier)
                    rawMemoryRecord.FieldValues.Add(itm.Key, itm.Value);

                rawMemoryRecord.FieldValues.Add("ZoneProfit", zoneProfit);
                rawMemoryRecord.FieldValues.Add("CompletionPercentage", completionPercentage);
                rawMemoryRecord.FieldValues.Add("Priority", rawMemoryRecordData.MinDealPriority);
                rawMemoryRecords.Add(rawMemoryRecord);
            }

            return rawMemoryRecords;
        }

        private List<DealZoneGroup> BuildCostDealZoneGroups(Dictionary<int, DealDefinition> costDealDefinitions)
        {
            List<DealZoneGroup> costDealZoneGroups = new List<DealZoneGroup>();

            foreach (var costDealDefinition in costDealDefinitions)
            {
                int dealId = costDealDefinition.Key;

                DealDefinition dealDefinition = costDealDefinition.Value;
                dealDefinition.ThrowIfNull("dealDefinition");
                dealDefinition.Settings.ThrowIfNull("dealDefinition.Settings");

                DealGetZoneGroupsContext dealGetZoneGroupsContext = new DealGetZoneGroupsContext(dealId, DealZoneGroupPart.Cost, false);
                dealDefinition.Settings.GetZoneGroups(dealGetZoneGroupsContext);
                dealGetZoneGroupsContext.SupplierZoneGroups.ThrowIfNull("dealGetZoneGroupsContext.SupplierZoneGroups");

                foreach (var supplierZoneGroup in dealGetZoneGroupsContext.SupplierZoneGroups)
                    costDealZoneGroups.Add(new DealZoneGroup() { DealId = dealId, ZoneGroupNb = supplierZoneGroup.DealSupplierZoneGroupNb });
            }

            return costDealZoneGroups;
        }

        private Dictionary<int, DealDefinition> GetCostDealDefinitions(DateTime effectiveDate)
        {
            Dictionary<int, DealDefinition> dealDefinitions = new DealDefinitionManager().GetAllCachedDealDefinitions();
            if (dealDefinitions == null || dealDefinitions.Count == 0)
                return null;

            Dictionary<int, DealDefinition> costDealDefinitions = new Dictionary<int, DealDefinition>();

            foreach (var dealDefinition in dealDefinitions.Values)
            {
                dealDefinition.Settings.ThrowIfNull("dealDefinition.Settings");

                if (dealDefinition.Settings.Status == DealStatus.Draft)
                    continue;

                if (dealDefinition.Settings.GetDealZoneGroupPart() == DealZoneGroupPart.Sale)
                    continue;

                if (!Utilities.IsEffective(effectiveDate, dealDefinition.Settings.RealBED, dealDefinition.Settings.RealEED))
                    continue;

                costDealDefinitions.Add(dealDefinition.DealId, dealDefinition);
            }

            return costDealDefinitions;
        }

        private List<AnalyticRecord> GetAnalyticRecords(AnalyticQuery query, IEnumerable<int> costDealDefinitionIds, Dictionary<PropertyName, string> propertyNames)
        {
            List<string> dimensionFields = new List<string>();
            dimensionFields.AddRange(query.DimensionFields);
            dimensionFields.Add(propertyNames[PropertyName.CostDeal]);
            dimensionFields.Add(propertyNames[PropertyName.CostDealZoneGroupNb]);
            dimensionFields.Add(propertyNames[PropertyName.Priority]);

            ObjectListRecordFilter objectListRecordFilter = new ObjectListRecordFilter()
            {
                FieldName = propertyNames[PropertyName.CostDeal],
                CompareOperator = ListRecordFilterOperator.In,
                Values = costDealDefinitionIds.VRCast<object>().ToList()
            };
            RecordFilter recordFilter = Vanrise.GenericData.Business.Helper.ConvertToRecordFilter(objectListRecordFilter.FieldName, new FieldNumberType(), objectListRecordFilter);

            RecordFilterGroup filterGroup = new RecordFilterGroup();
            filterGroup.Filters = new List<RecordFilter>();
            filterGroup.Filters.Add(recordFilter);

            AnalyticQuery costAnalyticQuery = new AnalyticQuery
            {
                TableId = Guid.Parse("4C1AAA1B-675B-420F-8E60-26B0747CA79B"),
                DimensionFields = dimensionFields,
                MeasureFields = new List<string> { propertyNames[PropertyName.CostDealDurInSec], propertyNames[PropertyName.TotalProfit] },
                FromTime = query.FromTime,
                ToTime = query.ToTime.Value,
                FilterGroup = filterGroup
            };

            AnalyticRecord analyticRecordSummary;
            List<AnalyticRecord> costDealBillingStatRecords = new AnalyticManager().GetAllFilteredRecords(costAnalyticQuery, out analyticRecordSummary);
            return costDealBillingStatRecords;
        }

        private Dictionary<DealZoneGroup, List<CostDealBillingStatRecord>> StructureBillingRecords(List<AnalyticRecord> analyticRecords, Dictionary<PropertyName, string> propertyNames,
            List<string> dimensionRecords, List<DealZoneGroup> costDealZoneGroups, out Dictionary<string, RawMemoryRecordData> rawMemoryRecordDataByIdentifier)
        {
            rawMemoryRecordDataByIdentifier = new Dictionary<string, RawMemoryRecordData>();
            var costDealBillingStatRecordsByDealZoneGroup = new Dictionary<DealZoneGroup, List<CostDealBillingStatRecord>>();

            foreach (var analyticRecord in analyticRecords)
            {
                string rawMemoryRecordIdentifier = SerializeRawMemoryRecordIdentifier(dimensionRecords, analyticRecord.DimensionValues);

                int costDeal = (int)analyticRecord.DimensionValues[dimensionRecords.Count].Value;
                int costDealZoneGroupNb = (int)analyticRecord.DimensionValues[dimensionRecords.Count + 1].Value;
                int costDealPriority = (int)analyticRecord.DimensionValues[dimensionRecords.Count + 2].Value;

                DealZoneGroup dealZoneGroup = new DealZoneGroup() { DealId = costDeal, ZoneGroupNb = costDealZoneGroupNb };
                if (!costDealZoneGroups.Contains(dealZoneGroup))
                    continue;

                decimal costDealDurInSecValue = 0;
                MeasureValue costDealDurInSec;
                analyticRecord.MeasureValues.TryGetValue(propertyNames[PropertyName.CostDealDurInSec], out costDealDurInSec);
                if (costDealDurInSec != null && costDealDurInSec.Value != null)
                    costDealDurInSecValue = Convert.ToDecimal(costDealDurInSec.Value);

                decimal zoneProfitValue = 0;
                MeasureValue zoneProfit;
                analyticRecord.MeasureValues.TryGetValue(propertyNames[PropertyName.TotalProfit], out zoneProfit);
                if (zoneProfit != null && zoneProfit.Value != null)
                    zoneProfitValue = Convert.ToDecimal(zoneProfit.Value);

                CostDealBillingStatRecord costDealBillingStatRecord = new CostDealBillingStatRecord(analyticRecord, costDeal, costDealZoneGroupNb, costDealDurInSecValue, zoneProfitValue);

                List<CostDealBillingStatRecord> costDealBillingStatRecords = costDealBillingStatRecordsByDealZoneGroup.GetOrCreateItem(dealZoneGroup);
                costDealBillingStatRecords.Add(costDealBillingStatRecord);

                RawMemoryRecordData rawMemoryRecordData;
                if (!rawMemoryRecordDataByIdentifier.TryGetValue(rawMemoryRecordIdentifier, out rawMemoryRecordData))
                {
                    rawMemoryRecordData = new RawMemoryRecordData();
                    rawMemoryRecordData.MinDealPriority = costDealPriority;
                    rawMemoryRecordData.DealZoneGroups = new List<DealZoneGroup>() { dealZoneGroup };
                    rawMemoryRecordData.CostDealBillingStatRecords = new List<CostDealBillingStatRecord>() { costDealBillingStatRecord };
                    rawMemoryRecordDataByIdentifier.Add(rawMemoryRecordIdentifier, rawMemoryRecordData);
                }
                else
                {
                    rawMemoryRecordData.DealZoneGroups.Add(dealZoneGroup);
                    rawMemoryRecordData.CostDealBillingStatRecords.Add(costDealBillingStatRecord);

                    if (rawMemoryRecordData.MinDealPriority > costDealPriority)
                        rawMemoryRecordData.MinDealPriority = costDealPriority;
                }
            }

            return costDealBillingStatRecordsByDealZoneGroup;
        }

        private Dictionary<DealZoneGroup, DealZoneGroupTrafficSummary> GetDealZoneGroupTrafficSummary(Dictionary<DealZoneGroup, List<CostDealBillingStatRecord>> costDealBillingStatRecordsDict,
            Dictionary<int, DealDefinition> costDealDefinitions, DateTime fromTime, DateTime toTime)
        {
            Dictionary<DealZoneGroup, DealZoneGroupTrafficSummary> dealZoneGroupTrafficSummaryDict = new Dictionary<DealZoneGroup, DealZoneGroupTrafficSummary>();

            foreach (var costDealBillingStatRecordsKvp in costDealBillingStatRecordsDict)
            {
                DealZoneGroup dealZoneGroup = costDealBillingStatRecordsKvp.Key;
                List<CostDealBillingStatRecord> costDealBillingStatRecords = costDealBillingStatRecordsKvp.Value;

                Decimal? periodEstimatedVolumeInSec = null;

                DealDefinition dealDefinition = costDealDefinitions.GetRecord(dealZoneGroup.DealId);
                if (!dealDefinition.Settings.RealEED.HasValue)
                    throw new NullReferenceException(String.Format("deal.Settings.EndDate '{0}'", dealDefinition.DealId));

                var getDealZoneGroupDataContext = new GetDealZoneGroupDataContext() { ZoneGroupNb = dealZoneGroup.ZoneGroupNb, IsSale = false };
                dealDefinition.Settings.GetDealZoneGroupData(getDealZoneGroupDataContext);
                getDealZoneGroupDataContext.DealZoneGroupData.ThrowIfNull("getDealZoneGroupDataContext.DealZoneGroupData");

                if (getDealZoneGroupDataContext.DealZoneGroupData.TotalVolumeInMin.HasValue)
                {
                    int nbOfDealDays = (int)(dealDefinition.Settings.RealEED.Value - dealDefinition.Settings.RealBED).TotalDays;
                    Decimal dailyEstimatedVolumeInSec = (Decimal)(getDealZoneGroupDataContext.DealZoneGroupData.TotalVolumeInMin.Value * 60) / nbOfDealDays;

                    int nbOfPeriodDays = (int)(Utilities.Min(dealDefinition.Settings.RealEED.Value, toTime) - Utilities.Max(dealDefinition.Settings.RealBED, fromTime)).TotalDays;
                    periodEstimatedVolumeInSec = nbOfPeriodDays * dailyEstimatedVolumeInSec;
                }

                Decimal totalProfit = 0;
                Decimal reachedDurationInSec = 0;

                foreach (var costDealBillingStatRecord in costDealBillingStatRecords)
                {
                    totalProfit += costDealBillingStatRecord.TotalProfit;
                    reachedDurationInSec += costDealBillingStatRecord.CostDealDurInSec;
                }

                DealZoneGroupTrafficSummary dealZoneGroupTrafficSummary = new DealZoneGroupTrafficSummary()
                {
                    TotalProfit = totalProfit,
                    ReachedVolumeInSec = reachedDurationInSec,
                    EstimatedVolumeInSec = periodEstimatedVolumeInSec
                };
                dealZoneGroupTrafficSummaryDict.Add(dealZoneGroup, dealZoneGroupTrafficSummary);
            }

            return dealZoneGroupTrafficSummaryDict;
        }

        private string SerializeRawMemoryRecordIdentifier(List<string> dimensionRecords, DimensionValue[] dimensionValues)
        {
            StringBuilder sb_RawMemoryRecordIdentifier = new StringBuilder();

            for (var index = 0; index < dimensionRecords.Count; index++)
            {
                string currentDimensionFieldName = dimensionRecords[index];
                object currentDimensionValue = dimensionValues[index].Value;

                if (sb_RawMemoryRecordIdentifier.Length > 0)
                    sb_RawMemoryRecordIdentifier.Append("&");

                sb_RawMemoryRecordIdentifier.Append(string.Concat(currentDimensionFieldName, ":", currentDimensionValue));
            }

            return sb_RawMemoryRecordIdentifier.ToString();
        }

        private Dictionary<string, object> DeserializeRawMemoryRecordIdentifier(string rawMemoryRecordIdentifier)
        {
            Dictionary<string, object> deserializeRawMemoryRecordIdentifier = new Dictionary<string, object>();

            string[] dimensions = rawMemoryRecordIdentifier.Split('&');

            foreach (var dimension in dimensions)
            {
                string[] dimensionItems = dimension.Split(':');
                string dimensionName = dimensionItems[0];
                object dimensionValue = dimensionItems[1];

                deserializeRawMemoryRecordIdentifier.Add(dimensionName, dimensionValue);
            }

            return deserializeRawMemoryRecordIdentifier;
        }

        private Dictionary<PropertyName, string> BuildPropertyNames()
        {
            Dictionary<PropertyName, string> propertyNames = new Dictionary<PropertyName, string>
            {
                { PropertyName.SupplierZone, "SupplierZone" },
                { PropertyName.Supplier, "Supplier" },
                { PropertyName.SaleZone, "SaleZone" },
                { PropertyName.CostDeal, "CostDeal" },
                { PropertyName.CostDealZoneGroupNb, "CostDealZoneGroupNb" },
                { PropertyName.Priority, "Priority" },
                { PropertyName.CostDealDurInSec, "CostDealDurationInSec" },
                { PropertyName.TotalProfit, "TotalProfit" }
            };

            return propertyNames;
        }

        private enum PropertyName { SupplierZone, Supplier, SaleZone, CostDeal, CostDealZoneGroupNb, Priority, CostDealDurInSec, TotalProfit }

        private class CostDealBillingStatRecord
        {
            public AnalyticRecord AnalyticRecord { get; set; }
            public int CostDeal { get; set; }
            public int CostDealZoneGroupNb { get; set; }
            public Decimal CostDealDurInSec { get; set; }
            public Decimal TotalProfit { get; set; }

            public CostDealBillingStatRecord(AnalyticRecord analyticRecord, int costDeal, int costDealZoneGroupNb, decimal costDealDurInSec, decimal totalProfit)
            {
                CostDeal = costDeal;
                CostDealZoneGroupNb = costDealZoneGroupNb;
                CostDealDurInSec = costDealDurInSec;
                TotalProfit = totalProfit;
                AnalyticRecord = analyticRecord;
            }
        }

        private class DealZoneGroupTrafficSummary
        {
            public Decimal TotalProfit { get; set; }

            public Decimal ReachedVolumeInSec { get; set; }

            public Decimal? EstimatedVolumeInSec { get; set; }
        }

        private class RawMemoryRecordData
        {
            public int MinDealPriority { get; set; }

            public List<DealZoneGroup> DealZoneGroups { get; set; }

            public List<CostDealBillingStatRecord> CostDealBillingStatRecords { get; set; }
        }
    }
}