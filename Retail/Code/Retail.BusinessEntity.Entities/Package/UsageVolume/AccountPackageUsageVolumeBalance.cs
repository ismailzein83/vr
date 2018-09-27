using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Entities
{
    public class AccountPackageUsageVolumeBalance
    {
        public long AccountPackageUsageVolumeBalanceId { get; set; }

        public long AccountPackageId { get; set; }

        public Guid PackageItemId { get; set; }

        public DateTime FromTime { get; set; }

        public DateTime ToTime { get; set; }

        public decimal ItemVolume { get; set; }

        public decimal UsedVolume { get; set; }
    }

    public class AccountPackageUsageVolumeBalanceToAdd
    {
        public long AccountPackageId { get; set; }

        public Guid PackageItemId { get; set; }

        public DateTime FromTime { get; set; }

        public DateTime ToTime { get; set; }

        public decimal ItemVolume { get; set; }

        public decimal UsedVolume { get; set; }
    }

    public class AccountPackageUsageVolumeBalanceToUpdate
    {
        public long AccountPackageVolumeBalanceId { get; set; }

        public decimal UsedVolume { get; set; }
    }
}
