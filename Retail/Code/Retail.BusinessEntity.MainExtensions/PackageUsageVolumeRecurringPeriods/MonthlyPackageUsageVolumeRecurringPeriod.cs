using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Retail.BusinessEntity.Entities;

namespace Retail.BusinessEntity.MainExtensions.PackageUsageVolumeRecurringPeriods
{
    public class MonthlyPackageUsageVolumeRecurringPeriod : PackageUsageVolumeRecurringPeriod
    {
        public override Guid ConfigId
        {
            get { return new Guid("2CE2B9F6-F41D-4CFF-A138-12FF1A5A3A2F"); }
        }

        public override void GetEventApplicablePeriod(IPackageUsageVolumeRecurringPeriodGetEventApplicablePeriodContext context)
        {
            DateTime periodStart = new DateTime(context.EventTime.Year, context.EventTime.Month, context.PackageAssignmentStartTime.Day);
            if (periodStart > context.EventTime)
                periodStart = periodStart.AddMonths(-1);
            context.PeriodStart = periodStart;
            context.PeriodEnd = periodStart.AddMonths(1);
        }

        public override void GetChargingDates(IPackageUsageVolumeRecurringPeriodGetChargingDatesContext context)
        {
            DateTime chargingDate = new DateTime(context.FromDate.Year, context.FromDate.Month, context.PackageAssignmentStartTime.Day);
            if (chargingDate < context.FromDate)
                chargingDate = chargingDate.AddMonths(1);

            List<PackageUsageVolumeRecurringPeriodChargingDate> chargingDates = new List<PackageUsageVolumeRecurringPeriodChargingDate>();

            while (chargingDate <= context.ToDate)
            {
                var recurringPeriodChargingDate = new PackageUsageVolumeRecurringPeriodChargingDate
                {
                    ChargingDate = chargingDate
                };
                if (context.PackageAssignmentEndTime.HasValue)
                {
                    DateTime recurringPeriodEndDate = chargingDate.AddMonths(1);
                    if (context.PackageAssignmentEndTime.Value < recurringPeriodEndDate)
                        recurringPeriodChargingDate.PeriodFractionOfTheFullPeriod = (Decimal)((context.PackageAssignmentEndTime.Value - chargingDate).TotalDays / (recurringPeriodEndDate - chargingDate).TotalDays);
                }
                chargingDates.Add(recurringPeriodChargingDate);

                chargingDate = chargingDate.AddMonths(1);
            }
            if (chargingDates.Count > 0)
                context.ChargingDates = chargingDates;
        }
    }
}
