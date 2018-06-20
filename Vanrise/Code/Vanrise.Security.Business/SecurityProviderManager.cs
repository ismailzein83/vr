using System;
using System.Linq;
using System.Collections.Generic;
using Vanrise.Caching;
using Vanrise.Common;
using Vanrise.Security.Data;
using Vanrise.Security.Entities;
using Vanrise.Common.Business;
using Vanrise.GenericData.Entities;

namespace Vanrise.Security.Business
{
    public class SecurityProviderManager
    {
        static Guid beDefinitionId = new Guid("00166bed-92c1-4a5d-8280-a048c8a0eb95");

        public SecurityProviderInfo GetSecurityProviderInfobyId(Guid securityProviderId)
        {
            var cachedSecurityProviders = this.GetCachedSecurityProviders();
            if (cachedSecurityProviders == null)
                return null;

            SecurityProvider securityProvider = cachedSecurityProviders.GetRecord(securityProviderId);
            if (securityProvider == null)
                return null;

            return SecurityProviderInfoMapper(securityProvider);
        }

        public SecurityProvider GetSecurityProviderbyId(Guid securityProviderId)
        {
            var cachedSecurityProviders = this.GetCachedSecurityProviders();
            if (cachedSecurityProviders == null)
                return null;

            SecurityProvider securityProvider = cachedSecurityProviders.GetRecord(securityProviderId);
            if (securityProvider == null)
                return null;

            securityProvider.Settings.ThrowIfNull("securityProvider.Settings", securityProviderId);
            securityProvider.Settings.ExtendedSettings.ThrowIfNull("securityProvider.Settings.ExtendedSettings", securityProviderId);

            return securityProvider;
        }

        public SecurityProvider GetDefaultSecurityProvider()
        {
            Guid defaultSecurityProviderId = new ConfigManager().GetDefaultSecurityProviderId();
            return GetSecurityProviderbyId(defaultSecurityProviderId);
        }

        public IEnumerable<SecurityProvider> GetSecurityProviders()
        {
            var cachedSecurityProviders = this.GetCachedSecurityProviders();
            if (cachedSecurityProviders == null)
                return null;

            return cachedSecurityProviders.Values;
        }

        public IEnumerable<SecurityProviderInfo> GetSecurityProvidersInfo(SecurityProviderFilter filter)
        {
            IEnumerable<SecurityProvider> securityProviders = GetSecurityProviders();

            if (securityProviders == null)
                return null;

            Func<SecurityProvider, bool> filterPredicate = (securityProvider) =>
            {
                if (filter != null && filter.Filters != null)
                {
                    foreach (var securityProviderFilter in filter.Filters)
                    {
                        SecurityProviderFilterContext context = new SecurityProviderFilterContext() { SecurityProvider = securityProvider };
                        if (securityProviderFilter.IsExcluded(context))
                            return false;
                    }
                }

                return true;
            };

            return securityProviders.MapRecords(SecurityProviderInfoMapper, filterPredicate);
        }

        public IEnumerable<SecurityProviderInfo> GetRemoteSecurityProvidersInfo(Guid connectionId, string serializedFilter)
        {
            VRConnectionManager connectionManager = new VRConnectionManager();
            var vrConnection = connectionManager.GetVRConnection<VRInterAppRestConnection>(connectionId);
            VRInterAppRestConnection connectionSettings = vrConnection.Settings as VRInterAppRestConnection;
            return connectionSettings.Get<IEnumerable<SecurityProviderInfo>>(string.Format("/api/VR_Sec/SecurityProvider/GetSecurityProvidersInfo?filter={0}", serializedFilter));
        }

        public IEnumerable<SecurityProviderConfigs> GetSecurityProviderConfigs()
        {
            var extensionConfigurationManager = new ExtensionConfigurationManager();
            return extensionConfigurationManager.GetExtensionConfigurations<SecurityProviderConfigs>(SecurityProviderConfigs.EXTENSION_TYPE);
        }

        private SecurityProviderInfo SecurityProviderInfoMapper(SecurityProvider securityProvider)
        {
            securityProvider.ThrowIfNull("securityProvider");
            securityProvider.Settings.ThrowIfNull("securityProvider.Settings", securityProvider.SecurityProviderId);
            securityProvider.Settings.ExtendedSettings.ThrowIfNull("securityProvider.Settings.ExtendedSettings", securityProvider.SecurityProviderId);

            return new SecurityProviderInfo()
            {
                Name = securityProvider.Name,
                SecurityProviderId = securityProvider.SecurityProviderId,
                AuthenticateUserEditor = securityProvider.Settings.ExtendedSettings.AuthenticateUserEditor,
                FindUserEditor = securityProvider.Settings.ExtendedSettings.FindUserEditor,
                SupportPasswordManagement = securityProvider.Settings.ExtendedSettings.SupportPasswordManagement
            };
        }

        private Dictionary<Guid, SecurityProvider> GetCachedSecurityProviders()
        {
            IGenericBusinessEntityManager genericBusinessEntityManager = Vanrise.GenericData.Entities.BusinessManagerFactory.GetManager<IGenericBusinessEntityManager>();
            return genericBusinessEntityManager.GetCachedOrCreate("GetCachedSecurityProviders", beDefinitionId, () =>
            {
                List<GenericBusinessEntity> genericBusinessEntities = genericBusinessEntityManager.GetAllGenericBusinessEntities(beDefinitionId);
                Dictionary<Guid, SecurityProvider> result = new Dictionary<Guid, SecurityProvider>();

                if (genericBusinessEntities != null)
                {
                    foreach (GenericBusinessEntity genericBusinessEntity in genericBusinessEntities)
                    {
                        if (genericBusinessEntity.FieldValues == null)
                            continue;

                        SecurityProvider securityProvider = new SecurityProvider()
                        {
                            SecurityProviderId = (Guid)genericBusinessEntity.FieldValues.GetRecord("SecurityProviderId"),
                            Name = genericBusinessEntity.FieldValues.GetRecord("Name") as string,
                            Settings = genericBusinessEntity.FieldValues.GetRecord("Settings") as SecurityProviderSettings
                        };
                        result.Add(securityProvider.SecurityProviderId, securityProvider);
                    }
                }

                return result;
            });
        }
    }
}