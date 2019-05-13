using Retail.BusinessEntity.Entities;
using System;
using Vanrise.Common;
using Vanrise.GenericData.Business;

namespace Retail.BusinessEntity.Business
{
    public class AccountPackageProviderManager
    {
        public AccountPackageProvider GetAccountPackageProvider(Guid accountBEDefinitionId)
        {
            BusinessEntityDefinitionManager beDefinitionManager = new BusinessEntityDefinitionManager();

            GetAccountPackageProviderCacheName cacheName = new GetAccountPackageProviderCacheName() { AccountBEDefinitionId = accountBEDefinitionId };
            return beDefinitionManager.GetCachedOrCreate(cacheName, () =>
            {
                var beDefinition = beDefinitionManager.GetBusinessEntityDefinition(accountBEDefinitionId);
                beDefinition.ThrowIfNull("beDefinition", accountBEDefinitionId);
                beDefinition.Settings.ThrowIfNull("beDefinition.Settings", accountBEDefinitionId);

                var additionalSettings = beDefinition.Settings.GetAdditionalSettings(new BEDefinitionSettingsGetAdditionalSettingsContext());
                if (additionalSettings == null)
                    return null;

                var accountPackageProvider = additionalSettings.GetRecord("AccountPackageProvider");
                if (accountPackageProvider == null)
                    return null;

                return accountPackageProvider as AccountPackageProvider;
            });
        }

        private struct GetAccountPackageProviderCacheName
        {
            public Guid AccountBEDefinitionId { get; set; }
        }
    }
}