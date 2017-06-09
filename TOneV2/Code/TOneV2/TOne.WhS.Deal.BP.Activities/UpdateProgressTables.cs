using System;
using System.Collections.Generic;
using System.Activities;
using TOne.WhS.Deal.Entities;
using TOne.WhS.Deal.Business;
using Vanrise.BusinessProcess;
using System.Linq;

namespace TOne.WhS.Deal.BP.Activities
{
    public class UpdateProgressTablesInput
    {
        public DateTime BeginDate { get; set; }

        public Boolean IsSale { get; set; }

        public Dictionary<DealZoneGroupTierRate, Dictionary<DateTime, DealBillingSummary>> CurrentDealBillingSummaryRecords { get; set; }
    }

    public sealed class UpdateProgressTables : BaseAsyncActivity<UpdateProgressTablesInput>
    {
        public InArgument<DateTime> BeginDate { get; set; }

        public InArgument<Boolean> IsSale { get; set; }

        public InArgument<Dictionary<DealZoneGroupTierRate, Dictionary<DateTime, DealBillingSummary>>> CurrentDealBillingSummaryRecords { get; set; }

        protected override void DoWork(UpdateProgressTablesInput inputArgument, AsyncActivityHandle handle)
        {
            DealDetailedProgressManager dealDetailedProgressManager = new DealDetailedProgressManager();
            dealDetailedProgressManager.UpdateDealDetailedProgressTable(inputArgument.IsSale, inputArgument.BeginDate, inputArgument.CurrentDealBillingSummaryRecords);
             
            DealProgressManager dealProgressManager = new DealProgressManager();
            dealProgressManager.UpdateDealProgressTable(inputArgument.IsSale, inputArgument.BeginDate, inputArgument.CurrentDealBillingSummaryRecords);
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
    }
}
