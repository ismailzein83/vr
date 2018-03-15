using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Invoice.Entities;
using Vanrise.Common;
namespace Vanrise.Invoice.MainExtensions
{
    public enum MonthlyType { SpecificDay = 0, LastDay = 1 }
    public class MonthlyBillingPeriod : BillingPeriod
    {
        public override Guid ConfigId { get { return new Guid("37F03848-3F78-4A00-BA56-E6C7E2F5F3A2"); } }
        public List<MonthlyPeriod> MonthlyPeriods { get; set; }
        public override List<BillingInterval> GetPeriod(IBillingPeriodContext context)
        {
            List<BillingInterval> billingIntervalList = new List<BillingInterval>();

            BillingInterval perviousBillingInterval = new Entities.BillingInterval();
            if (context.PreviousPeriodEndDate.HasValue)
            {
             
                DateTime nextFromDate = context.PreviousPeriodEndDate.Value;
                DateTime nextToDate = GetNextPeriodDate(nextFromDate, nextFromDate);
                do
                {
                    perviousBillingInterval.FromDate = nextFromDate;
                    perviousBillingInterval.ToDate = nextToDate;

                    nextFromDate = perviousBillingInterval.ToDate.AddDays(1);
                    nextToDate = GetNextPeriodDate(nextFromDate, nextFromDate);

                } while (nextToDate < context.IssueDate);
                
               if (perviousBillingInterval.ToDate > context.IssueDate)
               {
                    perviousBillingInterval = GetIntervalIfPreviousPeriodNotValid(context.IssueDate);
               }
            }
            else
            {
                perviousBillingInterval = GetIntervalIfPreviousPeriodNotValid(context.IssueDate);
            }
            billingIntervalList.Add(perviousBillingInterval);
            return billingIntervalList;
        }
        private DateTime GetNextPeriodDate(DateTime fromDate, DateTime month)
        {
            foreach(var item in this.MonthlyPeriods)
            {
                var date = item.MonthlyType == MonthlyType.SpecificDay ?
                  new DateTime(month.Year, month.Month, item.SpecificDay.Value) : month.GetLastDayOfMonth();
                if (date > fromDate)
                    return date.AddDays(-1);
            }
            return GetNextPeriodDate(fromDate, month.AddMonths(1));
        }
        private DateTime GetPreviousPeriodDate(DateTime fromDate, DateTime month, bool isFromtheIssueDate)
        {
            DateTime? maxDate = null;
            foreach (var item in this.MonthlyPeriods)
            {
                var date = item.MonthlyType == MonthlyType.SpecificDay ? 
                    new DateTime(month.Year, month.Month, item.SpecificDay.Value) : month.GetLastDayOfMonth();
                if ((date < fromDate || (isFromtheIssueDate && date == fromDate)) && (!maxDate.HasValue || date > maxDate.Value))
                    maxDate = date;
            }
            if (maxDate.HasValue)
                return maxDate.Value;
           return GetPreviousPeriodDate(fromDate, month.AddMonths(-1), false);
        }
        private BillingInterval GetIntervalIfPreviousPeriodNotValid(DateTime issueDate)
        {
            BillingInterval perviousBillingInterval = new BillingInterval();
            var toDate = GetPreviousPeriodDate(issueDate, issueDate, true);
            perviousBillingInterval.ToDate = toDate.AddDays(-1);
            perviousBillingInterval.FromDate = GetPreviousPeriodDate(toDate, toDate, false);
            return perviousBillingInterval;
        }
        public override string GetDescription()
        {
            return "Monthly";
        }
    }
    public class MonthlyPeriod
    {
        public MonthlyType MonthlyType { get; set; }
        public int? SpecificDay { get; set; }

    }
}
