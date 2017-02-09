using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Invoice.Entities;

namespace Vanrise.Invoice.MainExtensions
{
    public class WeeklyBillingPeriod : BillingPeriod
    {
        public override Guid ConfigId { get { return new Guid("A08230D0-317E-4B30-A5EA-5ED72E2604D8"); } }
        public DayOfWeek DailyType { get; set; }
        public override BillingInterval GetPeriod(IBillingPeriodContext context)
        {
            BillingInterval perviousBillingInterval = new Entities.BillingInterval();
            if (context.PreviousPeriodEndDate.HasValue)
            {

                BillingInterval nextBillingInterval = new Entities.BillingInterval();
                nextBillingInterval.FromDate = context.PreviousPeriodEndDate.Value;
                if (nextBillingInterval.FromDate.DayOfWeek != DailyType)
                {
                    while (nextBillingInterval.FromDate.DayOfWeek != DailyType)
                    {
                        nextBillingInterval.FromDate = nextBillingInterval.FromDate.AddDays(1);
                    }
                }

                nextBillingInterval.ToDate = nextBillingInterval.FromDate.AddDays(6);
                if (nextBillingInterval.ToDate > context.IssueDate)
                {
                    perviousBillingInterval = GetIntervalIfPreviousPeriodNotValid(context.IssueDate);
                }else
                {
                    perviousBillingInterval.FromDate = nextBillingInterval.FromDate;
                    perviousBillingInterval.ToDate = nextBillingInterval.ToDate;
                    while (nextBillingInterval.ToDate <= context.IssueDate)
                    {
                        perviousBillingInterval.FromDate = nextBillingInterval.FromDate;
                        perviousBillingInterval.ToDate = nextBillingInterval.ToDate;
                        nextBillingInterval.FromDate = nextBillingInterval.ToDate.AddDays(1);
                        nextBillingInterval.ToDate = nextBillingInterval.FromDate.AddDays(6);
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
            perviousBillingInterval.FromDate = perviousBillingInterval.ToDate.AddDays(-6);
            if (perviousBillingInterval.FromDate.DayOfWeek != DailyType)
            {
                while (perviousBillingInterval.FromDate.DayOfWeek != DailyType)
                {
                    perviousBillingInterval.FromDate = perviousBillingInterval.FromDate.AddDays(-1);
                    perviousBillingInterval.ToDate = perviousBillingInterval.ToDate.AddDays(-1);
                }
            }
            return perviousBillingInterval;
        }
    }
}
