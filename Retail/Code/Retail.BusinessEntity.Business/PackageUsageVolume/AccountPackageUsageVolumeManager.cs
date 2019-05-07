using Retail.BusinessEntity.Data;
using Retail.BusinessEntity.Entities;
using System.Collections.Generic;

namespace Retail.BusinessEntity.Business
{
    public class AccountPackageUsageVolumeBalanceManager
    {
        public Dictionary<PackageUsageVolumeBalanceKey, AccountPackageUsageVolumeBalanceInProcess> GetVolumeBalances(HashSet<PackageUsageVolumeBalanceKey> volumeBalanceKeys)
        {
            IAccountPackageUsageVolumeBalanceDataManager dataManager = BEDataManagerFactory.GetDataManager<IAccountPackageUsageVolumeBalanceDataManager>();
            List<AccountPackageUsageVolumeBalance> volumeBalances = dataManager.GetAccountPackageUsageVolumeBalancesByKeys(volumeBalanceKeys);

            Dictionary<PackageUsageVolumeBalanceKey, AccountPackageUsageVolumeBalanceInProcess> volumeBalancesByKey = null;

            if (volumeBalances != null && volumeBalances.Count > 0)
            {
                volumeBalancesByKey = new Dictionary<PackageUsageVolumeBalanceKey, AccountPackageUsageVolumeBalanceInProcess>();

                foreach (var volumeBalance in volumeBalances)
                {
                    var balanceKey = new PackageUsageVolumeBalanceKey
                    {
                        AccountPackageId = volumeBalance.AccountPackageId,
                        PackageItemId = volumeBalance.PackageItemId,
                        ItemFromTime = volumeBalance.FromTime
                    };
                    volumeBalancesByKey.Add(balanceKey, new AccountPackageUsageVolumeBalanceInProcess { Balance = volumeBalance });
                }
            }

            return volumeBalancesByKey;
        }

        public void UpdateVolumeBalancesInDB(IEnumerable<AccountPackageUsageVolumeBalanceInProcess> volumeBalances)
        {
            IAccountPackageUsageVolumeBalanceDataManager dataManager = BEDataManagerFactory.GetDataManager<IAccountPackageUsageVolumeBalanceDataManager>();

            List<AccountPackageUsageVolumeBalanceToAdd> balancesToAdd = new List<AccountPackageUsageVolumeBalanceToAdd>();
            List<AccountPackageUsageVolumeBalanceToUpdate> balancesToUpdate = new List<AccountPackageUsageVolumeBalanceToUpdate>();

            foreach (var balanceInProcess in volumeBalances)
            {
                var balance = balanceInProcess.Balance;
                if (balanceInProcess.ShouldAdd)
                {
                    balancesToAdd.Add(new AccountPackageUsageVolumeBalanceToAdd
                    {
                        AccountPackageId = balance.AccountPackageId,
                        PackageItemId = balance.PackageItemId,
                        FromTime = balance.FromTime,
                        ToTime = balance.ToTime,
                        ItemVolume = balance.ItemVolume,
                        UsedVolume = balance.UsedVolume
                    });
                }
                else if (balanceInProcess.ShouldUpdate)
                {
                    balancesToUpdate.Add(new AccountPackageUsageVolumeBalanceToUpdate
                    {
                        AccountPackageVolumeBalanceId = balance.AccountPackageUsageVolumeBalanceId,
                        UsedVolume = balance.UsedVolume
                    });
                }
            }

            if (balancesToAdd.Count > 0)
                dataManager.AddAccountPackageUsageVolumeBalance(balancesToAdd);

            if (balancesToUpdate.Count > 0)
                dataManager.UpdateAccountPackageUsageVolumeBalance(balancesToUpdate);
        }
    }

    public class AccountPackageUsageVolumeBalanceInProcess
    {
        public AccountPackageUsageVolumeBalance Balance { get; set; }

        public bool ShouldAdd { get; set; }

        public bool ShouldUpdate { get; set; }
    }
}