using System;
using Vanrise.Entities;

namespace Retail.BusinessEntity.Entities
{
    public class AccountPackageRecurChargeData
    {
        public AccountPackage AccountPackage { get; set; }

        public DateTime BeginChargePeriod { get; set; }

        public DateTime EndChargePeriod { get; set; }
    }

    public class AccountPackageRecurChargePeriod
    {
        public long AccountPackageId { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }
    }
}