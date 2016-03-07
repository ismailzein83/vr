using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Caching;
using Vanrise.Entities;
using Vanrise.GenericData.Data;
using Vanrise.GenericData.Entities;
using Vanrise.Common;
using System.Collections.Concurrent;
using System.Reflection;
namespace Vanrise.GenericData.Business
{
    public class GenericBusinessEntityManager:BaseBEManager
    {
        
        #region Public Methods
        
        public Vanrise.Entities.IDataRetrievalResult<GenericBusinessEntityDetail> GetFilteredGenericBusinessEntities(Vanrise.Entities.DataRetrievalInput<GenericBusinessEntityQuery> input)
        {
            var cachedGenericBusinessEntities = GetCachedGenericBusinessEntities(input.Query.BusinessEntityDefinitionId);
            IEnumerable<GenericFilterRuntimeField> runtimeFilters = GetGenericFilterRuntimeFields(input.Query.BusinessEntityDefinitionId);

            Func<GenericBusinessEntity, bool> filterExpression = (itm) => (input.Query.BusinessEntityDefinitionId == itm.BusinessEntityDefinitionId && (input.Query.FilterValuesByFieldPath == null || MatchGenericBusinessEntity(runtimeFilters, input.Query.FilterValuesByFieldPath, itm)));
            
            return DataRetrievalManager.Instance.ProcessResult(input, cachedGenericBusinessEntities.ToBigResult(input, filterExpression, GenericBusinessEntityDetailMapper));
        }

        public GenericBusinessEntity GetGenericBusinessEntity(long genericBusinessEntityId, int businessEntityDefinitionId)
        {
            var cachedGenericBusinessEntities = GetCachedGenericBusinessEntities(businessEntityDefinitionId);
            return cachedGenericBusinessEntities.GetRecord(genericBusinessEntityId);
        }

        public Vanrise.Entities.UpdateOperationOutput<GenericBusinessEntityDetail> UpdateGenericBusinessEntity(GenericBusinessEntity genericBusinessEntity)
        {
            UpdateOperationOutput<GenericBusinessEntityDetail> updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<GenericBusinessEntityDetail>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            IGenericBusinessEntityDataManager dataManager = GenericDataDataManagerFactory.GetDataManager<IGenericBusinessEntityDataManager>();
            bool updateActionSucc = dataManager.UpdateGenericBusinessEntity(genericBusinessEntity);

            if (updateActionSucc)
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired(genericBusinessEntity.BusinessEntityDefinitionId);
                updateOperationOutput.UpdatedObject = GenericBusinessEntityDetailMapper(genericBusinessEntity);
            }
            return updateOperationOutput;
        }
        
        public Vanrise.Entities.InsertOperationOutput<GenericBusinessEntityDetail> AddGenericBusinessEntity(GenericBusinessEntity genericBusinessEntity)
        {
            InsertOperationOutput<GenericBusinessEntityDetail> insertOperationOutput = new Vanrise.Entities.InsertOperationOutput<GenericBusinessEntityDetail>();

            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;
            long genericBusinessEntityId = -1;
            IGenericBusinessEntityDataManager dataManager = GenericDataDataManagerFactory.GetDataManager<IGenericBusinessEntityDataManager>();
            bool insertActionSucc = dataManager.AddGenericBusinessEntity(genericBusinessEntity, out genericBusinessEntityId);
            if (insertActionSucc)
            {
                    insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                    genericBusinessEntity.GenericBusinessEntityId = genericBusinessEntityId;
                    insertOperationOutput.InsertedObject = GenericBusinessEntityDetailMapper(genericBusinessEntity);
                    CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired(genericBusinessEntity.BusinessEntityDefinitionId);
            }
            else
            {
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.SameExists;

            }

            return insertOperationOutput;
        }
        
        public IEnumerable<GenericBusinessEntity> GetGenericBusinessEntities(int businessEntityDefinitionId)
        {
            var cachedGenericBusinessEntities = GetCachedGenericBusinessEntities(businessEntityDefinitionId);
            return cachedGenericBusinessEntities.Values;
        }

        public dynamic GetFieldPathValue(GenericBusinessEntity entity, string fieldPath)
        {
            if (entity.Details == null)
                return null;
           // Type entityType = entity.Details.GetType();
            return entity.Details[fieldPath];
            //var obj = entityType.GetProperties();
            //var ds = obj.First(x => x.Name == fieldPath).GetValue(entity.Details);
            //return ds;
            //var entityField = entityType.GetField(fieldPath, BindingFlags.NonPublic | BindingFlags.Instance);
            //if (entityField == null)
            //    return null;
            //return entityField.GetValue(entity.Details);
        }

        #endregion
        
        #region Private Methods
        
