using System;
using System.Activities;
using System.Collections.Generic;
using TOne.WhS.Deal.Business;
using TOne.WhS.Deal.Entities;
using Vanrise.BusinessProcess;
using Vanrise.Common;

namespace TOne.WhS.Deal.BP.Activities
{
    public class LoadDealBillingSummaryRecordsInput
    {
        public Boolean IsSale { get; set; }

        public DateTime BeginDate { get; set; }
    }

    public class LoadDealBillingSummaryRecordsOutput
    {
        public HashSet<DealZoneGroup> AffectedDealZoneGroups { get; set; }

        public Dictionary<DealZoneGroup, List<DealBillingSummary>> CurrentDealBillingSummaryRecords { get; set; }
    }

    public sealed class LoadDealBillingSummaryRecords : BaseAsyncActivity<LoadDealBillingSummaryRecordsInput, LoadDealBillingSummaryRecordsOutput>
    {
        [RequiredArgument]
        public InArgument<Boolean> IsSale { get; set; }

        [RequiredArgument]
        public InArgument<DateTime> BeginDate { get; set; }

        [RequiredArgument]
        public OutArgument<HashSet<DealZoneGroup>> AffectedDealZoneGroups { get; set; }

        [RequiredArgument]
        public OutArgument<Dictionary<DealZoneGroup, List<DealBillingSummary>>> CurrentDealBillingSummaryRecords { get; set; }

        protected override LoadDealBillingSummaryRecordsOutput DoWorkWithResult(LoadDealBillingSummaryRecordsInput inputArgument, AsyncActivityHandle handle)
        {
            DealBillingSummaryManager dealBillingSummaryManager = new DealBillingSummaryManager();
            Dictionary<DealZoneGroup, List<DealBillingSummary>> currentDealBillingSummaryRecords = dealBillingSummaryManager.LoadDealBillingSummaryRecords(inputArgument.BeginDate, inputArgument.IsSale);
            HashSet<DealZoneGroup> affectedDealZoneGroups = currentDealBillingSummaryRecords.Keys.ToHashSet();

            return new LoadDealBillingSummaryRecordsOutput()
            {
                AffectedDealZoneGroups = affectedDealZoneGroups,
                CurrentDealBillingSummaryRecords = currentDealBillingSummaryRecords
            };
        }

        protected override LoadDealBillingSummaryRecordsInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new LoadDealBillingSummaryRecordsInput
            {
                IsSale = this.IsSale.Get(context),
                BeginDate = this.BeginDate.Get(context)
            };
        }

        protected override void OnWorkComplete(AsyncCodeActivityContext context, LoadDealBillingSummaryRecordsOutput result)
        {
            this.AffectedDealZoneGroups.Set(context, result.AffectedDealZoneGroups);
            this.CurrentDealBillingSummaryRecords.Set(context, result.CurrentDealBillingSummaryRecords);
        }
    }
}