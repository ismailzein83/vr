using System;
using System.Activities;
using System.Collections.Generic;
using TOne.WhS.Deal.Business;
using TOne.WhS.Deal.Entities;
using Vanrise.BusinessProcess;

namespace TOne.WhS.Deal.BP.Activities
{
    public class LoadBillingSummaryRecordsInput
    {
        public Boolean IsSale { get; set; }

        public DateTime BeginDate { get; set; }
    }

    public class LoadBillingSummaryRecordsOutput
    {
        public Dictionary<DealZoneGroup, DealBillingSummary> CurrentDealBillingSummaryRecords { get; set; }

        public Dictionary<DealZoneGroup, BaseDealBillingSummary> ExpectedDealBillingSummaryRecords { get; set; } 
    }

    public sealed class LoadBillingSummaryRecords : BaseAsyncActivity<LoadBillingSummaryRecordsInput, LoadBillingSummaryRecordsOutput> 
    {
        public InArgument<Boolean> IsSale { get; set; } 

        public InArgument<DateTime> BeginDate { get; set; }

        public OutArgument<Dictionary<DealZoneGroup, DealBillingSummary>> CurrentDealBillingSummaryRecords { get; set; }

        public OutArgument<Dictionary<DealZoneGroup, BaseDealBillingSummary>> ExpectedDealBillingSummaryRecords { get; set; }


        protected override LoadBillingSummaryRecordsOutput DoWorkWithResult(LoadBillingSummaryRecordsInput inputArgument, AsyncActivityHandle handle)
        {
            Dictionary<DealZoneGroup, DealBillingSummary> currentDealBillingSummaryRecords = new Dictionary<DealZoneGroup, DealBillingSummary>();
            Dictionary<DealZoneGroup, BaseDealBillingSummary> expectedDealBillingSummaryRecords = new Dictionary<DealZoneGroup, BaseDealBillingSummary>();

            new DealBillingSummaryManager().GetCurrentAndExpectedDealBillingSummaryRecords(inputArgument.BeginDate, inputArgument.IsSale, currentDealBillingSummaryRecords, expectedDealBillingSummaryRecords);

            return new LoadBillingSummaryRecordsOutput()
            {
                CurrentDealBillingSummaryRecords = currentDealBillingSummaryRecords,
                ExpectedDealBillingSummaryRecords = expectedDealBillingSummaryRecords
            };
        }

        protected override LoadBillingSummaryRecordsInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new LoadBillingSummaryRecordsInput
            {
                BeginDate = this.BeginDate.Get(context),
                IsSale = this.IsSale.Get(context),
            };
        }

        protected override void OnWorkComplete(AsyncCodeActivityContext context, LoadBillingSummaryRecordsOutput result)
        {
            this.CurrentDealBillingSummaryRecords.Set(context, result.CurrentDealBillingSummaryRecords); 
            this.ExpectedDealBillingSummaryRecords.Set(context, result.ExpectedDealBillingSummaryRecords);
        }
    }
} 