        private Dictionary<long, GenericBusinessEntity> GetCachedGenericBusinessEntities(int businessDefinitionId)
        {
            return CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject(String.Format("GetGenericBusinessEntities_{0}", businessDefinitionId), businessDefinitionId,
               () =>
               {
                   IGenericBusinessEntityDataManager dataManager = GenericDataDataManagerFactory.GetDataManager<IGenericBusinessEntityDataManager>();
                   IEnumerable<GenericBusinessEntity> genericBusinessEntities = dataManager.GetGenericBusinessEntitiesByDefinition(businessDefinitionId);
                   return genericBusinessEntities.ToDictionary(kvp => kvp.GenericBusinessEntityId, kvp => kvp);
               });
        }
        
        IEnumerable<GenericFilterRuntimeField> GetGenericFilterRuntimeFields(int businessEntityDefinitionId)
        {
            GenericManagementRuntime managementRuntime = new GenericUIRuntimeManager().GetManagementRuntime(businessEntityDefinitionId);
            if (managementRuntime == null)
                throw new NullReferenceException("managementRuntime");
            if (managementRuntime.Filter == null)
                throw new NullReferenceException("managementRuntime.Filter");
            if (managementRuntime.Filter.Fields == null || managementRuntime.Filter.Fields.Count == 0)
                throw new NullReferenceException("managementRuntime.Filter.Fields");
            return managementRuntime.Filter.Fields;
        }

        bool MatchGenericBusinessEntity(IEnumerable<GenericFilterRuntimeField> runtimeFilters, Dictionary<string, object> filterValuesByFieldPath, GenericBusinessEntity genericBusinessEntity)
        {
            foreach (var runtimeFilter in runtimeFilters)
            {
                object filterValue;
                filterValuesByFieldPath.TryGetValue(runtimeFilter.FieldPath, out filterValue);

                if (filterValue == null)
                    continue;

                if (!runtimeFilter.FieldType.IsMatched(GetFieldPathValue(genericBusinessEntity, runtimeFilter.FieldPath).Value, filterValue))
                    return false;
            }

            return true;
        }

        #endregion

        #region Private Classes
        
        public class CacheManager : Vanrise.Caching.BaseCacheManager<int>
        {
            IGenericBusinessEntityDataManager _dataManager = GenericDataDataManagerFactory.GetDataManager<IGenericBusinessEntityDataManager>();
            ConcurrentDictionary<int, Object> _updateHandlesByRuleType = new ConcurrentDictionary<int, Object>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(int parameter)
            {
                Object updateHandle;
                _updateHandlesByRuleType.TryGetValue(parameter, out updateHandle);
                bool isCacheExpired = _dataManager.AreGenericBusinessEntityUpdated(parameter, ref updateHandle);
                _updateHandlesByRuleType.AddOrUpdate(parameter, updateHandle, (key, existingHandle) => updateHandle);
                return isCacheExpired;
            }
        }

        #endregion

        #region Mappers
        
        private GenericBusinessEntityDetail GenericBusinessEntityDetailMapper(GenericBusinessEntity genericBusinessEntity)
        {
            BusinessEntityDefinitionManager businessEntityDefinitionManager = new Business.BusinessEntityDefinitionManager();
            var businessEntityDefinition = businessEntityDefinitionManager.GetBusinessEntityDefinition(genericBusinessEntity.BusinessEntityDefinitionId);

            if(businessEntityDefinition == null)
                throw new NullReferenceException("businessEntityDefinition");

            GenericBEDefinitionSettings definitionSettings = businessEntityDefinition.Settings as GenericBEDefinitionSettings;
      
            if (definitionSettings == null)
                throw new NullReferenceException("definitionSettings");
          
            DataRecordTypeManager recordTypeManager = new DataRecordTypeManager();
          
            var recordType = recordTypeManager.GetDataRecordType(definitionSettings.DataRecordTypeId);
            if (recordType == null)
                throw new NullReferenceException("recordType");
           
            GenericUIRuntimeManager uiRuntimeManager = new GenericUIRuntimeManager();
            GenericBusinessEntityDetail entityDetail = new GenericBusinessEntityDetail();
            entityDetail.Entity = genericBusinessEntity;
            entityDetail.FieldValueDescriptions = new List<string>();
            foreach (var columnConfig in definitionSettings.ManagementDesign.GridDesign.Columns)
            {
                var columnValue = GetFieldPathValue(genericBusinessEntity, columnConfig.FieldPath);
                if (columnValue != null)
                {
                    var uiRuntimeField = uiRuntimeManager.BuildRuntimeField<GenericEditorRuntimeField>(columnConfig, recordType.Fields, recordType.DataRecordTypeId);
                    entityDetail.FieldValueDescriptions.Add(uiRuntimeField.FieldType.GetDescription(columnValue));
                }
                else
                    entityDetail.FieldValueDescriptions.Add(null);
            }
          
            return entityDetail;
        }

        #endregion
    }
}
 