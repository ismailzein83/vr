using System;
using System.Collections.Generic;
using System.Activities;
using TOne.WhS.Deal.Entities;
using TOne.WhS.Deal.Business;

namespace TOne.WhS.Deal.BP.Activities
{
    public sealed class UpdateProgressTables : CodeActivity
    {
        public InArgument<DateTime> BeginDate { get; set; }

        public InArgument<Dictionary<DealZoneGroup, DealBillingSummary>> CurrentDealBillingSummaryRecords { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            DateTime beginDate = this.BeginDate.Get(context);
            Dictionary<DealZoneGroup, DealBillingSummary> dealBillingSummaryRecords = this.CurrentDealBillingSummaryRecords.Get(context);

        }
    }
}
