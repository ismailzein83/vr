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

            BillingInterval perviousBillingInterval = new Entities.BillingInterval();
            int numberOfDays = this.NumberOfDays - 1;
            if (context.PreviousPeriodEndDate.HasValue)
            {
                BillingInterval nextBillingInterval = new Entities.BillingInterval();
                nextBillingInterval.FromDate = context.PreviousPeriodEndDate.Value;

                nextBillingInterval.ToDate = nextBillingInterval.FromDate.AddDays(numberOfDays);
             
                if (nextBillingInterval.ToDate > context.IssueDate)
                {
                    perviousBillingInterval = GetIntervalIfPreviousPeriodNotValid(context.IssueDate, numberOfDays);
                }
                else
                {
                    perviousBillingInterval.FromDate = nextBillingInterval.FromDate;
                    perviousBillingInterval.ToDate = nextBillingInterval.ToDate;
                    while (nextBillingInterval.ToDate <= context.IssueDate && nextBillingInterval.ToDate < DateTime.Today)
                    {
                        perviousBillingInterval.FromDate = nextBillingInterval.FromDate;
                        perviousBillingInterval.ToDate = nextBillingInterval.ToDate;
                        nextBillingInterval.FromDate = nextBillingInterval.ToDate.AddDays(1);
                        nextBillingInterval.ToDate = nextBillingInterval.FromDate.AddDays(numberOfDays);
                    }
                }
            }else
            {
                perviousBillingInterval = GetIntervalIfPreviousPeriodNotValid(context.IssueDate, numberOfDays);
            }
            billingIntervalList.Add(perviousBillingInterval);
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
