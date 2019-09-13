using System;
using System.Collections.Generic;
using Retail.BusinessEntity.Entities;
using Vanrise.Common;
using Vanrise.Entities;

namespace Retail.BusinessEntity.MainExtensions.FinancialRecurringChargePeriod
{
    public class QuarterlyRecurringCharge : RecurringChargePeriodSettings
    {
        public override Guid ConfigId { get { return new Guid("A5A8B72B-F834-4DAA-80F8-25157C2C3D37"); } }
        public bool InAdvance { get; set; }

        public override void Execute(IFinancialRecurringChargePeriodSettingsContext context)
        {
            List<RecurringChargePeriodOutput> periodsList = new List<RecurringChargePeriodOutput>();
            DateTime currentDateTime = context.FromDate;
            DateTimeRange dateTimeRange = Utilities.GetQuarter(currentDateTime);

            DateTime fromDate = dateTimeRange.From;
            DateTime toDate = dateTimeRange.To;

            if (toDate > context.ToDate)
            {
                dateTimeRange = Utilities.GetQuarter(currentDateTime.AddMonths(-3));
                fromDate = dateTimeRange.From;
                toDate = dateTimeRange.To;
            }

            bool reCalculateQuarter = false;
            while (toDate >= context.FromDate && toDate <= context.ToDate)
            {
                if (InAdvance)
                {
                    toDate = toDate.AddMonths(3);
                    reCalculateQuarter = true;
                }

                if (reCalculateQuarter)
                {
                    dateTimeRange = Utilities.GetQuarter(toDate);
                    fromDate = dateTimeRange.From;
                    toDate = dateTimeRange.To;
                }

                if (fromDate < context.RecurringChargeBED)
                    fromDate = context.RecurringChargeBED;

                periodsList.Add(new RecurringChargePeriodOutput
                {
                    From = fromDate,
                    To = toDate,
                    RecurringChargeDate = toDate
                });

                if (!InAdvance)
                    toDate = toDate.AddMonths(3);

                reCalculateQuarter = true;
            }

            context.Periods = periodsList;
        }
    }
}