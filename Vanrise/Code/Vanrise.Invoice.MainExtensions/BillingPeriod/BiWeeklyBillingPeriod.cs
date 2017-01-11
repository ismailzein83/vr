using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Invoice.Entities;

namespace Vanrise.Invoice.MainExtensions
{
    public class BiWeeklyBillingPeriod : BillingPeriod
    {
        public override Guid ConfigId { get { return new Guid("66B9FD8C-DACB-4F35-9853-F6C0C9DFE4F1"); } }
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
                nextBillingInterval.ToDate = nextBillingInterval.FromDate.AddDays(13);
                perviousBillingInterval.FromDate = nextBillingInterval.FromDate;
                perviousBillingInterval.ToDate = nextBillingInterval.ToDate;
                while (nextBillingInterval.ToDate <= context.IssueDate)
                {
                    perviousBillingInterval.FromDate = nextBillingInterval.FromDate;
                    perviousBillingInterval.ToDate = nextBillingInterval.ToDate;
                    nextBillingInterval.FromDate = nextBillingInterval.ToDate.AddDays(1);
                    nextBillingInterval.ToDate = nextBillingInterval.FromDate.AddDays(13);
                }
            }
            else
            {
                perviousBillingInterval.ToDate = context.IssueDate.AddDays(-1);
                perviousBillingInterval.FromDate = perviousBillingInterval.ToDate.AddDays(-13);
                if (perviousBillingInterval.FromDate.DayOfWeek != DailyType)
                {
                    while (perviousBillingInterval.FromDate.DayOfWeek != DailyType)
                    {
                        perviousBillingInterval.FromDate = perviousBillingInterval.FromDate.AddDays(-1);
                        perviousBillingInterval.ToDate = perviousBillingInterval.ToDate.AddDays(-1);
                    }
                }
            }
            return perviousBillingInterval;
        }
    }
}
