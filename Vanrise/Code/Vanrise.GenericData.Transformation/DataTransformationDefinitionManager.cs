using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;
using Vanrise.GenericData.Transformation.Entities;
using Vanrise.Common;
using Vanrise.Caching;
using Vanrise.GenericData.Transformation.Data;
namespace Vanrise.GenericData.Transformation
{
    public class DataTransformationDefinitionManager
    {
    
        #region Public Methods
        public IDataRetrievalResult<DataTransformationDefinitionDetail> GetFilteredDataTransformationDefinitions(DataRetrievalInput<DataTransformationDefinitionQuery> input)
        {
            var allItems = GetCachedDataTransformationDefinitions();

            Func<DataTransformationDefinition, bool> filterExpression = (itemObject) => 
                (input.Query.Name == null || itemObject.Name.ToLower().Contains(input.Query.Name.ToLower()));

            return DataRetrievalManager.Instance.ProcessResult(input, allItems.ToBigResult(input, filterExpression, DataTransformationDefinitionDetailMapper));
        }
        public DataTransformationDefinition GetDataTransformationDefinition(int dataTransformationDefinitionId)
        {
            var dataDataTransformationDefinitions = GetCachedDataTransformationDefinitions();
            return dataDataTransformationDefinitions.GetRecord(dataTransformationDefinitionId);
        }
        public string GetDataTransformationDefinitionName(int? dataTransformationDefinitionId)
        {
            if (dataTransformationDefinitionId == null)
                return null;
            var dataTransformationDefinitions = GetCachedDataTransformationDefinitions();

            DataTransformationDefinition dataTransformationDefinition = dataTransformationDefinitions.GetRecord((int)dataTransformationDefinitionId);

            if (dataTransformationDefinition != null)
                return dataTransformationDefinition.Name;

            return null;
        }
        public Vanrise.Entities.InsertOperationOutput<DataTransformationDefinitionDetail> AddDataTransformationDefinition(DataTransformationDefinition dataTransformationDefinition)
        {
            InsertOperationOutput<DataTransformationDefinitionDetail> insertOperationOutput = new Vanrise.Entities.InsertOperationOutput<DataTransformationDefinitionDetail>();

            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;
            int dataTransformationDefinitionId = -1;


            IDataTransformationDefinitionDataManager dataManager = DataTransformationDefinitionDataManagerFactory.GetDataManager<IDataTransformationDefinitionDataManager>();
            bool insertActionSucc = dataManager.AddDataTransformationDefinition(dataTransformationDefinition, out dataTransformationDefinitionId);

            if (insertActionSucc)
            {
                CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired("GetDataTransformationDefinitions");
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                dataTransformationDefinition.DataTransformationDefinitionId = dataTransformationDefinitionId;
                insertOperationOutput.InsertedObject = DataTransformationDefinitionDetailMapper(dataTransformationDefinition);
            }
            else
            {
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.SameExists;
            }

            return insertOperationOutput;
        }
        public Vanrise.Entities.UpdateOperationOutput<DataTransformationDefinitionDetail> UpdateDataTransformationDefinition(DataTransformationDefinition dataTransformationDefinition)
        {
            IDataTransformationDefinitionDataManager dataManager = DataTransformationDefinitionDataManagerFactory.GetDataManager<IDataTransformationDefinitionDataManager>();
            bool updateActionSucc = dataManager.UpdateDataTransformationDefinition(dataTransformationDefinition);
            UpdateOperationOutput<DataTransformationDefinitionDetail> updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<DataTransformationDefinitionDetail>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            if (updateActionSucc)
            {
                CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired("GetDataTransformationDefinitions");
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = DataTransformationDefinitionDetailMapper(dataTransformationDefinition);
            }
            else
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
            }

            return updateOperationOutput;
        }
        #endregion

        #region Private Methods
        private Dictionary<int, DataTransformationDefinition> GetCachedDataTransformationDefinitions()
        {
            return CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetDataTransformationDefinitions",
               () =>
               {
                   IDataTransformationDefinitionDataManager dataManager = DataTransformationDefinitionDataManagerFactory.GetDataManager<IDataTransformationDefinitionDataManager>();
                   IEnumerable<DataTransformationDefinition> dataTransformationDefinitiones = dataManager.GetDataTransformationDefinitions();
                   return dataTransformationDefinitiones.ToDictionary(kvp => kvp.DataTransformationDefinitionId, kvp => kvp);
               });
        }

        #endregion

        #region Private Classes
        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IDataTransformationDefinitionDataManager _dataManager = DataTransformationDefinitionDataManagerFactory.GetDataManager<IDataTransformationDefinitionDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreDataTransformationDefinitionUpdated(ref _updateHandle);
            }
        }


        #endregion

        #region Mappers

        private DataTransformationDefinitionDetail DataTransformationDefinitionDetailMapper(DataTransformationDefinition dataTransformationDefinition)
        {
            DataTransformationDefinitionDetail dataTransformationDefinitionDetail = new DataTransformationDefinitionDetail();
            dataTransformationDefinitionDetail.Entity = dataTransformationDefinition;
            return dataTransformationDefinitionDetail;
        }
        #endregion
    }
}
