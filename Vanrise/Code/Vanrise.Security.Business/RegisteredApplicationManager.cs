using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Caching;
using Vanrise.Security.Data;
using Vanrise.Security.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;

namespace Vanrise.Security.Business
{
    public class RegisteredApplicationManager
    {
        public IEnumerable<RegisteredApplication> GetAllRegisteredApplications()
        {
            var cachedRegisteredApplications = this.GetCachedRegisteredApplications();
            if (cachedRegisteredApplications == null)
                return null;

            return cachedRegisteredApplications.Values;
        }

        public RegisteredApplication GetRegisteredApplicationbyId(Guid applicationId)
        {
            var cachedRegisteredApplications = this.GetCachedRegisteredApplications();
            if (cachedRegisteredApplications == null)
                return null;

            return cachedRegisteredApplications.GetRecord(applicationId);
        }

        public IEnumerable<RegisteredApplicationInfo> GetRegisteredApplicationsInfo(RegisteredApplicationFilter filter)
        {
            IEnumerable<RegisteredApplication> registeredApplications = GetAllRegisteredApplications();

            if (registeredApplications == null)
                return null;

            Func<RegisteredApplication, bool> filterPredicate = (registeredApplication) =>
            {
                if (filter != null && filter.Filters != null)
                {
                    foreach (var registeredApplicationFilter in filter.Filters)
                    {
                        RegisteredApplicationFilterContext context = new RegisteredApplicationFilterContext() { RegisteredApplication = registeredApplication };
                        if (registeredApplicationFilter.IsExcluded(context))
                            return false;
                    }
                }

                return true;
            };

            return registeredApplications.MapRecords(RegisteredApplicationInfoMapper, filterPredicate);
        }

        public IEnumerable<RegisteredApplicationInfo> GetRemoteRegisteredApplicationsInfo(Guid securityProviderId, string serializedFilter)
        {
            SecurityProvider securityProvider = new SecurityProviderManager().GetSecurityProviderbyId(securityProviderId);
            securityProvider.ThrowIfNull("securityProvider", securityProviderId);

            SecurityProviderGetRemoteRegisteredApplicationsInfoContext context = new SecurityProviderGetRemoteRegisteredApplicationsInfoContext() { SerializedFilter = serializedFilter };
            return securityProvider.Settings.ExtendedSettings.GetRemoteRegisteredApplicationsInfo(context);
        }

        public RegisterApplicationOutput RegisterApplication(string applicationName, string applicationURL)
        {
            IRegisteredApplicationDataManager dataManager = SecurityDataManagerFactory.GetDataManager<IRegisteredApplicationDataManager>();
            RegisteredApplication registeredApplication = new RegisteredApplication()
            {
                ApplicationId = Guid.NewGuid(),
                Name = applicationName,
                URL = applicationURL
            };
            bool insertActionSucc = dataManager.AddRegisteredApplication(registeredApplication);
            if (insertActionSucc)
            {
                CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                return new RegisterApplicationOutput() { ApplicationId = registeredApplication.ApplicationId };
            }

            return null;
        }

        public bool HasRemoteApplications(int userId)
        {
            User user = new UserManager().GetUserbyId(userId);
            var result = GetRemoteRegisteredApplicationsInfo(user.SecurityProviderId, null);
            return result != null && result.Count() > 0;
        }

        private RegisteredApplicationInfo RegisteredApplicationInfoMapper(RegisteredApplication registeredApplication)
        {
            return new RegisteredApplicationInfo()
            {
                ApplicationId = registeredApplication.ApplicationId,
                Name = registeredApplication.Name,
                URL = registeredApplication.URL
            };
        }

        private Dictionary<Guid, RegisteredApplication> GetCachedRegisteredApplications()
        {
            return CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetCachedRegisteredApplications",
               () =>
               {
                   IRegisteredApplicationDataManager dataManager = SecurityDataManagerFactory.GetDataManager<IRegisteredApplicationDataManager>();
                   IEnumerable<RegisteredApplication> registeredApplications = dataManager.GetRegisteredApplications();
                   return registeredApplications.ToDictionary(kvp => kvp.ApplicationId, kvp => kvp);
               });
        }

        public class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IRegisteredApplicationDataManager _dataManager = SecurityDataManagerFactory.GetDataManager<IRegisteredApplicationDataManager>();
            object _updateHandle;
            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreRegisteredApplicationsUpdated(ref _updateHandle);
            }
        }
    }
}
