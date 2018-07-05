using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.MainExtensions.RecurringCharges
{
    public class OnceAYearRecuringCharge : RecurringChargePeriodSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("447B67A5-DBEA-4410-9C00-C8D297B8F81C"); }
        }
        public RecurringChargeDayMonth Date { get; set; }

        public override void Execute(IRecurringChargePeriodSettingsContext context)
        {
            List<RecurringChargePeriodOutput> periodsList = new List<RecurringChargePeriodOutput>();

            if (this.Date.Month >= context.FromDate.Month && this.Date.Month <= context.ToDate.Month)
            {
                if (context.FromDate.Month == context.ToDate.Month)
                {
                    if (this.Date.Day >= context.FromDate.Day && this.Date.Day <= context.ToDate.Day)
                    {
                        for (var i = context.FromDate.Year; i <= context.ToDate.Year; i++)
                            periodsList.Add(new RecurringChargePeriodOutput
                            {
                                From = new DateTime(i, this.Date.Month, this.Date.Day),
                                To = new DateTime(i+1, this.Date.Month, this.Date.Day -1),
                            });
                    }
                }
                else
                {
                    for (var i = context.FromDate.Year; i <= context.ToDate.Year; i++)
                        periodsList.Add(new RecurringChargePeriodOutput
                        {
                            From = new DateTime(i, this.Date.Month, this.Date.Day),
                            To = new DateTime(i + 1, this.Date.Month, this.Date.Day - 1),
                        });
                }
            }
            context.Periods = periodsList;
        }
    }
    public class RecurringChargeDayMonth
    {
        public int Month { get; set; }
        public int Day { get; set; }
    }
}
