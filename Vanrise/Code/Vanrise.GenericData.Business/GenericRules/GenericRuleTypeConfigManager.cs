using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;
using Vanrise.Common;
using Vanrise.Caching;
using Vanrise.GenericData.Data;
namespace Vanrise.GenericData.Business
{
    public class GenericRuleTypeConfigManager
    {
        #region Public Methods
        public IEnumerable<GenericRuleTypeConfig> GetGenericRuleTypes()
        {
            var cachedGenericRuleTypes = GetCachedGenericRuleTypes();
            return cachedGenericRuleTypes.Values;
        }

        public GenericRuleTypeConfig GetGenericRuleTypeByName(string ruleTypeName)
        {
            var cachedGenericRuleTypes = GetCachedGenericRuleTypes();
            return cachedGenericRuleTypes.FindRecord(x=>x.Name == ruleTypeName);
        }

        public int GetGenericRuleTypeIdByName(string ruleTypeName)
        {
            var ruleTypeConfig = GetGenericRuleTypeByName(ruleTypeName);
            if (ruleTypeConfig == null)
                throw new NullReferenceException(String.Format("ruleTypeConfig. ruleTypeName '{0}'", ruleTypeName));
            return ruleTypeConfig.GenericRuleTypeConfigId;
        }

        public GenericRuleTypeConfig GetGenericRuleTypeById(int ruleTypeConfigId)
        {
            var cachedGenericRuleTypes = GetCachedGenericRuleTypes();
            return cachedGenericRuleTypes.GetRecord(ruleTypeConfigId);
        }

        #endregion

        #region Private Methods
        private Dictionary<int, GenericRuleTypeConfig> GetCachedGenericRuleTypes()
        {
            return CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetGenericRuleTypes",
               () =>
               {
                   IGenericRuleTypeConfigDataManager dataManager = GenericDataDataManagerFactory.GetDataManager<IGenericRuleTypeConfigDataManager>();
                   IEnumerable<GenericRuleTypeConfig> denericRuleTypes = dataManager.GetGenericRuleTypes();
                   return denericRuleTypes.ToDictionary(kvp => kvp.GenericRuleTypeConfigId, kvp => kvp);
               });
        }

        #endregion

        #region Private Classes
        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IGenericRuleTypeConfigDataManager _dataManager = GenericDataDataManagerFactory.GetDataManager<IGenericRuleTypeConfigDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreGenericRuleTypeConfigUpdated(ref _updateHandle);
            }
        }
        #endregion
    }
}
