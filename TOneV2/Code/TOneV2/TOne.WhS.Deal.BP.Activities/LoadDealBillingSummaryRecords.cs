using System;
using System.Activities;
using System.Collections.Generic;
using TOne.WhS.Deal.Business;
using TOne.WhS.Deal.Entities;
using Vanrise.BusinessProcess;

namespace TOne.WhS.Deal.BP.Activities
{
    public class LoadDealBillingSummaryRecordsInput
    {
        public DateTime BeginDate { get; set; }

        public Boolean IsSale { get; set; }
    }

    public class LoadDealBillingSummaryRecordsOutput
    {
        public Dictionary<DealZoneGroupTierRate, Dictionary<DateTime, DealBillingSummary>> CurrentDealBillingSummaryRecords { get; set; }

        public Dictionary<DealZoneGroup, Dictionary<DateTime, BaseDealBillingSummary>> ExpectedBaseDealBillingSummaryRecords { get; set; }
    }

    public sealed class LoadDealBillingSummaryRecords : BaseAsyncActivity<LoadDealBillingSummaryRecordsInput, LoadDealBillingSummaryRecordsOutput>
    {
        public InArgument<DateTime> BeginDate { get; set; }

        public InArgument<Boolean> IsSale { get; set; }

        public OutArgument<Dictionary<DealZoneGroupTierRate, Dictionary<DateTime, DealBillingSummary>>> CurrentDealBillingSummaryRecords { get; set; }

        public OutArgument<Dictionary<DealZoneGroup, Dictionary<DateTime, BaseDealBillingSummary>>> ExpectedBaseDealBillingSummaryRecords { get; set; }


        protected override LoadDealBillingSummaryRecordsOutput DoWorkWithResult(LoadDealBillingSummaryRecordsInput inputArgument, AsyncActivityHandle handle)
        {
            Dictionary<DealZoneGroupTierRate, Dictionary<DateTime, DealBillingSummary>> currentDealBillingSummaryRecords;
            Dictionary<DealZoneGroup, Dictionary<DateTime, BaseDealBillingSummary>> expectedBaseDealBillingSummaryRecords;

            DealBillingSummaryManager dealBillingSummaryManager = new DealBillingSummaryManager();
            dealBillingSummaryManager.LoadDealBillingSummaryRecords(inputArgument.BeginDate, inputArgument.IsSale, out currentDealBillingSummaryRecords, out expectedBaseDealBillingSummaryRecords);

            return new LoadDealBillingSummaryRecordsOutput()
            {
                CurrentDealBillingSummaryRecords = currentDealBillingSummaryRecords,
                ExpectedBaseDealBillingSummaryRecords = expectedBaseDealBillingSummaryRecords
            };
        }

        protected override LoadDealBillingSummaryRecordsInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new LoadDealBillingSummaryRecordsInput
            {
                BeginDate = this.BeginDate.Get(context),
                IsSale = this.IsSale.Get(context),
            };
        }

        protected override void OnWorkComplete(AsyncCodeActivityContext context, LoadDealBillingSummaryRecordsOutput result)
        {
            this.CurrentDealBillingSummaryRecords.Set(context, result.CurrentDealBillingSummaryRecords);
            this.ExpectedBaseDealBillingSummaryRecords.Set(context, result.ExpectedBaseDealBillingSummaryRecords);
        }
    }
}