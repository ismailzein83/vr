using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.MainExtensions.RecurringCharges
{
    public class MonthlyRecuringCharge : RecurringChargePeriodSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("A1D0F08B-B8F7-4B9C-9C83-49DABD2A68D5"); }
        }
        public int Day { get; set; }
        public override void Execute(IRecurringChargePeriodSettingsContext context)
        {
            List<RecurringChargePeriodOutput> periodsList = new List<RecurringChargePeriodOutput>();
            if (context.FromDate.Year == context.ToDate.Year)
            {
                if (context.FromDate.Month == context.ToDate.Month)
                {
                    if (this.Day >= context.FromDate.Day && this.Day <= context.ToDate.Day)
                    {
                        int daysInMonth = DateTime.DaysInMonth(context.FromDate.Year, context.FromDate.Month);
                        int day = this.Day;
                        if (day > daysInMonth)
                            day = daysInMonth;
                        var fromDate = new DateTime(context.FromDate.Year, context.FromDate.Month, day);
                        var toDate = fromDate.AddMonths(1).AddDays(-1);
                        periodsList.Add(new RecurringChargePeriodOutput
                        {
                            From = fromDate,
                            To = toDate,
                        });
                    }
                }
                else
                {
                    for (var i = context.FromDate.Month; i < context.ToDate.Month; i++)
                    {
                        if(i==context.FromDate.Month && this.Day < context.FromDate.Day)
                        {
                            continue;
                        }
                        int daysInMonth = DateTime.DaysInMonth(context.FromDate.Year, i);
                        int day = this.Day;
                        if (day > daysInMonth)
                            day = daysInMonth;
                        var fromDate = new DateTime(context.FromDate.Year, i, day);
                        var toDate = fromDate.AddMonths(1).AddDays(-1);
                        if (i == context.ToDate.Month - 1)
                        {
                            if (toDate.Month==context.ToDate.Month && toDate.Day > context.ToDate.Day)
                            {
                                break;
                            }
                        }
                        periodsList.Add(new RecurringChargePeriodOutput
                        {
                            From = fromDate,
                            To = toDate,
                        });
                    }
                }
            }
            else
            {
                int currentYear = context.FromDate.Year;
                while (currentYear <= context.ToDate.Year)
                {
                    if (currentYear == context.FromDate.Year)
                    {
                        int currentMonth = context.FromDate.Month;
                        if (this.Day < context.FromDate.Day)
                            currentMonth++;
                        while (currentMonth <= 12)
                        {
                            int daysInMonth = DateTime.DaysInMonth(currentYear, currentMonth);
                            int day = this.Day;
                            if (day > daysInMonth)
                                day = daysInMonth;
                            var fromDate = new DateTime(currentYear, currentMonth, day);
                            var toDate = fromDate.AddMonths(1).AddDays(-1);
                            periodsList.Add(new RecurringChargePeriodOutput
                            {
                                From = fromDate,
                                To = toDate,
                            });
                            currentMonth++;
                        }
                    }
                    else if (currentYear == context.ToDate.Year)
                    {
                        int currentMonth = 1;
                        int lastMonth = context.ToDate.Month;
                        if (this.Day > context.ToDate.Day)
                            lastMonth--;
                        while (currentMonth < lastMonth)
                        {
                            int daysInMonth = DateTime.DaysInMonth(currentYear, currentMonth);
                            int day = this.Day;
                            if (day > daysInMonth)
                                day = daysInMonth;

                            var fromDate = new DateTime(currentYear, currentMonth, day);
                            var toDate = fromDate.AddMonths(1).AddDays(-1);
                            if (currentMonth == lastMonth - 1)
                            {
                                if (toDate.Month==context.ToDate.Month && toDate.Day > context.ToDate.Day)
                                {
                                    break;
                                }
                            }
                            periodsList.Add(new RecurringChargePeriodOutput
                            {
                                From = fromDate,
                                To = toDate,
                            });
                            currentMonth++;
                        }
                    }
                    else
                    {
                        int currentMonth = 1;
                        while (currentMonth <= 12)
                        {
                            int daysInMonth = DateTime.DaysInMonth(currentYear, currentMonth);
                            int day = this.Day;
                            if (day > daysInMonth)
                                day = daysInMonth;

                            var fromDate = new DateTime(currentYear, currentMonth, day);
                            var toDate = fromDate.AddMonths(1).AddDays(-1);
                            periodsList.Add(new RecurringChargePeriodOutput
                            {
                                From = fromDate,
                                To = toDate,
                            });
                            currentMonth++;
                        }
                    }
                    currentYear++;
                }
            }
            context.Periods = periodsList;
        }
    }
}
