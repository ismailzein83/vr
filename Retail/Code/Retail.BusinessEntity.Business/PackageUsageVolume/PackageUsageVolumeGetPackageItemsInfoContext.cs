using System;
using System.Collections.Generic;
using Retail.BusinessEntity.Entities;

namespace Retail.BusinessEntity.Business
{
    public class PackageUsageVolumeGetPackageItemsInfoContext : IPackageUsageVolumeGetPackageItemsInfoContext
    {
        public PackageUsageVolumeGetPackageItemsInfoContext(AccountPackage accountPackage, List<Guid> itemIds, DateTime eventTime, Guid volumePackageDefinitionId)
        {
            this.AccountPackage = accountPackage;
            this.ItemIds = itemIds;
            this.EventTime = eventTime;
            this.VolumePackageDefinitionId = volumePackageDefinitionId;
        }

        public AccountPackage AccountPackage { get; private set; }

        public List<Guid> ItemIds { get; private set; }

        public DateTime EventTime { get; private set; }

        public Guid VolumePackageDefinitionId { get; private set; }

        public DateTime FromTime { get; set; }

        public DateTime ToTime { get; set; }

        public List<PackageUsageVolumeItem> Items { get; set; }
    }
}