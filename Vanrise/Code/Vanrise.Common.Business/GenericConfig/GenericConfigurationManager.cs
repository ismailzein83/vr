using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Data;
using Vanrise.Entities;

namespace Vanrise.Common.Business
{
    public abstract class GenericConfigurationManager<T> where T : GenericConfiguration
    {
        public void UpdateConfiguration(T genericConfig)
        {
            string ownerKey = genericConfig.OwnerKey;
            if (ownerKey == null)
                ownerKey = Guid.Empty.ToString();
            int typeId = TypeManager.Instance.GetTypeId(this.GetType());
            IGenericConfigurationDataManager dataManager = CommonDataManagerFactory.GetDataManager<IGenericConfigurationDataManager>();
            dataManager.UpdateConfiguration(ownerKey, typeId, genericConfig);
        }

        public T GetConfiguration(string ownerKey)
        {
            if (ownerKey == null)
                ownerKey = Guid.Empty.ToString();
            int typeId = TypeManager.Instance.GetTypeId(this.GetType());
            string key = String.Concat("{0}_{1}", ownerKey, typeId);
            var genericConfigurations = GetCachedGenericConfigurations();

          return genericConfigurations.GetRecord(key) as T;
        }

        #region Private Classes

        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IGenericConfigurationDataManager _dataManager = CommonDataManagerFactory.GetDataManager<IGenericConfigurationDataManager>();
            object _updateHandle;
            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreGenericConfigurationsUpdated(ref _updateHandle);
            }
        }
        protected Dictionary<string, GenericConfiguration> GetCachedGenericConfigurations()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetCachedGenericConfigurations",
              () =>
              {
                  IGenericConfigurationDataManager dataManager = CommonDataManagerFactory.GetDataManager<IGenericConfigurationDataManager>();
                  return dataManager.GetALllConfigurations();
              });
        }

        #endregion
    }
}
