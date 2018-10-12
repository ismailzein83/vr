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
        public override void Execute(IRecurringChargePeriodSettingsContext context)
        {
            List<RecurringChargePeriodOutput> periodsList = new List<RecurringChargePeriodOutput>();
            if (context.FromDate.Year == context.ToDate.Year)
            {
                if (context.FromDate.Month == context.ToDate.Month)
                {
                    if (context.FromDate.Day == 1)
                    {
                        var fromDate = new DateTime(context.FromDate.Year, context.FromDate.Month, 1);
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
                    for (var i = context.FromDate.Month; i <= context.ToDate.Month; i++)
                    {
                        if (context.FromDate.Month == i && context.FromDate.Day != 1)
                        {
                            continue;
                        }
                        var fromDate = new DateTime(context.FromDate.Year, i, 1);
                        var toDate = fromDate.AddMonths(1).AddDays(-1);
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
                        if (context.FromDate.Day != 1)
                            currentMonth++;
                        while (currentMonth <= 12)
                        {
                            var fromDate = new DateTime(currentYear, currentMonth, 1);
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
                        while (currentMonth <= context.ToDate.Month)
                        {
                            var fromDate = new DateTime(currentYear, currentMonth, 1);
                            var toDate = fromDate.AddMonths(1).AddDays(-1);
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
                            var fromDate = new DateTime(currentYear, currentMonth, 1);
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
