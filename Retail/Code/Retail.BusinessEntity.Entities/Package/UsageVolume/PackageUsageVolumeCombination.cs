﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Entities
{
    public class PackageUsageVolumeCombination
    {
        public int PackageUsageVolumeCombinationId { get; set; }

        public Dictionary<int, List<Guid>> PackageItemsByPackageId { get; set; }
    }
}
