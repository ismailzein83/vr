using System;
using System.Collections.Generic;
using TOne.WhS.Deal.Entities;
using Vanrise.Analytic.Business;
using Vanrise.Analytic.Entities;
using Vanrise.Entities;
using Vanrise.Common;
using System.Linq;

namespace TOne.WhS.Deal.Business
{
    public class DealBillingSummaryManager
    {
        #region Public Methods

        public Dictionary<DealZoneGroup, List<DealBillingSummary>> LoadDealBillingSummaryRecords(DateTime beginDate, bool isSale)
        {
            Dictionary<DealZoneGroup, List<DealBillingSummary>> dealBillingSummaryRecords = null;
            Dictionary<BatchDealZoneGroup, DealDurations> batchDealZoneGroupDict = null;

            Dictionary<PropertyName, string> propertyNames = BuildPropertyNames(isSale == true ? "Sale" : "Cost");
            Dictionary<PropertyName, int> dimensionFieldsDict = BuildDimensionFieldsDict(isSale == true ? "Sale" : "Cost");

            Vanrise.Entities.DataRetrievalInput<AnalyticQuery> analyticQuery = BuildAnalyticQuery(beginDate, propertyNames);

            var result = new AnalyticManager().GetFilteredRecords(analyticQuery) as AnalyticSummaryBigResult<AnalyticRecord>;
            if (result != null && result.Data != null && result.Data.Count() > 0)
            {
                dealBillingSummaryRecords = new Dictionary<DealZoneGroup, List<DealBillingSummary>>();
                batchDealZoneGroupDict = new Dictionary<BatchDealZoneGroup, DealDurations>();

                foreach (var analyticRecord in result.Data)
                {
                    DateTime batchStart = (DateTime)analyticRecord.DimensionValues[dimensionFieldsDict[PropertyName.HalfHour]].Value;
                    int origDealId = (int)analyticRecord.DimensionValues[dimensionFieldsDict[PropertyName.OrigDeal]].Value;
                    int origDealZoneGroupNb = (int)analyticRecord.DimensionValues[dimensionFieldsDict[PropertyName.OrigDealZoneGroupNb]].Value;
                    int? dealId = (int?)analyticRecord.DimensionValues[dimensionFieldsDict[PropertyName.Deal]].Value;
                    int? dealZoneGroupNb = (int?)analyticRecord.DimensionValues[dimensionFieldsDict[PropertyName.DealZoneGroupNb]].Value;
                    int? dealTierNb = (int?)analyticRecord.DimensionValues[dimensionFieldsDict[PropertyName.DealTierNb]].Value;
                    int? dealRateTierNb = (int?)analyticRecord.DimensionValues[dimensionFieldsDict[PropertyName.DealRateTierNb]].Value;
                    decimal durationInSeconds = (decimal)analyticRecord.MeasureValues.GetRecord(propertyNames[PropertyName.PricedDurationInSec]).Value;
                    decimal? dealDurationInSeconds = (decimal?)analyticRecord.MeasureValues.GetRecord(propertyNames[PropertyName.DealDurationInSec]).Value;

                    BatchDealZoneGroup batchDealZoneGroup = new BatchDealZoneGroup() { DealId = origDealId, DealZoneGroupNb = origDealZoneGroupNb, BatchStart = batchStart };
                    DealDurations dealDurations = batchDealZoneGroupDict.GetOrCreateItem(batchDealZoneGroup, () => { return new DealDurations() { DealDurationInSeconds = 0, DurationInSeconds = 0 }; });
                    dealDurations.DurationInSeconds += durationInSeconds;
                    dealDurations.DealDurationInSeconds += dealDurationInSeconds.HasValue ? dealDurationInSeconds.Value : 0;

                    if (dealId.HasValue)
                    {
                        DealZoneGroup dealZoneGroup = new DealZoneGroup() { DealId = dealId.Value, ZoneGroupNb = dealZoneGroupNb.Value };
                        DealBillingSummary dealBillingSummary = BuildDealBillingSummary(isSale, batchStart, dealId.Value, dealZoneGroupNb.Value, dealDurationInSeconds.Value, dealTierNb, dealRateTierNb);

                        List<DealBillingSummary> dealBillingSummaryList = dealBillingSummaryRecords.GetOrCreateItem(dealZoneGroup);
                        dealBillingSummaryList.Add(dealBillingSummary);
                    }
                }

                foreach (var batchDealZoneGroupItem in batchDealZoneGroupDict)
                {
                    decimal outDealDurationInSeconds = batchDealZoneGroupItem.Value.DurationInSeconds - batchDealZoneGroupItem.Value.DealDurationInSeconds;
                    if (outDealDurationInSeconds > 0)
                    {
                        BatchDealZoneGroup batchDealZoneGroup = batchDealZoneGroupItem.Key;
                        DealZoneGroup dealZoneGroup = new DealZoneGroup() { DealId = batchDealZoneGroup.DealId, ZoneGroupNb = batchDealZoneGroup.DealZoneGroupNb };
                        DealBillingSummary dealBillingSummary = BuildDealBillingSummary(isSale, batchDealZoneGroup.BatchStart, batchDealZoneGroup.DealId, batchDealZoneGroup.DealZoneGroupNb, outDealDurationInSeconds, null, null);

                        List<DealBillingSummary> dealBillingSummaryList = dealBillingSummaryRecords.GetOrCreateItem(dealZoneGroup);
                        dealBillingSummaryList.Add(dealBillingSummary);
                    }
                }
            }

            return dealBillingSummaryRecords;
        }

