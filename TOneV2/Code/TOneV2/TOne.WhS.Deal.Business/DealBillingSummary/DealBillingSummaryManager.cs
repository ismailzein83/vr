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

            Vanrise.Entities.DataRetrievalInput<AnalyticQuery> analyticQuery = BuildAnalyticQuery(beginDate, propertyNames);

            var result = new AnalyticManager().GetFilteredRecords(analyticQuery) as AnalyticSummaryBigResult<AnalyticRecord>;
            if (result != null && result.Data != null && result.Data.Count() > 0)
            {
                dealBillingSummaryRecords = new Dictionary<DealZoneGroup, List<DealBillingSummary>>();
                batchDealZoneGroupDict = new Dictionary<BatchDealZoneGroup, DealDurations>();

                foreach (var analyticRecord in result.Data)
                {
                    DateTime batchStart = (DateTime)analyticRecord.MeasureValues.GetRecord(propertyNames[PropertyName.HalfHour]).Value;
                    int origDealId = (int)analyticRecord.MeasureValues.GetRecord(propertyNames[PropertyName.OrigDeal]).Value;
                    int origDealZoneGroupNb = (int)analyticRecord.MeasureValues.GetRecord(propertyNames[PropertyName.OrigDealZoneGroupNb]).Value;
                    decimal durationInSeconds = (decimal)analyticRecord.MeasureValues.GetRecord(propertyNames[PropertyName.DurationInSec]).Value;
                    int? dealId = (int?)analyticRecord.MeasureValues.GetRecord(propertyNames[PropertyName.Deal]).Value;
                    int? dealZoneGroupNb = (int?)analyticRecord.MeasureValues.GetRecord(propertyNames[PropertyName.DealZoneGroupNb]).Value;
                    decimal? dealDurationInSeconds = (decimal?)analyticRecord.MeasureValues.GetRecord(propertyNames[PropertyName.DealDurationInSec]).Value;
                    int? dealTierNb = (int?)analyticRecord.MeasureValues.GetRecord(propertyNames[PropertyName.DealTierNb]).Value;
                    int? dealRateTierNb = (int?)analyticRecord.MeasureValues.GetRecord(propertyNames[PropertyName.DealRateTierNb]).Value;

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
                    TableId = 8,
                    DimensionFields = new List<string> { propertyNames[PropertyName.HalfHour], propertyNames[PropertyName.OrigDeal], propertyNames[PropertyName.OrigDealZoneGroupNb], propertyNames[PropertyName.Deal], 
                                                         propertyNames[PropertyName.DealZoneGroupNb], propertyNames[PropertyName.DealTierNb], propertyNames[PropertyName.DealRateTierNb] },
                    MeasureFields = new List<string>() { propertyNames[PropertyName.DurationInSec], propertyNames[PropertyName.DealDurationInSec] },
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
                }
            };
        }

        private DealBillingSummary BuildDealBillingSummary(bool isSale, DateTime batchStart, int dealId, int dealZoneGroupNb, decimal DurationInSeconds, int? dealTierNb, int? dealRateTierNb)
        {
            return new DealBillingSummary()
            {
                BatchStart = batchStart,
                DealId = dealId,
                DealZoneGroupNb = dealZoneGroupNb,
                DurationInSeconds = DurationInSeconds,
                DealTierNb = dealTierNb,
                DealRateTierNb = dealRateTierNb,
                IsSale = isSale
            };
        }

        private Dictionary<PropertyName, string> BuildPropertyNames(string prefixPropName)
        {
            Dictionary<PropertyName, string> propertyNames = new Dictionary<PropertyName, string>();

            propertyNames.Add(PropertyName.HalfHour, "HalfHour");
            propertyNames.Add(PropertyName.OrigDeal, string.Format("Orig{0}Deal", prefixPropName));
            propertyNames.Add(PropertyName.OrigDealZoneGroupNb, string.Format("Orig{0}DealZoneGroupNb", prefixPropName));
            propertyNames.Add(PropertyName.DurationInSec, "DurationNetInSec");
            propertyNames.Add(PropertyName.Deal, string.Format("{0}Deal", prefixPropName));
            propertyNames.Add(PropertyName.DealZoneGroupNb, string.Format("{0}DealZoneGroupNb", prefixPropName));
            propertyNames.Add(PropertyName.DealTierNb, string.Format("{0}DealTierNb", prefixPropName));
            propertyNames.Add(PropertyName.DealRateTierNb, string.Format("{0}DealRateTierNb", prefixPropName));
            propertyNames.Add(PropertyName.DealDurationInSec, string.Format("{0}DealDurationInSec", prefixPropName));

            return propertyNames;
        }

        #endregion

        #region Private Classes

        private enum PropertyName { HalfHour, OrigDeal, OrigDealZoneGroupNb, DurationInSec, Deal, DealZoneGroupNb, DealTierNb, DealRateTierNb, DealDurationInSec }

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