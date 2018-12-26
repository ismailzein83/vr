using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Invoice.Entities;

namespace Vanrise.Invoice.MainExtensions
{
    public class PeriodBillingPeriod : BillingPeriod
    {
        public override Guid ConfigId { get { return new Guid("3D0DD9A7-3422-4311-B16F-08B03F175FAE"); } }
        public int NumberOfDays { get; set; }
        public override List<BillingInterval> GetPeriod(IBillingPeriodContext context)
        {
            List<BillingInterval> billingIntervalList = new List<BillingInterval>();

            BillingInterval billingInterval = new Entities.BillingInterval();
            billingInterval.ToDate = context.IssueDate.Date.AddMilliseconds(-2);
            billingInterval.FromDate = billingInterval.ToDate.AddDays(-NumberOfDays + 1).Date;
            billingIntervalList.Add(billingInterval);
            return billingIntervalList;
        }
        private BillingInterval GetIntervalIfPreviousPeriodNotValid(DateTime issueDate, int numberOfDays)
        {
            BillingInterval perviousBillingInterval = new BillingInterval();
            perviousBillingInterval.ToDate = issueDate.AddDays(-1);
            perviousBillingInterval.FromDate = perviousBillingInterval.ToDate.AddDays(-numberOfDays);
            return perviousBillingInterval;
        }
        public override string GetDescription()
        {
            return string.Format("Period: {0} days",this.NumberOfDays);
        }
    }
}
