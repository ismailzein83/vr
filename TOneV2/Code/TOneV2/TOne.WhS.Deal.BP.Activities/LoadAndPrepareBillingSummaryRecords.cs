using System;
using System.Collections.Generic;
using System.Activities;
using Vanrise.Analytic.Business;
using Vanrise.Analytic.Entities;
using Vanrise.Entities;
using Vanrise.GenericData.Entities;
using TOne.WhS.Deal.Entities;
using Vanrise.Common;

namespace TOne.WhS.Deal.BP.Activities
{
    public sealed class LoadAndPrepareBillingSummaryRecords : CodeActivity
    {
        public InArgument<DateTime> BeginDate { get; set; }

        public OutArgument<Dictionary<DealZoneGroup, DealBillingSummaryRecord>> CurrentDealBillingSummaryRecords { get; set; }

        public OutArgument<Dictionary<DealZoneGroup, DealBillingSummaryRecord>> ExpectedDealBillingSummaryRecords { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            Vanrise.Entities.DataRetrievalInput<AnalyticQuery> analyticQuery = new DataRetrievalInput<AnalyticQuery>()
            {
                Query = new AnalyticQuery()
                {
                    DimensionFields = new List<string> { "HalfHour", "OrigSaleDeal", "OrigSaleDealZoneGroupNb", "SaleDeal", "SaleDealZoneGroupNb", "SaleDurationInSeconds", "SaleDealDurInSec", "SaleDealTierNb", "SaleDealRateTierNb" },
                    
                    //MeasureFields = new List<string>() { "AverageCost", "SaleDuration" },
                    TableId = 8,
                    FromTime = this.BeginDate.Get(context),
                    //CurrencyId = parameters.CurrencyId,
                    ParentDimensions = new List<string>(),
                    Filters = new List<DimensionFilter>(),
                    
                },
                //SortByColumnName = "DimensionValues[0].Name"
            };
            string direction = "Sale";
            Dictionary<DealZoneGroup, DealBillingSummaryRecord> currentDealBillingSummaryRecords = new Dictionary<DealZoneGroup, DealBillingSummaryRecord>();
            Dictionary<DealZoneGroup, BaseDealBillingSummaryRecord> expectedDealBillingSummaryRecords = new Dictionary<DealZoneGroup, BaseDealBillingSummaryRecord>();

            var result = new AnalyticManager().GetFilteredRecords(analyticQuery) as AnalyticSummaryBigResult<AnalyticRecord>;
            if (result != null)
            {
                foreach (var analyticRecord in result.Data)
                {
                    DateTime time = (DateTime)analyticRecord.MeasureValues.GetRecord("HalfHour").Value;
                    int saleDealId = (int)analyticRecord.MeasureValues.GetRecord("SaleDeal").Value;
                    int saleDealZoneGroupNb = (int)analyticRecord.MeasureValues.GetRecord("SaleDealZoneGroupNb").Value;
                    int origDealId = (int)analyticRecord.MeasureValues.GetRecord(string.Format("Orig{0}Deal", direction)).Value;
                    int origDealZoneGroupNb = (int)analyticRecord.MeasureValues.GetRecord("OrigSaleDealZoneGroupNb").Value;

                    DealZoneGroup currentDealKeyData = new DealZoneGroup() { DealId = saleDealId, ZoneGroupNb = saleDealZoneGroupNb };
                    if (!currentDealBillingSummaryRecords.ContainsKey(currentDealKeyData))
                    {
                        currentDealBillingSummaryRecords.Add(currentDealKeyData, new DealBillingSummaryRecord()
                        {
                            BatchStart = time,
                            DealId = saleDealId,
                            DealZoneGroupNb = saleDealZoneGroupNb,
                            DurationInSeconds = (decimal)analyticRecord.MeasureValues.GetRecord("SaleDealDurInSec").Value,
                            DealTierNb = (int)analyticRecord.MeasureValues.GetRecord("SaleDealTierNb").Value,
                            DealRateTierNb = (int)analyticRecord.MeasureValues.GetRecord("SaleDealRateTierNb").Value
                        });
                    }

                    BaseDealBillingSummaryRecord dealBillingSummaryRecord;
                    DealZoneGroup expectedDealKeyData = new DealZoneGroup() { DealId = origDealId, ZoneGroupNb = origDealZoneGroupNb };
                    if (expectedDealBillingSummaryRecords.TryGetValue(expectedDealKeyData, out dealBillingSummaryRecord))
                    {
                        dealBillingSummaryRecord.DurationInSeconds += (decimal)analyticRecord.MeasureValues.GetRecord("SaleDurationInSeconds").Value;
                    }
                    else
                    {
                        expectedDealBillingSummaryRecords.Add(expectedDealKeyData, new DealBillingSummaryRecord()
                        {
                            BatchStart = time,
                            DealId = (int)analyticRecord.MeasureValues.GetRecord("OrigSaleDeal").Value,
                            DealZoneGroupNb = (int)analyticRecord.MeasureValues.GetRecord("OrigSaleDealZoneGroupNb").Value,
                            DurationInSeconds = (decimal)analyticRecord.MeasureValues.GetRecord("SaleDurationInSeconds").Value,
                        });
                    }
                }
            }

            this.CurrentDealBillingSummaryRecords.Set(context, currentDealBillingSummaryRecords);
            this.ExpectedDealBillingSummaryRecords.Set(context, expectedDealBillingSummaryRecords);
        }
    }
}