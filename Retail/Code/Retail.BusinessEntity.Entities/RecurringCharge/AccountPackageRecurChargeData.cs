using System;

namespace Retail.BusinessEntity.Entities
{
    public class AccountPackageRecurChargeData
    {
        public AccountPackage AccountPackage { get; set; }

        public DateTime BeginChargePeriod { get; set; }

        public DateTime EndChargePeriod { get; set; }
    }
}