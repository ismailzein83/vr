using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Caching;
using Vanrise.Common;
using Vanrise.GenericData.Data;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.Business
{
    public class GenericRuleDefinitionManager
    {
        #region Public Methods

        public Vanrise.Entities.IDataRetrievalResult<GenericRuleDefinition> GetFilteredGenericRuleDefinitions(Vanrise.Entities.DataRetrievalInput<GenericRuleDefinitionQuery> input)
        {
            var cachedGenericRuleDefinitions = GetCachedGenericRuleDefinitions();
            Func<GenericRuleDefinition, bool> filterExpression = (genericRuleDefinition) => (input.Query.Name == null || genericRuleDefinition.Name.ToUpper().Contains(input.Query.Name.ToUpper()));
            return cachedGenericRuleDefinitions.ToBigResult(input, filterExpression);
        }
        
        #endregion

        #region Private Methods

        private Dictionary<int, GenericRuleDefinition> GetCachedGenericRuleDefinitions()
        {
            return CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetGenericRuleDefinitions", () => {
                IGenericRuleDefinitionDataManager dataManager = GenericDataDataManagerFactory.GetDataManager<IGenericRuleDefinitionDataManager>();
                IEnumerable<GenericRuleDefinition> genericRuleDefinitions = dataManager.GetGenericRuleDefinitions();
                return genericRuleDefinitions.ToDictionary(genericRuleDefinition => genericRuleDefinition.GenericRuleDefinitionId, genericRuleDefinition => genericRuleDefinition);
            });
        }

        #endregion

        #region Private Classes

        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IGenericRuleDefinitionDataManager _dataManager = GenericDataDataManagerFactory.GetDataManager<IGenericRuleDefinitionDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreGenericRuleDefinitionsUpdated(ref _updateHandle);
            }
        }

        #endregion
    }
}
