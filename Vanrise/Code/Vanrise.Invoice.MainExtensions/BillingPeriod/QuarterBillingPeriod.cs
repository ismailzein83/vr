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

            var fromDate = QuarterStart(context.IssueDate);
            var toDate = fromDate.AddMonths(3).AddDays(-1);
            while (toDate.Date >= context.IssueDate.Date)
            {
                fromDate = QuarterStart(fromDate.AddDays(-1));
                toDate = fromDate.AddMonths(3).AddDays(-1);
            }
            var perviousBillingInterval = new BillingInterval
            {
                FromDate = fromDate,
                ToDate = toDate
            };
            billingIntervalList.Add(perviousBillingInterval);
           
            return billingIntervalList;
        }


        public DateTime QuarterStart(DateTime issueDate)
        {
            int startingMonth = (issueDate.Month - 1) / 3;
            startingMonth *= 3;
            startingMonth++;
            return new DateTime(issueDate.Year, startingMonth, 1);
        }
        public override string GetDescription()
        {
            return "Quarter";
        }
    }
}
