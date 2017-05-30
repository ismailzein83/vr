using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using TOne.WhS.Deal.Entities;

namespace TOne.WhS.Deal.BP.Activities
{
    public sealed class UpdateProgressTables : CodeActivity
    {
        public InArgument<DateTime> BeginDate { get; set; }

        public InArgument<Dictionary<DealKeyData, DealBillingSummaryRecord>> CurrentDealBillingSummaryRecords { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            DateTime beginDate = this.BeginDate.Get(context);
            Dictionary<DealKeyData, DealBillingSummaryRecord> dealBillingSummaryRecords = this.CurrentDealBillingSummaryRecords.Get(context);


        }
    }
}
