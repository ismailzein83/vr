using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.Invoice.Entities;

namespace Vanrise.Invoice.MainExtensions
{
    public class SemiMonthlyBillingPeriod : BillingPeriod
    {
        public override Guid ConfigId { get { return new Guid("E0EDEB7A-FE1D-4207-A1E6-0AE2A42ED452"); } }

        public override List<BillingInterval> GetPeriod(IBillingPeriodContext context)
        {
            List<BillingInterval> billingIntervalList = new List<BillingInterval>();
            BillingInterval perviousBillingInterval = new Entities.BillingInterval();
            if (context.PreviousPeriodEndDate.HasValue)
            {

                BillingInterval nextBillingInterval = new Entities.BillingInterval();
                int fromDateDate = context.PreviousPeriodEndDate.Value.Day > 1 ? 16 : 1;
                int toDateDate = fromDateDate == 1 ? 15 : context.PreviousPeriodEndDate.Value.GetLastDayOfMonth().Day;

                nextBillingInterval.FromDate = new DateTime(context.PreviousPeriodEndDate.Value.Year, context.PreviousPeriodEndDate.Value.Month, fromDateDate);
                nextBillingInterval.ToDate = new DateTime(context.PreviousPeriodEndDate.Value.Year, context.PreviousPeriodEndDate.Value.Month, toDateDate);
                if (nextBillingInterval.ToDate > context.IssueDate)
                {
                    perviousBillingInterval = GetIntervalIfPreviousPeriodNotValid(context.IssueDate);
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
                        nextBillingInterval.ToDate = nextBillingInterval.FromDate.Day == 1 ? nextBillingInterval.FromDate.AddDays(14) : nextBillingInterval.FromDate.GetLastDayOfMonth();
                    }
                }
            }
            else
            {
                perviousBillingInterval = GetIntervalIfPreviousPeriodNotValid(context.IssueDate);
            }
            billingIntervalList.Add(perviousBillingInterval);
            return billingIntervalList;
        }
        private BillingInterval GetIntervalIfPreviousPeriodNotValid(DateTime issueDate)
        {
            BillingInterval perviousBillingInterval = new BillingInterval();
            int toDateDate = issueDate.Day > 15 ? 16 : 1;
            perviousBillingInterval.ToDate = new DateTime(issueDate.Year, issueDate.Month, toDateDate).AddDays(-1);
            int fromDateDate = toDateDate == 16 ? 1 : 16;
            perviousBillingInterval.FromDate = new DateTime( perviousBillingInterval.ToDate.Year,perviousBillingInterval.ToDate.Month,fromDateDate);
            return perviousBillingInterval;
        }
        public override string GetDescription()
        {
            return "Semi-Monthly";
        }
    }
}
