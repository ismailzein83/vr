using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Business
{
    public class AccountPackageUsageVolumeManager
    {
        public Dictionary<PackageUsageVolumeBalanceKey, AccountPackageUsageVolumeBalanceInProcess> GetVolumeBalances(HashSet<PackageUsageVolumeBalanceKey> volumeBalanceKeys)
        {
            List<AccountPackageUsageVolumeBalance> volumeBalances = null;//TODO: get Volume balances from DB based on volumeBalanceKeys

            Dictionary<PackageUsageVolumeBalanceKey, AccountPackageUsageVolumeBalanceInProcess> volumeBalancesByKey = new Dictionary<PackageUsageVolumeBalanceKey, AccountPackageUsageVolumeBalanceInProcess>();
            if (volumeBalances != null)
            {
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
            {
                //TODO: add balances to DB
            }

            if (balancesToUpdate.Count > 0)
            {
                //TODO: update balances in DB
            }
        }
    }

    public struct PackageUsageVolumeBalanceKey
    {
        public long AccountPackageId { get; set; }

        public Guid PackageItemId { get; set; }

        public DateTime ItemFromTime { get; set; }

        public override int GetHashCode()
        {
            return this.PackageItemId.GetHashCode();
        }
    }

    public class AccountPackageUsageVolumeBalanceInProcess
    {
        public AccountPackageUsageVolumeBalance Balance { get; set; }

        public bool ShouldAdd { get; set; }

        public bool ShouldUpdate { get; set; }
    }

}
