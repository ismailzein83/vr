using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using TOne.WhS.Deal.Business;
using TOne.WhS.Deal.Entities;
using Vanrise.BusinessProcess;
using Vanrise.Common;

namespace TOne.WhS.Deal.BP.Activities
{
    public class UpdateProgressTablesInput
    {
        public DateTime BeginDate { get; set; }

        public Boolean IsSale { get; set; }

        public Dictionary<DealZoneGroupTierRate, Dictionary<DateTime, DealBillingSummary>> CurrentDealBillingSummaryRecords { get; set; }
    }

    public class UpdateProgressTablesOutput
    {
        public Dictionary<DealZoneGroup, DealProgress> DealProgressesBeforeUpdate  { get; set; } 
    }

    public sealed class UpdateProgressTables : BaseAsyncActivity<UpdateProgressTablesInput, UpdateProgressTablesOutput>
    {
        public InArgument<DateTime> BeginDate { get; set; }

        public InArgument<Boolean> IsSale { get; set; }

        public InArgument<Dictionary<DealZoneGroupTierRate, Dictionary<DateTime, DealBillingSummary>>> CurrentDealBillingSummaryRecords { get; set; }

        public OutArgument<Dictionary<DealZoneGroup, DealProgress>> DealProgressesBeforeUpdate { get; set; } 

        protected override UpdateProgressTablesOutput DoWorkWithResult(UpdateProgressTablesInput inputArgument, AsyncActivityHandle handle)
        {
            DealDetailedProgressManager dealDetailedProgressManager = new DealDetailedProgressManager();
            dealDetailedProgressManager.UpdateDealDetailedProgressTable(inputArgument.IsSale, inputArgument.BeginDate, inputArgument.CurrentDealBillingSummaryRecords);

            DealProgressManager dealProgressManager = new DealProgressManager();
            HashSet<DealZoneGroup> existingDealZoneGroups = inputArgument.CurrentDealBillingSummaryRecords.Keys.Select(itm => new DealZoneGroup() { DealId = itm.DealId, ZoneGroupNb = itm.ZoneGroupNb }).ToHashSet();
            Dictionary<DealZoneGroup, DealProgress> dealProgressByZoneGroup = dealProgressManager.GetDealProgresses(existingDealZoneGroups, inputArgument.IsSale);
            dealProgressManager.UpdateDealProgressTable(inputArgument.IsSale, inputArgument.BeginDate, dealProgressByZoneGroup, inputArgument.CurrentDealBillingSummaryRecords);

            return new UpdateProgressTablesOutput()
            {
                DealProgressesBeforeUpdate = dealProgressByZoneGroup
            };
        }

        protected override UpdateProgressTablesInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new UpdateProgressTablesInput
            {
                BeginDate = this.BeginDate.Get(context),
                IsSale = this.IsSale.Get(context),
                CurrentDealBillingSummaryRecords = this.CurrentDealBillingSummaryRecords.Get(context),
            };
        }

        protected override void OnWorkComplete(AsyncCodeActivityContext context, UpdateProgressTablesOutput result)
        {
            this.DealProgressesBeforeUpdate.Set(context, result.DealProgressesBeforeUpdate);
        }
    }
}
