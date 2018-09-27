using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Entities
{
    public abstract class PackageUsageVolumeRecurringPeriod
    {
        public abstract Guid ConfigId { get; }

        public abstract void GetEventApplicablePeriod(IPackageUsageVolumeRecurringPeriodGetEventApplicablePeriodContext context);

        public abstract void GetChargingDates(IPackageUsageVolumeRecurringPeriodGetChargingDatesContext context);
    }

    public interface IPackageUsageVolumeRecurringPeriodGetEventApplicablePeriodContext
    {
        DateTime EventTime { get; }

        DateTime PackageAssignmentStartTime { get; }

        DateTime? PackageAssignmentEndTime { get; }

        DateTime PeriodStart { set; }

        DateTime PeriodEnd { set; }
    }

    public interface IPackageUsageVolumeRecurringPeriodGetChargingDatesContext
    {
        DateTime FromDate { get; }

        DateTime ToDate { get; }

        DateTime PackageAssignmentStartTime { get; }

        DateTime? PackageAssignmentEndTime { get; }

        List<PackageUsageVolumeRecurringPeriodChargingDate> ChargingDates { set; }
    }

    public class PackageUsageVolumeRecurringPeriodChargingDate
    {
        public DateTime ChargingDate { get; set; }

        public Decimal? PeriodFractionOfTheFullPeriod { get; set; }
    }
}
