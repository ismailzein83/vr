using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Caching;
using Vanrise.Common.Data;
using Vanrise.Entities;

namespace Vanrise.Common.Business
{
    public class ExtensionConfigurationManager
    {
        #region Private Methods
        public IEnumerable<T> GetExtensionConfigurations<T>(string type) where T : ExtensionConfiguration
        {
            var extensionConfigurations = GetCachedExtensionConfigurations<T>(type);
            if (extensionConfigurations == null)
                throw new NullReferenceException(string.Format("ExtensionConfigurations for {0}",type));
            return extensionConfigurations.Values;
        }

        public T GetExtensionConfigurationByName<T>(string name, string type) where T : ExtensionConfiguration
        {
            var extensionConfigurations = GetCachedExtensionConfigurations<T>(type);
            if (extensionConfigurations == null)
                throw new NullReferenceException(string.Format("ExtensionConfigurations for {0}", type));
            return extensionConfigurations.Values.FirstOrDefault(x=>x.Name==name);
        }
        private Dictionary<Guid, T> GetCachedExtensionConfigurations<T>(string type) where T : ExtensionConfiguration
        {
            return CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject(String.Format("GetCachedExtensionConfigurations_{0}", type), type,
               () =>
               {
                   IExtensionConfigurationDataManager dataManager = CommonDataManagerFactory.GetDataManager<IExtensionConfigurationDataManager>();
                   IEnumerable<T> extensionConfigurations = dataManager.GetExtensionConfigurationsByType<T>(type);
                   return extensionConfigurations.ToDictionary(kvp => kvp.ExtensionConfigurationId, kvp => kvp);
               });
        }
        #endregion


        #region Private Classes

        public class CacheManager : Vanrise.Caching.BaseCacheManager<string>
        {
            IExtensionConfigurationDataManager _dataManager = CommonDataManagerFactory.GetDataManager<IExtensionConfigurationDataManager>();
            ConcurrentDictionary<string, Object> _updateHandlesByRuleType = new ConcurrentDictionary<string, Object>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(string parameter)
            {
                Object updateHandle;
                _updateHandlesByRuleType.TryGetValue(parameter, out updateHandle);
                bool isCacheExpired = _dataManager.AreExtensionConfigurationUpdated(parameter, ref updateHandle);
                _updateHandlesByRuleType.AddOrUpdate(parameter, updateHandle, (key, existingHandle) => updateHandle);
                return isCacheExpired;
            }
        }

        #endregion
    }
}
