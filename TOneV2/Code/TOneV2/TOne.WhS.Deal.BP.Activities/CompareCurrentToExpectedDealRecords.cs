using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Vanrise.BusinessProcess;
using TOne.WhS.Deal.Entities;
using TOne.WhS.Deal.Business;

namespace TOne.WhS.Deal.BP.Activities
{
    public class CompareCurrentToExpectedDealRecordsInput
    {
        public Dictionary<DealZoneGroupTierRate, Dictionary<DateTime, DealBillingSummary>> CurrentDealBillingSummaryRecords { get; set; }

        public Dictionary<DealZoneGroupTierRate, Dictionary<DateTime, DealBillingSummary>> ExpectedDealBillingSummaryRecords { get; set; }
    }

    public class CompareCurrentToExpectedDealRecordsOutput
    {
        public HashSet<DateTime> DaysToReprocess { get; set; }
    }

    public sealed class CompareCurrentToExpectedDealRecords : BaseAsyncActivity<CompareCurrentToExpectedDealRecordsInput, CompareCurrentToExpectedDealRecordsOutput>
    {
        public InArgument<Dictionary<DealZoneGroupTierRate, Dictionary<DateTime, DealBillingSummary>>> CurrentDealBillingSummaryRecords { get; set; }

        public InArgument<Dictionary<DealZoneGroupTierRate, Dictionary<DateTime, DealBillingSummary>>> ExpectedDealBillingSummaryRecords { get; set; }

        public OutArgument<HashSet<DateTime>> DaysToReprocess { get; set; }

        protected override CompareCurrentToExpectedDealRecordsOutput DoWorkWithResult(CompareCurrentToExpectedDealRecordsInput inputArgument, AsyncActivityHandle handle)
        {
            DealReprocessInputManager dealReprocessInputManager = new DealReprocessInputManager();
            List<DealReprocessInput> itemsToAdd = new List<DealReprocessInput>();

            Dictionary<DateTime, DealBillingSummary> dealBillingSummaryByBatchStart;
            DealBillingSummary dealBillingSummary;

            foreach (var kvp_expectedDealBillingSummaryRecord in inputArgument.ExpectedDealBillingSummaryRecords)
            {
                DealZoneGroupTierRate dealZoneGroupTierRate = kvp_expectedDealBillingSummaryRecord.Key;
                Dictionary<DateTime, DealBillingSummary> expectedDealBillingSummaryByBatchStart = kvp_expectedDealBillingSummaryRecord.Value;

                if (!inputArgument.CurrentDealBillingSummaryRecords.TryGetValue(dealZoneGroupTierRate, out dealBillingSummaryByBatchStart))
                {
                    itemsToAdd.AddRange(expectedDealBillingSummaryByBatchStart.Values.Select(itm => dealReprocessInputManager.BuildDealReprocessInput(itm)));
                }
                else
                {
                    foreach (var dealBillingSummaryRecordByBatchStart in expectedDealBillingSummaryByBatchStart)
                    {
                        DateTime batchStart = dealBillingSummaryRecordByBatchStart.Key;
                        DealBillingSummary expectedDealBillingSummary = dealBillingSummaryRecordByBatchStart.Value;

                        if (!dealBillingSummaryByBatchStart.TryGetValue(batchStart, out dealBillingSummary) || !expectedDealBillingSummary.IsEqual(dealBillingSummary))
                            itemsToAdd.Add(dealReprocessInputManager.BuildDealReprocessInput(expectedDealBillingSummary));
                    }
                }
            }

            dealReprocessInputManager.InsertDealReprocessInputs(itemsToAdd);

            if (itemsToAdd.Count == 0)
                return null;

            return new CompareCurrentToExpectedDealRecordsOutput()
            {
                DaysToReprocess = new HashSet<DateTime>(itemsToAdd.Select(itm => itm.FromTime.Date))
            };
        }

        protected override CompareCurrentToExpectedDealRecordsInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new CompareCurrentToExpectedDealRecordsInput
            {
                CurrentDealBillingSummaryRecords = this.CurrentDealBillingSummaryRecords.Get(context),
                ExpectedDealBillingSummaryRecords = this.ExpectedDealBillingSummaryRecords.Get(context)
            };
        }

        protected override void OnWorkComplete(AsyncCodeActivityContext context, CompareCurrentToExpectedDealRecordsOutput result)
        {
            this.DaysToReprocess.Set(context, result.DaysToReprocess);
        }
    }
}
