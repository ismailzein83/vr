using System;
using System.Collections.Generic;
using Vanrise.GenericData.Entities;

namespace Retail.BusinessEntity.Entities
{
    public abstract class AccountPackageProvider : GenericBEAdditionalSettings
    {
        public abstract Dictionary<AccountEventTime, List<RetailAccountPackage>> GetRetailAccountPackages(IAccountPackageProviderGetRetailAccountPackagesContext context);
        public abstract bool DoesUserHaveViewPackageAccess(IPackageDefinitionAccessContext context);
        public abstract bool DoesUserHaveAddPackageAccess(IPackageDefinitionAccessContext context);
        public abstract bool DoesUserHaveEditPackageAccess(IPackageDefinitionAccessContext context);

        public virtual bool TryGetAccount(IAccountPackageProviderTryGetAccountContext context)
        {
            context.Account = null;
            return false;
        }
    }

    public interface IAccountPackageProviderGetRetailAccountPackagesContext
    {
        Guid AccountBEDefinitionId { get; }

        List<AccountEventTime> AccountEventTimeList { get; }
    }

    public class PackageDefinitionAccessContext : IPackageDefinitionAccessContext
    {
        public Guid PackageDefinitionId { get; set; }
        public int UserId { get; set; }
        public Guid AccountBEDefinitionId { get; set; }
    }
    public interface IPackageDefinitionAccessContext
    {
        Guid PackageDefinitionId { get; }
        int UserId { get;}
        Guid AccountBEDefinitionId { get;  }
    }
    public class AccountPackageProviderGetRetailAccountPackagesContext : IAccountPackageProviderGetRetailAccountPackagesContext
    {
        public Guid AccountBEDefinitionId { get; set; }

        public List<AccountEventTime> AccountEventTimeList { get; set; }
    }

    public interface IAccountPackageProviderTryGetAccountContext
    {
        Guid AccountBEDefinitionId { get; }

        long AccountId { get; }

        object Account { set; }
    }

    public class AccountPackageProviderTryGetAccountContext : IAccountPackageProviderTryGetAccountContext
    {
        public Guid AccountBEDefinitionId { get; set; }

        public long AccountId { get; set; }

        public object Account { get; set; }
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