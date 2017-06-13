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

        public void LoadDealBillingSummaryRecords(DateTime beginDate, bool isSale, out Dictionary<DealZoneGroupTierRate, Dictionary<DateTime, DealBillingSummary>> currentDealBillingSummaryRecords,
             out Dictionary<DealZoneGroup, Dictionary<DateTime, BaseDealBillingSummary>> expectedBaseDealBillingSummaryRecords)
        {
            currentDealBillingSummaryRecords = null;
            expectedBaseDealBillingSummaryRecords = null;

            Dictionary<PropertyName, string> propertyNames = BuildPropertyNames(isSale == true ? "Sale" : "Cost");

            Vanrise.Entities.DataRetrievalInput<AnalyticQuery> analyticQuery = BuildAnalyticQuery(beginDate, propertyNames);

            var result = new AnalyticManager().GetFilteredRecords(analyticQuery) as AnalyticSummaryBigResult<AnalyticRecord>;
            if (result != null && result.Data != null && result.Data.Count() > 0)
            {
                currentDealBillingSummaryRecords = new Dictionary<DealZoneGroupTierRate, Dictionary<DateTime, DealBillingSummary>>();
                expectedBaseDealBillingSummaryRecords = new Dictionary<DealZoneGroup, Dictionary<DateTime, BaseDealBillingSummary>>();

                foreach (var analyticRecord in result.Data)
                {
                    DateTime batchStart = (DateTime)analyticRecord.MeasureValues.GetRecord(propertyNames[PropertyName.HalfHour]).Value;
                    int origDealId = (int)analyticRecord.MeasureValues.GetRecord(propertyNames[PropertyName.OrigDeal]).Value;
                    int origDealZoneGroupNb = (int)analyticRecord.MeasureValues.GetRecord(propertyNames[PropertyName.OrigDealZoneGroupNb]).Value;
                    decimal durationInSeconds = (decimal)analyticRecord.MeasureValues.GetRecord(propertyNames[PropertyName.DurationInSec]).Value;
                    int dealId = (int)analyticRecord.MeasureValues.GetRecord(propertyNames[PropertyName.Deal]).Value;
                    int dealZoneGroupNb = (int)analyticRecord.MeasureValues.GetRecord(propertyNames[PropertyName.DealZoneGroupNb]).Value;
                    decimal dealDurationInSeconds = (decimal)analyticRecord.MeasureValues.GetRecord(propertyNames[PropertyName.DealDurationInSec]).Value;
                    int dealTierNb = (int)analyticRecord.MeasureValues.GetRecord(propertyNames[PropertyName.DealTierNb]).Value;
                    int dealRateTierNb = (int)analyticRecord.MeasureValues.GetRecord(propertyNames[PropertyName.DealRateTierNb]).Value;

                    DealZoneGroupTierRate dealZoneGroupTierRate = new DealZoneGroupTierRate() { DealId = dealId, ZoneGroupNb = dealZoneGroupNb, TierNb = dealTierNb, RateTierNb = dealRateTierNb };
                    DealBillingSummary dealBillingSummary = BuildDealBillingSummary(isSale, batchStart, dealId, dealZoneGroupNb, dealDurationInSeconds, dealTierNb, dealRateTierNb);
                    BuildCurrentDealBillingSummaryRecord(currentDealBillingSummaryRecords, dealZoneGroupTierRate, dealBillingSummary);

                    DealZoneGroup dealZoneGroup = new DealZoneGroup() { DealId = origDealId, ZoneGroupNb = origDealZoneGroupNb };
                    BaseDealBillingSummary baseDealBillingSummary = BuildBaseDealBillingSummary(isSale, batchStart, origDealId, origDealZoneGroupNb, durationInSeconds);
                    BuildExpectedDealBillingSummaryRecord(expectedBaseDealBillingSummaryRecords, dealZoneGroup, baseDealBillingSummary);
                }
            }
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
                    MeasureFields = new List<string>() { propertyNames[PropertyName.DurationInSec], propertyNames[PropertyName.DurationInSec] },
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

        private DealBillingSummary BuildDealBillingSummary(bool isSale, DateTime batchStart, int dealId, int dealZoneGroupNb, decimal dealDurationInSeconds, int dealTierNb, int dealRateTierNb)
        {
            return new DealBillingSummary()
            {
                BatchStart = batchStart,
                DealId = dealId,
                DealZoneGroupNb = dealZoneGroupNb,
                DurationInSeconds = dealDurationInSeconds,
                DealTierNb = dealTierNb,
                DealRateTierNb = dealRateTierNb,
                IsSale = isSale
            };
        }

        private BaseDealBillingSummary BuildBaseDealBillingSummary(bool isSale, DateTime batchStart, int origDealId, int origDealZoneGroupNb, decimal durationInSeconds)
        {
            return new BaseDealBillingSummary()
            {
                BatchStart = batchStart,
                DealId = origDealId,
                DealZoneGroupNb = origDealZoneGroupNb,
                DurationInSeconds = durationInSeconds,
                IsSale = isSale
            };
        }

        private void BuildCurrentDealBillingSummaryRecord(Dictionary<DealZoneGroupTierRate, Dictionary<DateTime, DealBillingSummary>> currentDealBillingSummaryRecords, DealZoneGroupTierRate dealZoneGroupTierRate, DealBillingSummary dealBillingSummary)
        {
            Dictionary<DateTime, DealBillingSummary> currentDealBillingSummaryRecordsByBatchStart;

            if (!currentDealBillingSummaryRecords.TryGetValue(dealZoneGroupTierRate, out currentDealBillingSummaryRecordsByBatchStart))
            {
                currentDealBillingSummaryRecordsByBatchStart = new Dictionary<DateTime, DealBillingSummary>();
                currentDealBillingSummaryRecordsByBatchStart.Add(dealBillingSummary.BatchStart, dealBillingSummary);
                currentDealBillingSummaryRecords.Add(dealZoneGroupTierRate, currentDealBillingSummaryRecordsByBatchStart);
            }
            else
            {
                currentDealBillingSummaryRecordsByBatchStart.Add(dealBillingSummary.BatchStart, dealBillingSummary);
            }
        }

        private void BuildExpectedDealBillingSummaryRecord(Dictionary<DealZoneGroup, Dictionary<DateTime, BaseDealBillingSummary>> expectedDealBillingSummaryRecords, DealZoneGroup dealZoneGroup, BaseDealBillingSummary baseDealBillingSummary)
        {
            Dictionary<DateTime, BaseDealBillingSummary> expectedDealBillingSummaryRecordsByBatchStart;
            BaseDealBillingSummary tempBaseDealBillingSummary;

            if (!expectedDealBillingSummaryRecords.TryGetValue(dealZoneGroup, out expectedDealBillingSummaryRecordsByBatchStart))
            {
                expectedDealBillingSummaryRecordsByBatchStart = new Dictionary<DateTime, BaseDealBillingSummary>();
                expectedDealBillingSummaryRecordsByBatchStart.Add(baseDealBillingSummary.BatchStart, baseDealBillingSummary);
                expectedDealBillingSummaryRecords.Add(dealZoneGroup, expectedDealBillingSummaryRecordsByBatchStart);
            }
            else
            {
                if (!expectedDealBillingSummaryRecordsByBatchStart.TryGetValue(baseDealBillingSummary.BatchStart, out tempBaseDealBillingSummary))
                    expectedDealBillingSummaryRecordsByBatchStart.Add(baseDealBillingSummary.BatchStart, baseDealBillingSummary);
                else
                    tempBaseDealBillingSummary.DurationInSeconds += baseDealBillingSummary.DurationInSeconds;
            }
        }

        private enum PropertyName { HalfHour, OrigDeal, OrigDealZoneGroupNb, DurationInSec, Deal, DealZoneGroupNb, DealTierNb, DealRateTierNb, DealDurationInSec }

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
    }
}
