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
                    var fromDate = new DateTime(context.FromDate.Year, context.FromDate.Month, 1);
                    var toDate = fromDate.AddMonths(3).AddDays(-1);
                    periodsList.Add(new RecurringChargePeriodOutput
                    {
                        From = fromDate,
                        To = toDate,
                    });
                }
                else
                {
                    for (var i = context.FromDate.Month; i <= context.ToDate.Month; i+=3)
                    {
                        if (i == context.FromDate.Month && 1 < context.FromDate.Day)
                        {
                            continue;
                        }
                        var fromDate = new DateTime(context.FromDate.Year, i, 1);
                        var toDate = fromDate.AddMonths(3).AddDays(-1);
                        if (i == context.ToDate.Month)
                        {
                            if (toDate.Month == context.ToDate.Month && toDate.Day > context.ToDate.Day)
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
                        if (1 < context.FromDate.Day)
                            currentMonth += 3;

                        while (currentMonth <= 12)
                        {
                            var fromDate = new DateTime(currentYear, currentMonth, 1);
                            var toDate = fromDate.AddMonths(3).AddDays(-1);
                            periodsList.Add(new RecurringChargePeriodOutput
                            {
                                From = fromDate,
                                To = toDate,
                            });
                            currentMonth+=3;
                        }
                    }
                    else if (currentYear == context.ToDate.Year)
                    {
                        int currentMonth = 1;
                        while (currentMonth <= context.ToDate.Month)
                        {
                            var fromDate = new DateTime(currentYear, currentMonth, 1);
                            var toDate = fromDate.AddMonths(3).AddDays(-1);
                            if (currentMonth == context.ToDate.Month)
                            {
                                if (toDate.Month == context.ToDate.Month && toDate.Day > context.ToDate.Day)
                                {
                                    break;
                                }
                            }
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
                        int currentMonth = 1;
                        while (currentMonth <= 12)
                        {
                            var fromDate = new DateTime(currentYear, currentMonth, 1);
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
