﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;
using Vanrise.GenericData.Transformation.Entities;
using Vanrise.Common;
using Vanrise.Caching;
using Vanrise.GenericData.Transformation.Data;
using Vanrise.Common.Business;
using Vanrise.GenericData.Business;
namespace Vanrise.GenericData.Transformation
{
    public class DataTransformationDefinitionManager
    {

        #region Public Methods


        public IEnumerable<DataTransformationDefinitionInfo> GetDataTransformationDefinitions(DataTransformationDefinitionFilter filter)
        {
            var cachedDataTransformationDefinitions = GetCachedDataTransformationDefinitions();
            return cachedDataTransformationDefinitions.MapRecords(DataTransformationDefinitionInfoMapper, null);

        }

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

        public IEnumerable<DataTransformationRecordType> GetDataTransformationDefinitionRecords(int dataTransformationDefinitionId)
        {
            var dataTransformationDefinition = GetDataTransformationDefinition(dataTransformationDefinitionId);
            if (dataTransformationDefinition != null)
                return dataTransformationDefinition.RecordTypes;
            else
                return null;
        }
        public string GetDataTransformationDefinitionName(int dataTransformationDefinitionId)
        {
            var dataTransformationDefinitions = GetCachedDataTransformationDefinitions();

            DataTransformationDefinition dataTransformationDefinition = dataTransformationDefinitions.GetRecord(dataTransformationDefinitionId);

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
                CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
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
                CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = DataTransformationDefinitionDetailMapper(dataTransformationDefinition);
            }
            else
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
            }

            return updateOperationOutput;
        }
        public List<Vanrise.Entities.TemplateConfig> GetMappingStepsTemplates()
        {
            TemplateConfigManager manager = new TemplateConfigManager();
            return manager.GetTemplateConfigurations(Constants.MappingStepConfigType);
        }

        internal DataTransformationRuntimeType GetTransformationRuntimeType(int dataTransformationDefinitionId)
        {
            string cacheName = String.Format("GetTransformationRuntimeType_{0}", dataTransformationDefinitionId);
            return CacheManagerFactory.GetCacheManager<RuntimeCacheManager>().GetOrCreateObject(cacheName,
              () =>
              {
                  var dataTransformationDefinition = GetDataTransformationDefinition(dataTransformationDefinitionId);
                  if (dataTransformationDefinition == null)
                      throw new NullReferenceException(String.Format("dataTransformationDefinition '{0}'", dataTransformationDefinitionId));
                  DataTransformationRuntimeType runtimeType;
                  List<string> errorMessages;

                  if (TryCompileDataTransformation(dataTransformationDefinition, out runtimeType, out errorMessages))
                      return runtimeType;
                  else
                  {
                      StringBuilder errorsBuilder = new StringBuilder();
                      if (errorMessages != null)
                      {
                          foreach (var errorMessage in errorMessages)
                          {
                              errorsBuilder.AppendLine(errorMessage);
                          }
                      }
                      throw new Exception(String.Format("Compile Error when building executor type for data transformation definition Id '{0}'. Errors: {1}",
                          dataTransformationDefinitionId, errorsBuilder));
                  }
              });
        }

        public bool TryCompileDataTransformation(DataTransformationDefinition dataTransformationDefinition, out DataTransformationRuntimeType runtimeType, out List<string> errorMessages)
        {
            DataTransformationCodeGenerationContext codeGenerationContext = new DataTransformationCodeGenerationContext(dataTransformationDefinition);
            return codeGenerationContext.TryBuildRuntimeType(out runtimeType, out errorMessages);
        }

        public IEnumerable<DataTransformationRecordType> GetDataTransformationDefinitionRecords(int dataTransformationDefinitionId, DataTransformationRecordTypeInfoFilter filter)
        {
            var dataTransformationDefinition = GetDataTransformationDefinition(dataTransformationDefinitionId);
            if (dataTransformationDefinition != null)
            {
                Func<DataTransformationRecordType, bool> filterExpression = (x) => (filter.DataRecordTypeIds == null || filter.DataRecordTypeIds.Count() == 0 || filter.DataRecordTypeIds.Contains(x.DataRecordTypeId.Value));

                return dataTransformationDefinition.RecordTypes.FindAllRecords(filterExpression);
            }
            else
                return null;
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
        public class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IDataTransformationDefinitionDataManager _dataManager = DataTransformationDefinitionDataManagerFactory.GetDataManager<IDataTransformationDefinitionDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired()
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

        private DataTransformationDefinitionInfo DataTransformationDefinitionInfoMapper(DataTransformationDefinition dataTransformationDefinition)
        {
            DataTransformationDefinitionInfo dataTransformationDefinitionInfo = new DataTransformationDefinitionInfo();
            dataTransformationDefinitionInfo.DataTransformationDefinitionId = dataTransformationDefinition.DataTransformationDefinitionId;
            dataTransformationDefinitionInfo.Name = dataTransformationDefinition.Name;
            return dataTransformationDefinitionInfo;
        }

        #endregion
    }
}
