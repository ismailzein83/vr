using System;
using System.Collections.Generic;
using Vanrise.GenericData.Entities;

namespace Retail.BusinessEntity.Entities
{
    public abstract class AccountPackageProvider : GenericBEAdditionalSettings
    {
        public abstract Dictionary<AccountEventTime, List<RetailAccountPackage>> GetRetailAccountPackages(IAccountPackageProviderGetRetailAccountPackagesContext context);
    }

    public interface IAccountPackageProviderGetRetailAccountPackagesContext
    {
        Guid AccountBEDefinitionId { get; }

        List<AccountEventTime> AccountEventTimeList { get; }
    }

    public class AccountPackageProviderGetRetailAccountPackagesContext : IAccountPackageProviderGetRetailAccountPackagesContext
    {
        public Guid AccountBEDefinitionId { get; set; }

        public List<AccountEventTime> AccountEventTimeList { get; set; }
    }

    public interface IAccountPackageProviderGetRetailAccountPackageContext
    {
        long AccountId { get; }

        long PackageId { get; }

        DateTime EventTime { get; }
    }

    public class AccountPackageProviderGetRetailAccountPackageContext : IAccountPackageProviderGetRetailAccountPackageContext
    {
        public long AccountId { get; set; }

        public long PackageId { get; set; }

        public DateTime EventTime { get; set; }
    }

    public struct AccountEventTime
    {
        public long AccountId { get; set; }

        public DateTime EventTime { get; set; }

        public override int GetHashCode()
        {
            return this.AccountId.GetHashCode() + this.EventTime.GetHashCode();
        }
    }
}