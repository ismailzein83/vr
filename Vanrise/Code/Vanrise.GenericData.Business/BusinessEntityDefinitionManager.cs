using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Caching;
using Vanrise.GenericData.Data;
using Vanrise.GenericData.Entities;
using Vanrise.Common;

namespace Vanrise.GenericData.Business
{
    public class BusinessEntityDefinitionManager
    {
        #region Public Methods

        public Vanrise.Entities.IDataRetrievalResult<BusinessEntityDefinitionDetail> GetFilteredBusinessEntityDefinitions(Vanrise.Entities.DataRetrievalInput<BusinessEntityDefinitionQuery> input)
        {
            var cachedBEDefinitions = GetCachedBusinessEntityDefinitions();

            Func<BusinessEntityDefinition, bool> filterExpression = (dataRecordStorage) =>
                (input.Query.Name == null || dataRecordStorage.Name.ToLower().Contains(input.Query.Name.ToLower()));
            return DataRetrievalManager.Instance.ProcessResult(input, cachedBEDefinitions.ToBigResult(input, filterExpression, BusinessEntityDefinitionDetailMapper));
        }
        public int GetBusinessEntityDefinitionId(string businessEntityDefinitionName)
        {
            var cachedBEDefinitions = GetCachedBusinessEntityDefinitions();
            var businessEntityDefinition = cachedBEDefinitions.FindRecord(x=>x.Name == businessEntityDefinitionName);
            return businessEntityDefinition.BusinessEntityDefinitionId;
        }

        public BusinessEntityDefinition GetBusinessEntityDefinition(int businessEntityDefinitionId)
        {
            var cachedBEDefinitions = GetCachedBusinessEntityDefinitions();
            return cachedBEDefinitions.FindRecord(beDefinition => beDefinition.BusinessEntityDefinitionId == businessEntityDefinitionId);
        }

        public IEnumerable<BusinessEntityDefinitionInfo> GetBusinessEntityDefinitionsInfo(BusinessEntityDefinitionInfoFilter filter)
        {
            var cachedBEDefinitions = GetCachedBusinessEntityDefinitions();
            Func<BusinessEntityDefinition, bool> filterExpression = null;

            if (filter != null)
            {
                // Set filterExpression
            }

            return cachedBEDefinitions.MapRecords(BusinessEntityDefinitionInfoMapper, filterExpression);
        }

        #endregion

        #region Private Methods

        private Dictionary<int, BusinessEntityDefinition> GetCachedBusinessEntityDefinitions()
        {
            return CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetBusinessEntityDefinitions",
                () =>
                {
                    IBusinessEntityDefinitionDataManager dataManager = GenericDataDataManagerFactory.GetDataManager<IBusinessEntityDefinitionDataManager>();
                    IEnumerable<BusinessEntityDefinition> beDefinitions = dataManager.GetBusinessEntityDefinitions();
                    return beDefinitions.ToDictionary(beDefinition => beDefinition.BusinessEntityDefinitionId, beDefinition => beDefinition);
                });
        }

        #endregion

        #region Private Classes

        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IBusinessEntityDefinitionDataManager _dataManager = GenericDataDataManagerFactory.GetDataManager<IBusinessEntityDefinitionDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreGenericRuleDefinitionsUpdated(ref _updateHandle);
            }
        }

        #endregion

        #region Mappers

        BusinessEntityDefinitionInfo BusinessEntityDefinitionInfoMapper(BusinessEntityDefinition beDefinition)
        {
            return new BusinessEntityDefinitionInfo()
            {
                BusinessEntityDefinitionId = beDefinition.BusinessEntityDefinitionId,
                Name = beDefinition.Name
            };
        }
        BusinessEntityDefinitionDetail BusinessEntityDefinitionDetailMapper(BusinessEntityDefinition beDefinition)
        {
            return new BusinessEntityDefinitionDetail()
            {
                Entity = beDefinition,
            };
        }
        
        #endregion
    }
}
