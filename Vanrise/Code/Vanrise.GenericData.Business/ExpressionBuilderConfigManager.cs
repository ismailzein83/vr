using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities.ExpressionBuilder;
using Vanrise.Common;
using Vanrise.Caching;
using Vanrise.GenericData.Data;
namespace Vanrise.GenericData.Business
{
    public class ExpressionBuilderConfigManager
    {  
        
        #region Public Methods
        public IEnumerable<ExpressionBuilderConfig> GetExpressionBuilderTemplates()
        {
            var cachedExpressionBuilderTemplates = GetCachedExpressionBuilderTemplates();
            return cachedExpressionBuilderTemplates.Values;
        }

        public ExpressionBuilderConfig GetExpressionBuilderTemplate(int configId)
        {
            var cachedExpressionBuilderTemplates = GetCachedExpressionBuilderTemplates();
            return cachedExpressionBuilderTemplates.GetRecord(configId);
        }

        #endregion

        #region Private Methods
        private Dictionary<int, ExpressionBuilderConfig> GetCachedExpressionBuilderTemplates()
        {
            return CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetExpressionBuilderTemplates",
               () =>
               {
                   IExpressionBuilderConfigDataManager dataManager = GenericDataDataManagerFactory.GetDataManager<IExpressionBuilderConfigDataManager>();
                   IEnumerable<ExpressionBuilderConfig> expressionBuilderTemplates = dataManager.GetExpressionBuilderTemplates();
                   return expressionBuilderTemplates.ToDictionary(kvp => kvp.ExpressionBuilderConfigId, kvp => kvp);
               });
        }

        #endregion

        #region Private Classes
        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IExpressionBuilderConfigDataManager _dataManager = GenericDataDataManagerFactory.GetDataManager<IExpressionBuilderConfigDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreExpressionBuilderConfigUpdated(ref _updateHandle);
            }
        }
        #endregion
    }
}