        #endregion

        #region Private Methods

        private DataRetrievalInput<AnalyticQuery> BuildAnalyticQuery(DateTime beginDate, Dictionary<PropertyName, string> propertyNames)
        {
            return new DataRetrievalInput<AnalyticQuery>()
            {
                Query = new AnalyticQuery()
                {
                    TableId = Guid.Parse("4C1AAA1B-675B-420F-8E60-26B0747CA79B"),
                    DimensionFields = new List<string> { propertyNames[PropertyName.HalfHour], propertyNames[PropertyName.OrigDeal], propertyNames[PropertyName.OrigDealZoneGroupNb], propertyNames[PropertyName.Deal], 
                                                         propertyNames[PropertyName.DealZoneGroupNb], propertyNames[PropertyName.DealTierNb], propertyNames[PropertyName.DealRateTierNb] },
                    MeasureFields = new List<string>() { propertyNames[PropertyName.PricedDurationInSec], propertyNames[PropertyName.DealDurationInSec] },
                    
                    FromTime = beginDate,
                    FilterGroup = new Vanrise.GenericData.Entities.RecordFilterGroup()
                    {
                        Filters = new List<Vanrise.GenericData.Entities.RecordFilter>() 
                        { 
                            new Vanrise.GenericData.Entities.NonEmptyRecordFilter()
                            {
                                FieldName =  propertyNames[PropertyName.OrigDeal]
                            }
                        }
                    }
                },
                SortByColumnName = "DimensionValues[0].Name"
            };
        }

        private DealBillingSummary BuildDealBillingSummary(bool isSale, DateTime batchStart, int dealId, int dealZoneGroupNb, decimal DurationInSeconds, int? dealTierNb, int? dealRateTierNb)
        {
            return new DealBillingSummary()
            {
                BatchStart = batchStart,
                DealId = dealId,
                ZoneGroupNb = dealZoneGroupNb,
                DurationInSeconds = DurationInSeconds,
                TierNb = dealTierNb,
                RateTierNb = dealRateTierNb,
                IsSale = isSale
            };
        }

        private Dictionary<PropertyName, string> BuildPropertyNames(string prefixPropName)
        {
            Dictionary<PropertyName, string> propertyNames = new Dictionary<PropertyName, string>();

            propertyNames.Add(PropertyName.HalfHour, "HalfHour");
            propertyNames.Add(PropertyName.OrigDeal, string.Format("Orig{0}Deal", prefixPropName));
            propertyNames.Add(PropertyName.OrigDealZoneGroupNb, string.Format("Orig{0}DealZoneGroupNb", prefixPropName));
            propertyNames.Add(PropertyName.PricedDurationInSec, string.Format("{0}DurationInSec", prefixPropName));
            propertyNames.Add(PropertyName.Deal, string.Format("{0}Deal", prefixPropName));
            propertyNames.Add(PropertyName.DealZoneGroupNb, string.Format("{0}DealZoneGroupNb", prefixPropName));
            propertyNames.Add(PropertyName.DealTierNb, string.Format("{0}DealTierNb", prefixPropName));
            propertyNames.Add(PropertyName.DealRateTierNb, string.Format("{0}DealRateTierNb", prefixPropName));
            propertyNames.Add(PropertyName.DealDurationInSec, string.Format("{0}DealDurationInSec", prefixPropName));

            return propertyNames;
        }

        private Dictionary<PropertyName, int> BuildDimensionFieldsDict(string prefixPropName)
        {
            Dictionary<PropertyName, int> propertyNames = new Dictionary<PropertyName, int>();

            propertyNames.Add(PropertyName.HalfHour, 0);
            propertyNames.Add(PropertyName.OrigDeal, 1);
            propertyNames.Add(PropertyName.OrigDealZoneGroupNb, 2);
            propertyNames.Add(PropertyName.Deal, 3);
            propertyNames.Add(PropertyName.DealZoneGroupNb, 4);
            propertyNames.Add(PropertyName.DealTierNb, 5);
            propertyNames.Add(PropertyName.DealRateTierNb, 6);

            return propertyNames;
        }

        #endregion

        #region Private Classes

        private enum PropertyName { HalfHour, OrigDeal, OrigDealZoneGroupNb, PricedDurationInSec, Deal, DealZoneGroupNb, DealTierNb, DealRateTierNb, DealDurationInSec }

        private struct BatchDealZoneGroup
        {
            public DateTime BatchStart { get; set; }

            public int DealId { get; set; }

            public int DealZoneGroupNb { get; set; }
        }

        private class DealDurations
        {
            public decimal DurationInSeconds { get; set; }

            public decimal DealDurationInSeconds { get; set; }
        }

        #endregion
    }
}