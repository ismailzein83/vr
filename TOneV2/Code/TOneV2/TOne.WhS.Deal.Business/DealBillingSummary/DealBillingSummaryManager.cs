using System;
using System.Collections.Generic;
using TOne.WhS.Deal.Entities;
using Vanrise.Analytic.Business;
using Vanrise.Analytic.Entities;
using Vanrise.Entities;
using Vanrise.Common;

namespace TOne.WhS.Deal.Business
{
    public class DealBillingSummaryManager
    {
        public void GetCurrentAndExpectedDealBillingSummaryRecords(DateTime beginDate, bool isSale, Dictionary<DealZoneGroup, DealBillingSummary> currentDealBillingSummaryRecords, Dictionary<DealZoneGroup, BaseDealBillingSummary> expectedDealBillingSummaryRecords)
        {
            string trafficDirection = isSale == true ? "Sale" : "Cost";

            string halfHourFieldName = "halfHour";
            string origDealFieldName = string.Format("Orig{0}Deal", trafficDirection);
            string origDealZoneGroupNbFieldName = string.Format("Orig{0}DealZoneGroupNb", trafficDirection);
            string durationInSecFieldName = string.Format("{0}DurationInSec", trafficDirection);
            string dealFieldName = string.Format("{0}Deal", trafficDirection);
            string dealZoneGroupNbFieldName = string.Format("{0}DealZoneGroupNb", trafficDirection);
            string dealTierNb = string.Format("{0}DealTierNb", trafficDirection);
            string dealRateTierNb = string.Format("{0}DealRateTierNb", trafficDirection);
            string dealDurationInSecFieldName = string.Format("{0}DealDurationInSec", trafficDirection);

            Vanrise.Entities.DataRetrievalInput<AnalyticQuery> analyticQuery = new DataRetrievalInput<AnalyticQuery>()
            {
                Query = new AnalyticQuery()
                {
                    DimensionFields = new List<string> { halfHourFieldName, origDealFieldName, origDealZoneGroupNbFieldName, dealFieldName, dealZoneGroupNbFieldName, dealTierNb, dealRateTierNb },
                    MeasureFields = new List<string>() { durationInSecFieldName, dealDurationInSecFieldName },
                    TableId = 8,
                    FromTime = beginDate
                },
            };

            var result = new AnalyticManager().GetFilteredRecords(analyticQuery) as AnalyticSummaryBigResult<AnalyticRecord>;
            if (result != null)
            {
                foreach (var analyticRecord in result.Data)
                {
                    DateTime batchStart = (DateTime)analyticRecord.MeasureValues.GetRecord(halfHourFieldName).Value;
                    int origDealId = (int)analyticRecord.MeasureValues.GetRecord(origDealFieldName).Value;
                    int origDealZoneGroupNb = (int)analyticRecord.MeasureValues.GetRecord(origDealZoneGroupNbFieldName).Value;
                    int saleDealId = (int)analyticRecord.MeasureValues.GetRecord(dealFieldName).Value;
                    int saleDealZoneGroupNb = (int)analyticRecord.MeasureValues.GetRecord(dealZoneGroupNbFieldName).Value;

                    DealZoneGroup currentDealKeyData = new DealZoneGroup() { DealId = saleDealId, ZoneGroupNb = saleDealZoneGroupNb };
                    if (!currentDealBillingSummaryRecords.ContainsKey(currentDealKeyData))
                    {
                        currentDealBillingSummaryRecords.Add(currentDealKeyData, new DealBillingSummary()
                        {
                            BatchStart = batchStart,
                            DealId = saleDealId,
                            DealZoneGroupNb = saleDealZoneGroupNb,
                            DurationInSeconds = (decimal)analyticRecord.MeasureValues.GetRecord(dealDurationInSecFieldName).Value,
                            DealTierNb = (int)analyticRecord.MeasureValues.GetRecord(dealTierNb).Value,
                            DealRateTierNb = (int)analyticRecord.MeasureValues.GetRecord(dealRateTierNb).Value
                        });
                    }

                    BaseDealBillingSummary dealBillingSummaryRecord;
                    DealZoneGroup expectedDealKeyData = new DealZoneGroup() { DealId = origDealId, ZoneGroupNb = origDealZoneGroupNb };
                    if (expectedDealBillingSummaryRecords.TryGetValue(expectedDealKeyData, out dealBillingSummaryRecord))
                    {
                        dealBillingSummaryRecord.DurationInSeconds += (decimal)analyticRecord.MeasureValues.GetRecord(string.Format("{0}DurationInSeconds", trafficDirection)).Value;
                    }
                    else
                    {
                        expectedDealBillingSummaryRecords.Add(expectedDealKeyData, new DealBillingSummary()
                        {
                            BatchStart = batchStart,
                            DealId = (int)analyticRecord.MeasureValues.GetRecord(origDealFieldName).Value,
                            DealZoneGroupNb = (int)analyticRecord.MeasureValues.GetRecord(origDealZoneGroupNbFieldName).Value,
                            DurationInSeconds = (decimal)analyticRecord.MeasureValues.GetRecord(durationInSecFieldName).Value,
                        });
                    }
                }
            }
        }
    }
}
