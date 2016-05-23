﻿using System;
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
    public class GenericBusinessEntityManager : BaseBEManager, IBusinessEntityManager, IGenericBusinessEntityManager
    {
        #region Fields / Constructors

        private GenericUIRuntimeManager _uiRuntimeManager;
        private BusinessEntityDefinitionManager _businessEntityDefinitionManager;
        private DataRecordTypeManager _recordTypeManager;
        public GenericBusinessEntityManager()
        {
            _uiRuntimeManager = new GenericUIRuntimeManager();
            _businessEntityDefinitionManager = new Business.BusinessEntityDefinitionManager();
            _recordTypeManager = new DataRecordTypeManager();
        }
        #endregion

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
            ConvertDetailsToDataRecord(genericBusinessEntity);
            var updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<GenericBusinessEntityDetail>();
            updateOperationOutput.UpdatedObject = null;

            if (IsEntityTitleValid(genericBusinessEntity))
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;

                IGenericBusinessEntityDataManager dataManager = GenericDataDataManagerFactory.GetDataManager<IGenericBusinessEntityDataManager>();
                bool updated = dataManager.UpdateGenericBusinessEntity(genericBusinessEntity);

                if (updated)
                {
                    updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                    CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired(genericBusinessEntity.BusinessEntityDefinitionId);
                    updateOperationOutput.UpdatedObject = GenericBusinessEntityDetailMapper(genericBusinessEntity);
                }
            }
            else
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
            }

            return updateOperationOutput;
        }        
        public Vanrise.Entities.InsertOperationOutput<GenericBusinessEntityDetail> AddGenericBusinessEntity(GenericBusinessEntity genericBusinessEntity)
        {
            ConvertDetailsToDataRecord(genericBusinessEntity);
            var insertOperationOutput = new Vanrise.Entities.InsertOperationOutput<GenericBusinessEntityDetail>();
            insertOperationOutput.InsertedObject = null;
            
            if (IsEntityTitleValid(genericBusinessEntity))
            {
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
                long genericBusinessEntityId = -1;

                IGenericBusinessEntityDataManager dataManager = GenericDataDataManagerFactory.GetDataManager<IGenericBusinessEntityDataManager>();
                bool inserted = dataManager.AddGenericBusinessEntity(genericBusinessEntity, out genericBusinessEntityId);

                if (inserted)
                {
                    insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                    genericBusinessEntity.GenericBusinessEntityId = genericBusinessEntityId;
                    insertOperationOutput.InsertedObject = GenericBusinessEntityDetailMapper(genericBusinessEntity);
                    CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired(genericBusinessEntity.BusinessEntityDefinitionId);
                }
            }
            else
            {
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.SameExists;
            }

            return insertOperationOutput;
        }
        public Vanrise.Entities.DeleteOperationOutput<object> DeleteGenericBusinessEntity(long genericBusinessEntityId, int businessEntityDefinitionId)
        {
            var deleteOperationOutput = new Vanrise.Entities.DeleteOperationOutput<object>()
            {
                Result = Vanrise.Entities.DeleteOperationResult.Failed
            };

            IGenericBusinessEntityDataManager dataManager = GenericDataDataManagerFactory.GetDataManager<IGenericBusinessEntityDataManager>();
            bool deleted = dataManager.DeleteGenericBusinessEntity(genericBusinessEntityId, businessEntityDefinitionId);

            if (deleted)
            {
                deleteOperationOutput.Result = Vanrise.Entities.DeleteOperationResult.Succeeded;
                CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired(businessEntityDefinitionId);
            }

            return deleteOperationOutput;
        }
        public IEnumerable<GenericBusinessEntity> GetGenericBusinessEntities(int businessEntityDefinitionId)
        {
            var cachedGenericBusinessEntities = GetCachedGenericBusinessEntities(businessEntityDefinitionId);
            return cachedGenericBusinessEntities.Values;
        }       
        public IEnumerable<GenericBusinessEntityInfo> GetGenericBusinessEntityInfo( int businessEntityDefinitionId,GenericBusinessEntityFilter filter)
        {
            var cachedGenericBusinessEntities = GetCachedGenericBusinessEntities(businessEntityDefinitionId);
            if (filter != null)
            {
                
            }
            return cachedGenericBusinessEntities.MapRecords(GenericBusinessEntityInfoMapper);
        }
        public dynamic GetFieldPathValue(GenericBusinessEntity entity, string fieldPath)
        {
            if (entity.Details == null)
                return null;
            Type entityType = entity.Details.GetType();
            var field = entityType.GetProperty(fieldPath);
            if (field == null)
                throw new NullReferenceException(String.Format("field {0}", fieldPath));
            return field.GetValue(entity.Details);
            //return entity.Details[fieldPath];
            //var obj = entityType.GetProperties();
            //var ds = obj.First(x => x.Name == fieldPath).GetValue(entity.Details);
            //return ds;
            //var entityField = entityType.GetField(fieldPath, BindingFlags.NonPublic | BindingFlags.Instance);
            //if (entityField == null)
            //    return null;
            //return entityField.GetValue(entity.Details);
        }
        public string GetEntityDescription(IBusinessEntityDescriptionContext context)
        {
            var entityDefinitionSettings = context.EntityDefinition.Settings as GenericBEDefinitionSettings;
            Dictionary<long, GenericBusinessEntity> cachedEntities = GetCachedGenericBusinessEntities(context.EntityDefinition.BusinessEntityDefinitionId);

            List<string> descriptions = new List<string>();

            foreach (long entityId in context.EntityIds)
            {
                GenericBusinessEntity entity = GetGenericBusinessEntity(entityId, context.EntityDefinition.BusinessEntityDefinitionId);
                if (entity == null) throw new NullReferenceException("entity");
                string entityDescription = GetFieldPathValue(entity, entityDefinitionSettings.FieldPath);
                descriptions.Add(entityDescription);
            }

            return (descriptions.Count > 0) ? String.Join(",", descriptions) : null;
        }
        public GenericBusinessEntityTitle GetBusinessEntityTitle(int businessEntityDefinitionId,long? genericBussinessEntityId = null)
        {
           
            var businessEntityDefinition = GetBusinessEntityDefinition(businessEntityDefinitionId);
          
            GenericBusinessEntityTitle entityTitle = new GenericBusinessEntityTitle();
            entityTitle.Title = businessEntityDefinition.Title;
           
            if(genericBussinessEntityId !=null)
            {
                GenericBEDefinitionSettings definitionSettings = businessEntityDefinition.Settings as GenericBEDefinitionSettings;
                if (definitionSettings == null)
                    throw new NullReferenceException("definitionSettings");          
               
                var genericBusinessEntity = GetGenericBusinessEntity((long)genericBussinessEntityId, businessEntityDefinitionId);
                entityTitle.EntityName = GetFieldDescription(genericBusinessEntity, definitionSettings.FieldPath, definitionSettings.DataRecordTypeId);
            }
            return entityTitle;
        }        
        public bool IsMatched(IBusinessEntityMatchContext context)
        {
            if (context.FieldValueIds == null || context.FilterIds == null) return true;

            var fieldValueIds = context.FieldValueIds.MapRecords(itm => Convert.ToInt32(itm));
            var filterIds = context.FilterIds.MapRecords(itm => Convert.ToInt32(itm));
            foreach (var filterId in filterIds)
            {
                if (fieldValueIds.Contains(filterId))
                    return true;
            }
            return false;
        }
        public int GetDataRecordTypeId(int businessEntityDefinitionId)
        {
            BusinessEntityDefinition beDefinition = new BusinessEntityDefinitionManager().GetBusinessEntityDefinition(businessEntityDefinitionId);
            if (beDefinition == null)
                throw new NullReferenceException("beDefinition");
            if (beDefinition.Settings == null)
                throw new NullReferenceException("beDefinition.Settings");

            GenericBEDefinitionSettings gbeDefinitionSettings = beDefinition.Settings as GenericBEDefinitionSettings;
            if (gbeDefinitionSettings == null)
                throw new NullReferenceException("gbeDefinitionSettings");

            return gbeDefinitionSettings.DataRecordTypeId;
        }
        #endregion
        
        #region Private Methods
        private BusinessEntityDefinition GetBusinessEntityDefinition(int businessEntityDefinitionId)
        {
             var businessEntityDefinition = _businessEntityDefinitionManager.GetBusinessEntityDefinition(businessEntityDefinitionId);

            if (businessEntityDefinition == null)
                throw new NullReferenceException("businessEntityDefinition");
            return businessEntityDefinition;
        }
        private GenericBEDefinitionSettings GetGenericBEDefinitionSettings(int businessEntityDefinitionId)
        {

            var businessEntityDefinition = GetBusinessEntityDefinition(businessEntityDefinitionId);

            GenericBEDefinitionSettings definitionSettings = businessEntityDefinition.Settings as GenericBEDefinitionSettings;

            if (definitionSettings == null)
                throw new NullReferenceException("definitionSettings");

            return definitionSettings;
        }
        private DataRecordType GetDataRecordType(int dataRecordTypeId)
        {
            var recordType = _recordTypeManager.GetDataRecordType(dataRecordTypeId);
            if (recordType == null)
                throw new NullReferenceException("recordType");
            return recordType;
        }
        private string GetFieldDescription(GenericBusinessEntity genericBusinessEntity, string fieldPath, int dataRecordTypeId)
        {
            var recordType = GetDataRecordType(dataRecordTypeId);
            var columnValue = GetFieldPathValue(genericBusinessEntity, fieldPath);
            if (columnValue != null)
            {
                var uiRuntimeField = _uiRuntimeManager.GetFieldType(fieldPath, recordType.Fields, recordType.DataRecordTypeId);
                return uiRuntimeField.GetDescription(columnValue);
            }
            return null;
        }
        private string GetFieldDescription<T>(GenericUIField field, GenericBusinessEntity genericBusinessEntity, string fieldPath, int dataRecordTypeId) where T : GenericUIRuntimeField
        {
            var recordType = GetDataRecordType(dataRecordTypeId);
            var columnValue = GetFieldPathValue(genericBusinessEntity, fieldPath);
            if (columnValue != null)
            {
                var uiRuntimeField = _uiRuntimeManager.BuildRuntimeField<T>(field, recordType.Fields, recordType.DataRecordTypeId);
                //var value = (columnValue.Value != null) ? columnValue.Value : columnValue;
                return uiRuntimeField.FieldType.GetDescription(columnValue);
            }
            return null;
        }
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

                dynamic fieldValue = GetFieldPathValue(genericBusinessEntity, runtimeFilter.FieldPath);
                if (fieldValue == null)
                    continue;

                if (!runtimeFilter.FieldType.IsMatched(fieldValue, filterValue))
                    return false;
            }

            return true;
        }
        bool IsEntityTitleValid(GenericBusinessEntity entity)
        {
            BusinessEntityDefinition beDefinition = new BusinessEntityDefinitionManager().GetBusinessEntityDefinition(entity.BusinessEntityDefinitionId);
            if (beDefinition == null)
                throw new NullReferenceException("beDefinition");

            var genericBEDefinitionSettings = beDefinition.Settings as GenericBEDefinitionSettings;
            if (genericBEDefinitionSettings == null)
                throw new NullReferenceException("genericBEDefinitionSettings");

            string fieldPath = genericBEDefinitionSettings.FieldPath;
            if (fieldPath == null)
                throw new NullReferenceException("genericBEDefinitionSettings.FieldPath");

            dynamic entityTitle = GetFieldPathValue(entity, fieldPath);
            if (entityTitle == null)
                throw new NullReferenceException("entityTitle");

            string entityTitleString = entityTitle.ToString().ToLower();

            Dictionary<long, GenericBusinessEntity> cachedEntites = GetCachedGenericBusinessEntities(entity.BusinessEntityDefinitionId);
            GenericBusinessEntity entityMatch = cachedEntites.FindRecord(itm => (entity.GenericBusinessEntityId == 0 || itm.GenericBusinessEntityId != entity.GenericBusinessEntityId) && GetFieldPathValue(itm, fieldPath).ToString().ToLower() == entityTitleString);

            return (entityMatch == null);
        }
        void ConvertDetailsToDataRecord(GenericBusinessEntity genericBusinessEntity)
        {
            if (genericBusinessEntity == null || genericBusinessEntity.Details == null)
                return;
            int dataRecordTypeId = GetDataRecordTypeId(genericBusinessEntity.BusinessEntityDefinitionId);
            genericBusinessEntity.Details = _recordTypeManager.ConvertDynamicToDataRecord(genericBusinessEntity.Details, dataRecordTypeId);
        }
        #endregion

        #region Private Classes
        
        public class CacheManager : Vanrise.Caching.BaseCacheManager<int>
        {
            IGenericBusinessEntityDataManager _dataManager = GenericDataDataManagerFactory.GetDataManager<IGenericBusinessEntityDataManager>();
            ConcurrentDictionary<int, Object> _updateHandlesByRuleType = new ConcurrentDictionary<int, Object>();            

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
            GenericBEDefinitionSettings definitionSettings = GetGenericBEDefinitionSettings(genericBusinessEntity.BusinessEntityDefinitionId);
            GenericBusinessEntityDetail entityDetail = new GenericBusinessEntityDetail();
            entityDetail.Entity = genericBusinessEntity;
            entityDetail.FieldValueDescriptions = new List<string>();
            foreach (var columnConfig in definitionSettings.ManagementDesign.GridDesign.Columns)
            {
                entityDetail.FieldValueDescriptions.Add(GetFieldDescription<GenericEditorRuntimeField>(columnConfig, genericBusinessEntity, columnConfig.FieldPath, definitionSettings.DataRecordTypeId));
            }
            return entityDetail;
        }
        private GenericBusinessEntityInfo GenericBusinessEntityInfoMapper(GenericBusinessEntity genericBusinessEntity)
        {
            GenericBEDefinitionSettings definitionSettings = GetGenericBEDefinitionSettings(genericBusinessEntity.BusinessEntityDefinitionId);
            GenericBusinessEntityInfo entityInfo = new GenericBusinessEntityInfo();
            entityInfo.GenericBusinessEntityId = genericBusinessEntity.GenericBusinessEntityId;
            entityInfo.Name = GetFieldDescription(genericBusinessEntity, definitionSettings.FieldPath, definitionSettings.DataRecordTypeId);
            return entityInfo;
        }
       
        #endregion

        public dynamic GetEntity(IBusinessEntityGetByIdContext context)
        {
            return GetGenericBusinessEntity(context.EntityId, context.EntityDefinitionId);
        }
        
        public List<dynamic> GetAllEntities(IBusinessEntityGetAllContext context)
        {
            var cachedEntities = GetCachedGenericBusinessEntities(context.EntityDefinitionId);
            if (cachedEntities == null)
                return null;
            else
                return cachedEntities.Values.Select(itm => itm as dynamic).ToList();
        }

        public bool IsCacheExpired(IBusinessEntityIsCacheExpiredContext context, ref DateTime? lastCheckTime)
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().IsCacheExpired(context.EntityDefinitionId, ref lastCheckTime);
        }
    }
    public class GenericBusinessEntityFilter
    {
        public int BusinessEntityDefinitionId { get; set; }
    }
    public class GenericBusinessEntityTitle
    {
        public string Title { get; set; }
        public string EntityName { get; set; }
    }
}
 