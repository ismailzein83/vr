using System;
using System.Collections.Generic;

namespace Retail.BusinessEntity.Entities
{
    public class BasePackageUsageVolumeCombination
    {
        public Dictionary<int, List<Guid>> PackageItemsByPackageId { get; set; }
    }

    public class PackageUsageVolumeCombination : BasePackageUsageVolumeCombination
    {
        public int PackageUsageVolumeCombinationId { get; set; }
    }

    public class PackageUsageVolumeCombinationToAdd : BasePackageUsageVolumeCombination
    {

    }
}