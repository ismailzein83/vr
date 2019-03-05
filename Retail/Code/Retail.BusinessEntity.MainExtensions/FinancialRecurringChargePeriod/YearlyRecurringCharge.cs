using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.MainExtensions.FinancialRecurringChargePeriod
{
    public class YearlyRecurringCharge : RecurringChargePeriodSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("447B67A5-DBEA-4410-9C00-C8D297B8F81C"); }
        }
        public RecurringChargeDayMonth Date { get; set; }

        public override void Execute(IFinancialRecurringChargePeriodSettingsContext context)
        {
            List<RecurringChargePeriodOutput> periodsList = new List<RecurringChargePeriodOutput>();
            DateTime currentDateTime = context.FromDate;
            while (currentDateTime <= context.ToDate)
            {
                var dateTime = new DateTime(currentDateTime.Year, Date.Month, Date.Day);
                if (dateTime.Date == currentDateTime.Date)
                {
                    var fromDate = new DateTime(currentDateTime.Year, Date.Month, Date.Day);
                    var toDate = fromDate.AddYears(1).AddDays(-1);
                    periodsList.Add(new RecurringChargePeriodOutput
                    {
                        From = fromDate,
                        To = toDate,
                        RecurringChargeDate = currentDateTime
                    });
                }
                currentDateTime = currentDateTime.AddDays(1);
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
