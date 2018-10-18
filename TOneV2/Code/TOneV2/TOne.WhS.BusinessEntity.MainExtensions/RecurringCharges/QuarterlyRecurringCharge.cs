using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Common;

namespace TOne.WhS.BusinessEntity.MainExtensions.RecurringCharges
{
    public class QuarterlyRecurringCharge : RecurringChargePeriodSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("A5A8B72B-F834-4DAA-80F8-25157C2C3D37"); }
        }
        public bool InAdvance { get; set; }
        public override void Execute(IRecurringChargePeriodSettingsContext context)
        {
            List<RecurringChargePeriodOutput> periodsList = new List<RecurringChargePeriodOutput>();
            DateTime currentDateTime = context.FromDate;
            DateTime fromDate;
            DateTime toDate;
            GetQuarter(currentDateTime, out fromDate, out toDate);
            if(toDate > context.ToDate)
                GetQuarter(currentDateTime.AddMonths(-3), out fromDate, out toDate);

            bool reCalculateQuarter = false;
            while (toDate >= context.FromDate && toDate <= context.ToDate)
            {

                if (InAdvance)
                {
                    toDate = toDate.AddMonths(3);
                    reCalculateQuarter = true;
                }

                if(reCalculateQuarter)
                GetQuarter(toDate, out fromDate, out toDate);

                periodsList.Add(new RecurringChargePeriodOutput
                {
                    From = fromDate,
                    To = toDate
                });

                if (!InAdvance)
                {
                    toDate = toDate.AddMonths(3);
                }
                reCalculateQuarter = true;
            }

            context.Periods = periodsList;
        }
        public void GetQuarter(DateTime date, out DateTime startDate, out DateTime endDate)
        {
            int startingMonth = (date.Month - 1) / 3;
            startingMonth *= 3;
            startingMonth++;
            startDate = new DateTime(date.Year, startingMonth, 1);
            endDate = startDate.AddMonths(2).GetLastDayOfMonth();
        }
    }
}
