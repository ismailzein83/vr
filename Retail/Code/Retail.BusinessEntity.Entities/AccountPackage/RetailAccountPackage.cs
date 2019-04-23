﻿using System;

namespace Retail.BusinessEntity.Entities
{
    public class RetailAccountPackage
    {
        public long AccountPackageId { get; set; }

        public long AccountId { get; set; }

        public int PackageId { get; set; }

        public Guid AccountBEDefinitionId { get; set; }

        public DateTime BED { get; set; }

        public DateTime? EED { get; set; }
    }
}