using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.MainExtensions.RecurringCharges
{
    public class QuarterlyRecurringCharge : RecurringChargePeriodSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("A5A8B72B-F834-4DAA-80F8-25157C2C3D37"); }
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
                        var toDate = fromDate.AddMonths(3).AddDays(-1);
                        periodsList.Add(new RecurringChargePeriodOutput
                        {
                            From = fromDate,
                            To = toDate,
                        });
                    }
                }
                else
                {
                    for (var i = context.FromDate.Month; i <= context.ToDate.Month; i += 3)
                    {
                        if (context.FromDate.Month == i && context.FromDate.Day != 1)
                        {
                            continue;
                        }
                        var fromDate = new DateTime(context.FromDate.Year, i, 1);
                        if (fromDate > context.ToDate)
                        {
                            break;
                        }
                        var toDate = fromDate.AddMonths(3).AddDays(-1);
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
                            currentMonth += 3;
                        while (currentMonth <= 12)
                        {
                            var fromDate = new DateTime(currentYear, currentMonth, 1);
                            if (fromDate > context.ToDate)
                            {
                                break;
                            }
                            var toDate = fromDate.AddMonths(3).AddDays(-1);
                            periodsList.Add(new RecurringChargePeriodOutput
                            {
                                From = fromDate,
                                To = toDate,
                            });
                            currentMonth += 3;
                        }
                    }
                    else if (currentYear == context.ToDate.Year)
                    {
                        int currentMonth = periodsList != null && periodsList.Count > 0 ? periodsList.Last().To.Month + 1 : context.FromDate.Month + 3;
                        if (currentMonth > 12)
                            currentMonth -= 12;
                        while (currentMonth <= context.ToDate.Month)
                        {
                            var fromDate = new DateTime(currentYear, currentMonth, 1);
                            if (fromDate > context.ToDate)
                            {
                                break;
                            }
                            var toDate = fromDate.AddMonths(3).AddDays(-1);
                            periodsList.Add(new RecurringChargePeriodOutput
                            {
                                From = fromDate,
                                To = toDate,
                            });
                            currentMonth += 3;
                        }
                    }
                    else
                    {
                        int currentMonth = periodsList != null && periodsList.Count > 0 ? periodsList.Last().To.Month + 1 : context.FromDate.Month + 3;
                        if (currentMonth > 12)
                            currentMonth -= 12;
                        while (currentMonth <= 12)
                        {
                            var fromDate = new DateTime(currentYear, currentMonth, 1);
                            if (fromDate > context.ToDate)
                            {
                                break;
                            }
                            var toDate = fromDate.AddMonths(3).AddDays(-1);
                            periodsList.Add(new RecurringChargePeriodOutput
                            {
                                From = fromDate,
                                To = toDate,
                            });
                            currentMonth += 3;
                        }
                    }
                    currentYear++;
                }
            }
            context.Periods = periodsList;
        }
    }
}
