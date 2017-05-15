using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.AccountBalance.Entities;
using Vanrise.Common.Business;

namespace Retail.BusinessEntity.Business
{
    public class AccountBalanceManager
    {
        private struct GetAccountBalanceTypeIdCacheName
        {
            public Guid AccountBEDefinitionId { get; set; }
        }
        public Guid GetAccountBalanceTypeId(Guid accountBEDefinitionId)
        {
            var cacheName = new GetAccountBalanceTypeIdCacheName { AccountBEDefinitionId = accountBEDefinitionId };
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject(cacheName,
                () =>
                {
                    Vanrise.Common.Business.VRComponentTypeManager _vrComponentTypeManager = new Vanrise.Common.Business.VRComponentTypeManager();
                    IEnumerable<AccountType> accountTypes = _vrComponentTypeManager.GetComponentTypes<AccountTypeSettings, AccountType>();

                    foreach (var accountType in accountTypes)
                    {
                        var extendedSettings = accountType.Settings.ExtendedSettings as SubscriberAccountBalanceSetting;
                        if (extendedSettings != null && extendedSettings.AccountBEDefinitionId == accountBEDefinitionId)
                            return accountType.VRComponentTypeId;
                    }
                    return Guid.Empty;
                });
        }

        public class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            DateTime? _componentTypeCacheLastCheck;

            protected override bool ShouldSetCacheExpired()
            {
                return Vanrise.Caching.CacheManagerFactory.GetCacheManager<Vanrise.Common.Business.VRComponentTypeManager.CacheManager>().IsCacheExpired(ref _componentTypeCacheLastCheck);
            }
        }
    }
}
