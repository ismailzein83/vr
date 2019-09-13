﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Common;

namespace TOne.WhS.BusinessEntity.MainExtensions.RecurringCharges
{
    public class MonthlyRecuringCharge : RecurringChargePeriodSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("A1D0F08B-B8F7-4B9C-9C83-49DABD2A68D5"); }
        }
        public bool InAdvance { get; set; }
        public override void Execute(IRecurringChargePeriodSettingsContext context)
        {
            List<RecurringChargePeriodOutput> periodsList = new List<RecurringChargePeriodOutput>();
            DateTime currentDateTime = context.FromDate.GetLastDayOfMonth();
            while (currentDateTime >= context.FromDate && currentDateTime <= context.ToDate)
            {
                if (InAdvance)
                    currentDateTime = currentDateTime.AddMonths(1).GetLastDayOfMonth();

                DateTime fromDate = new DateTime(currentDateTime.Year, currentDateTime.Month, 1);

                if (fromDate < context.RecurringChargeBED)
                    fromDate = context.RecurringChargeBED;

                DateTime toDate = fromDate.GetLastDayOfMonth();

                periodsList.Add(new RecurringChargePeriodOutput
                {
                    From = fromDate,
                    To = toDate,
                    RecurringChargeDate = currentDateTime
                });

                if (!InAdvance)
                    currentDateTime = fromDate.AddMonths(1).GetLastDayOfMonth();
            }
            context.Periods = periodsList;
        }

    }
}
