using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Caching;
using Vanrise.Common;
using Vanrise.Entities;
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
        
        public GenericRuleDefinition GetGenericRuleDefinition(int genericRuleDefinitionId)
        {
            var cachedGenericRuleDefinitions = GetCachedGenericRuleDefinitions();
            return cachedGenericRuleDefinitions.FindRecord((genericRuleDefinition) => genericRuleDefinition.GenericRuleDefinitionId == genericRuleDefinitionId);
        }

        public Vanrise.Entities.InsertOperationOutput<GenericRuleDefinition> AddGenericRuleDefinition(GenericRuleDefinition genericRuleDefinition)
        {
            InsertOperationOutput<GenericRuleDefinition> insertOperationOutput = new Vanrise.Entities.InsertOperationOutput<GenericRuleDefinition>();

            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;
            int genericRuleDefinitionId = -1;

            IGenericRuleDefinitionDataManager dataManager = GenericDataDataManagerFactory.GetDataManager<IGenericRuleDefinitionDataManager>();
            bool added = dataManager.AddGenericRuleDefinition(genericRuleDefinition, out genericRuleDefinitionId);

            if (added)
            {
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                genericRuleDefinition.GenericRuleDefinitionId = genericRuleDefinitionId;
                insertOperationOutput.InsertedObject = genericRuleDefinition;

                CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired("GetGenericRuleDefinitions");
            }
            else
            {
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.SameExists;
            }

            return insertOperationOutput;
        }

        public Vanrise.Entities.UpdateOperationOutput<GenericRuleDefinition> UpdateGenericRuleDefinition(GenericRuleDefinition genericRuleDefinition)
        {
            UpdateOperationOutput<GenericRuleDefinition> updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<GenericRuleDefinition>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = genericRuleDefinition;
            int genericRuleDefinitionId = -1;

            IGenericRuleDefinitionDataManager dataManager = GenericDataDataManagerFactory.GetDataManager<IGenericRuleDefinitionDataManager>();
            bool added = dataManager.UpdateGenericRuleDefinition(genericRuleDefinition);

            if (added)
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired("GetGenericRuleDefinitions");
            }
            else
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
            }

            return updateOperationOutput;
        }

        public Vanrise.Entities.DeleteOperationOutput<object> DeleteGenericRuleDefinition(int genericRuleDefinitionId)
        {
            DeleteOperationOutput<object> deleteOperationOutput = new DeleteOperationOutput<object>();
            
            IGenericRuleDefinitionDataManager dataManager = GenericDataDataManagerFactory.GetDataManager<IGenericRuleDefinitionDataManager>();
            bool deleted = dataManager.DeleteGenericRuleDefinition(genericRuleDefinitionId);

            deleteOperationOutput.Result = (deleted) ? DeleteOperationResult.Succeeded : DeleteOperationResult.Failed;
            return deleteOperationOutput;
        }

        #endregion

        #region Private Methods

        private Dictionary<int, GenericRuleDefinition> GetCachedGenericRuleDefinitions()
        {
            return CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetGenericRuleDefinitions",
                () =>
                {
                    IGenericRuleDefinitionDataManager dataManager = GenericDataDataManagerFactory.GetDataManager<IGenericRuleDefinitionDataManager>();
                    IEnumerable<GenericRuleDefinition> genericRuleDefinitions = dataManager.GetGenericRuleDefinitions();
                    var dictionary = genericRuleDefinitions.ToDictionary(genericRuleDefinition => genericRuleDefinition.GenericRuleDefinitionId, genericRuleDefinition => genericRuleDefinition);
                    return dictionary;
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
