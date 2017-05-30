using System;
using System.Collections.Generic;
using System.Activities;
using TOne.WhS.Deal.Entities;
using TOne.WhS.Deal.Business;
using Vanrise.BusinessProcess;

namespace TOne.WhS.Deal.BP.Activities
{
    public class UpdateProgressTablesInput
    {
        public DateTime BeginDate { get; set; }

        public Dictionary<DealZoneGroup, DealBillingSummary> CurrentDealBillingSummaryRecords { get; set; }
    }

    public sealed class UpdateProgressTables : BaseAsyncActivity<UpdateProgressTablesInput>
    {
        public InArgument<DateTime> BeginDate { get; set; }

        public InArgument<Dictionary<DealZoneGroup, DealBillingSummary>> CurrentDealBillingSummaryRecords { get; set; }

        protected override void DoWork(UpdateProgressTablesInput inputArgument, AsyncActivityHandle handle)
        {


        }

        protected override UpdateProgressTablesInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new UpdateProgressTablesInput
            {
                BeginDate = this.BeginDate.Get(context),
                CurrentDealBillingSummaryRecords = this.CurrentDealBillingSummaryRecords.Get(context),
            };
        }
    }
}
