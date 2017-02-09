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
        public override BillingInterval GetPeriod(IBillingPeriodContext context)
        {
            BillingInterval perviousBillingInterval = new Entities.BillingInterval();
            if (context.PreviousPeriodEndDate.HasValue)
            {
                BillingInterval nextBillingInterval = new Entities.BillingInterval();
                nextBillingInterval.FromDate = context.PreviousPeriodEndDate.Value;

                nextBillingInterval.ToDate = nextBillingInterval.FromDate.AddDays(this.NumberOfDays);
             
                if (nextBillingInterval.ToDate > context.IssueDate)
                {
                    perviousBillingInterval = GetIntervalIfPreviousPeriodNotValid(context.IssueDate);
                }
                else
                {
                    perviousBillingInterval.FromDate = nextBillingInterval.FromDate;
                    perviousBillingInterval.ToDate = nextBillingInterval.ToDate;
                    while (nextBillingInterval.ToDate <= context.IssueDate)
                    {
                        perviousBillingInterval.FromDate = nextBillingInterval.FromDate;
                        perviousBillingInterval.ToDate = nextBillingInterval.ToDate;
                        nextBillingInterval.FromDate = nextBillingInterval.ToDate.AddDays(1);
                        nextBillingInterval.ToDate = nextBillingInterval.FromDate.AddDays(this.NumberOfDays);
                    }
                }
            }else
            {
                perviousBillingInterval = GetIntervalIfPreviousPeriodNotValid(context.IssueDate);
            }
      
            return perviousBillingInterval;
        }
        private BillingInterval GetIntervalIfPreviousPeriodNotValid(DateTime issueDate)
        {
            BillingInterval perviousBillingInterval = new BillingInterval();
            perviousBillingInterval.ToDate = issueDate.AddDays(-1);
            perviousBillingInterval.FromDate = perviousBillingInterval.ToDate.AddDays(-this.NumberOfDays);
            return perviousBillingInterval;
        }
    }
}
