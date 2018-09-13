using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Invoice.Entities;

namespace Vanrise.Invoice.MainExtensions
{
    public class QuarterBillingPeriod : BillingPeriod
    {
        public override Guid ConfigId
        {
            get { return new Guid("93330DE5-EEC8-461C-988B-E4E0C22BC541"); }
        }

        public override List<BillingInterval> GetPeriod(IBillingPeriodContext context)
        {
            List<BillingInterval> billingIntervalList = new List<BillingInterval>();
           
            var fromDate = new DateTime(context.IssueDate.Year, context.IssueDate.Month, 1);
            var toDate = new DateTime(context.IssueDate.Year, context.IssueDate.Month, 1);

            var perviousBillingInterval = new BillingInterval
            {
                FromDate = fromDate.AddMonths(-3),
                ToDate = toDate.AddDays(-1)
            };
            billingIntervalList.Add(perviousBillingInterval);
           
            return billingIntervalList;
        }
        public override string GetDescription()
        {
            return "Quarter";
        }
    }
}
