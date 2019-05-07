using Retail.BusinessEntity.Entities;
using System.Collections.Generic;

namespace Retail.BusinessEntity.Data
{
    public interface IAccountPackageUsageVolumeBalanceDataManager : IDataManager
    {
        List<AccountPackageUsageVolumeBalance> GetAccountPackageUsageVolumeBalancesByKeys(HashSet<PackageUsageVolumeBalanceKey> volumeBalanceKeys);

        void AddAccountPackageUsageVolumeBalance(List<AccountPackageUsageVolumeBalanceToAdd> accountPackageUsageVolumeBalances);

        void UpdateAccountPackageUsageVolumeBalance(List<AccountPackageUsageVolumeBalanceToUpdate> accountPackageUsageVolumeBalances);
    }
}
