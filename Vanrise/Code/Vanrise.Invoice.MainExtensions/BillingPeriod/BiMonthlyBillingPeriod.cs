using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Invoice.Entities;

namespace Vanrise.Invoice.MainExtensions
{
    public class BiMonthlyBillingPeriod : BillingPeriod
    {
        public override Guid ConfigId { get { return new Guid("94AA2673-04E4-4EFC-9913-CD95C40CD600"); } }

        public override List<BillingInterval> GetPeriod(IBillingPeriodContext context)
        {
            List<BillingInterval> billingIntervalList = new List<BillingInterval>();

            BillingInterval perviousBillingInterval = new Entities.BillingInterval();
            if (context.PreviousPeriodEndDate.HasValue)
            {

                BillingInterval nextBillingInterval = new Entities.BillingInterval();
                var date = new DateTime(context.PreviousPeriodEndDate.Value.Year, context.PreviousPeriodEndDate.Value.Month, 1);
                nextBillingInterval.FromDate = date;
                nextBillingInterval.ToDate = date.AddMonths(2).AddDays(-1);
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
                        nextBillingInterval.ToDate = nextBillingInterval.ToDate.AddMonths(2);
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
            var date = new DateTime(issueDate.Year, issueDate.Month, 1);
            perviousBillingInterval.FromDate = date.AddMonths(-2);
            perviousBillingInterval.ToDate = date.AddDays(-1);
            return perviousBillingInterval;
        }

        public override string GetDescription()
        {
            return "Bi-Monthly";
        }
    }
}
