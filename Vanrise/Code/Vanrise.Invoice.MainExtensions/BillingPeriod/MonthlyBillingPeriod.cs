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
        public override BillingInterval GetPeriod(IBillingPeriodContext context)
        {

            BillingInterval perviousBillingInterval = new Entities.BillingInterval();
            IEnumerable<MonthlyPeriod> ascMonthlyPeriods = this.MonthlyPeriods.OrderBy(x => !x.SpecificDay.HasValue).ThenBy(x => x.SpecificDay.HasValue);
            if (context.PreviousPeriodEndDate.HasValue)
            {
             
                BillingInterval nextBillingInterval = new Entities.BillingInterval();
                nextBillingInterval.FromDate = context.PreviousPeriodEndDate.Value;
                MonthlyPeriod firstMonthlyPeriod = ascMonthlyPeriods.ElementAt(0);
                DateTime firstDate;
                if (firstMonthlyPeriod.MonthlyType == MonthlyType.SpecificDay)
                    firstDate = new DateTime(nextBillingInterval.FromDate.Year, nextBillingInterval.FromDate.Month, firstMonthlyPeriod.SpecificDay.Value);
                else
                {
                    firstDate = nextBillingInterval.FromDate.GetLastDayOfMonth();
                }
                nextBillingInterval.ToDate = GetNextPeriodDate(nextBillingInterval.FromDate, ascMonthlyPeriods, firstDate);
               
                perviousBillingInterval.FromDate = nextBillingInterval.FromDate;
                perviousBillingInterval.ToDate = nextBillingInterval.ToDate;
                while (nextBillingInterval.ToDate < context.IssueDate)
                {
                    perviousBillingInterval.FromDate = nextBillingInterval.FromDate;
                    perviousBillingInterval.ToDate = nextBillingInterval.ToDate;
                    nextBillingInterval.FromDate = nextBillingInterval.ToDate.AddDays(1);
                    nextBillingInterval.ToDate = GetNextPeriodDate(nextBillingInterval.FromDate, ascMonthlyPeriods, firstDate);
                }
            }
            else
            {
                IEnumerable<MonthlyPeriod> descMonthlyPeriods = this.MonthlyPeriods.OrderByDescending(x => x.SpecificDay);
                perviousBillingInterval.ToDate = GetPreviousPeriodDate(context.IssueDate, descMonthlyPeriods).AddDays(-1);
                perviousBillingInterval.FromDate = GetPreviousPeriodDate(perviousBillingInterval.ToDate , descMonthlyPeriods);
            }
            return perviousBillingInterval;
        }
        public DateTime GetNextPeriodDate(DateTime fromDate, IEnumerable<MonthlyPeriod> monthlyPeriods,DateTime firstDate)
        {
            foreach(var item in monthlyPeriods)
            {
                if (item.MonthlyType == MonthlyType.SpecificDay)
                {
                    var date = new DateTime(firstDate.Year, firstDate.Month, item.SpecificDay.Value);
                    if (date > fromDate)
                        return date.AddDays(-1);
                }else if(item.MonthlyType == MonthlyType.LastDay)
                {
                    var date = new DateTime(firstDate.Year, firstDate.Month, fromDate.GetLastDayOfMonth().Day);
                    if (date > fromDate)
                    return fromDate.GetLastDayOfMonth().AddDays(-1);
                }
            }
            return GetNextPeriodDate(fromDate, monthlyPeriods, firstDate.AddMonths(1));
        }
        public DateTime GetPreviousPeriodDate(DateTime fromDate, IEnumerable<MonthlyPeriod> monthlyPeriods)
        {
            foreach (var item in monthlyPeriods)
            {
                if (item.MonthlyType == MonthlyType.SpecificDay)
                {
                    var date = new DateTime(fromDate.Year, fromDate.Month, item.SpecificDay.Value);
                    if (date < fromDate)
                        return date;
                }
                else if (item.MonthlyType == MonthlyType.LastDay)
                {
                  return fromDate.AddMonths(-1).GetLastDayOfMonth();
                }
            }
            return GetPreviousPeriodDate(fromDate.AddMonths(-1), monthlyPeriods);
        }
       
    }
    public class MonthlyPeriod
    {
        public MonthlyType MonthlyType { get; set; }
        public int? SpecificDay { get; set; }

    }
}
