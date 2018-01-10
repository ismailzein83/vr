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
using Vanrise.Security.Business;
using Vanrise.Security.Entities;
using Vanrise.Common.Business;

namespace Vanrise.GenericData.Business
{
    public class GenericBusinessEntityManager : BaseBEManager
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

        //#region Public Methods Old


        //public Vanrise.Entities.IDataRetrievalResult<GenericBusinessEntityDetail> GetFilteredGenericBusinessEntities(Vanrise.Entities.DataRetrievalInput<GenericBusinessEntityQuery> input)
        //{
        //    var cachedGenericBusinessEntities = GetCachedGenericBusinessEntities(input.Query.BusinessEntityDefinitionId);
        //    IEnumerable<GenericFilterRuntimeField> runtimeFilters = GetGenericFilterRuntimeFields(input.Query.BusinessEntityDefinitionId);

        //    Func<GenericBusinessEntity, bool> filterExpression = (itm) => (input.Query.BusinessEntityDefinitionId == itm.BusinessEntityDefinitionId && (input.Query.FilterValuesByFieldPath == null || MatchGenericBusinessEntity(runtimeFilters, input.Query.FilterValuesByFieldPath, itm)));

        //    GenericBEDefinitionSettings definitionSettings = GetGenericBEDefinitionSettings(input.Query.BusinessEntityDefinitionId);

        //    var resultProcessingHandler = new ResultProcessingHandler<GenericBusinessEntityDetail>()
        //    {
        //        ExportExcelHandler = new GenericBusinessEntityExportExcelHandler(definitionSettings)
        //    };
        //    VRActionLogger.Current.LogGetFilteredAction(new GenericBusinessEntityLoggableEntity(input.Query.BusinessEntityDefinitionId), input);
        //    return DataRetrievalManager.Instance.ProcessResult(input, cachedGenericBusinessEntities.ToBigResult(input, filterExpression, GenericBusinessEntityDetailMapper), resultProcessingHandler);
        //}
        //public GenericBusinessEntity GetGenericBusinessEntity(long genericBusinessEntityId, Guid businessEntityDefinitionId, bool isViewedFromUI)
        //{
        //    var cachedGenericBusinessEntities = GetCachedGenericBusinessEntities(businessEntityDefinitionId);
        //    var genericBusinessEntity = cachedGenericBusinessEntities.GetRecord(genericBusinessEntityId);
        //    if (genericBusinessEntity != null && isViewedFromUI)
        //        VRActionLogger.Current.LogObjectViewed(new GenericBusinessEntityLoggableEntity(businessEntityDefinitionId), genericBusinessEntity);
        //    return genericBusinessEntity;
        //}
        //public GenericBusinessEntity GetGenericBusinessEntity(long genericBusinessEntityId, Guid businessEntityDefinitionId)
        //{
        //    return GetGenericBusinessEntity(genericBusinessEntityId,businessEntityDefinitionId,false);
        //}
        //public Vanrise.Entities.UpdateOperationOutput<GenericBusinessEntityDetail> UpdateGenericBusinessEntity(GenericBusinessEntity genericBusinessEntity)
        //{
        //    ConvertDetailsToDataRecord(genericBusinessEntity);
        //    var updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<GenericBusinessEntityDetail>();
        //    updateOperationOutput.UpdatedObject = null;

        //    if (IsEntityTitleValid(genericBusinessEntity))
        //    {
        //        updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;

        //        IGenericBusinessEntityDataManager dataManager = GenericDataDataManagerFactory.GetDataManager<IGenericBusinessEntityDataManager>();
        //        bool updated = dataManager.UpdateGenericBusinessEntity(genericBusinessEntity);

        //        if (updated)
        //        {
        //            VRActionLogger.Current.TrackAndLogObjectUpdated(new GenericBusinessEntityLoggableEntity(genericBusinessEntity.BusinessEntityDefinitionId), genericBusinessEntity);
        //            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
        //            CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired(genericBusinessEntity.BusinessEntityDefinitionId);
        //            updateOperationOutput.UpdatedObject = GenericBusinessEntityDetailMapper(genericBusinessEntity);
        //        }
        //    }
        //    else
        //    {
        //        updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
        //    }

        //    return updateOperationOutput;
        //}        
        //public Vanrise.Entities.InsertOperationOutput<GenericBusinessEntityDetail> AddGenericBusinessEntity(GenericBusinessEntity genericBusinessEntity)
        //{
        //    ConvertDetailsToDataRecord(genericBusinessEntity);
        //    var insertOperationOutput = new Vanrise.Entities.InsertOperationOutput<GenericBusinessEntityDetail>();
        //    insertOperationOutput.InsertedObject = null;
            
        //    if (IsEntityTitleValid(genericBusinessEntity))
        //    {
        //        insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
        //        long genericBusinessEntityId = -1;

        //        IGenericBusinessEntityDataManager dataManager = GenericDataDataManagerFactory.GetDataManager<IGenericBusinessEntityDataManager>();
        //        bool inserted = dataManager.AddGenericBusinessEntity(genericBusinessEntity, out genericBusinessEntityId);

        //        if (inserted)
        //        {
        //            CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired(genericBusinessEntity.BusinessEntityDefinitionId);
        //            genericBusinessEntity.GenericBusinessEntityId = genericBusinessEntityId;
        //            VRActionLogger.Current.TrackAndLogObjectAdded(new GenericBusinessEntityLoggableEntity(genericBusinessEntity.BusinessEntityDefinitionId), genericBusinessEntity);
        //            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
        //            insertOperationOutput.InsertedObject = GenericBusinessEntityDetailMapper(genericBusinessEntity);
                   
        //        }
        //    }
        //    else
        //    {
        //        insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.SameExists;
        //    }

        //    return insertOperationOutput;
        //}
        //public Vanrise.Entities.DeleteOperationOutput<object> DeleteGenericBusinessEntity(long genericBusinessEntityId, Guid businessEntityDefinitionId)
        //{
        //    var deleteOperationOutput = new Vanrise.Entities.DeleteOperationOutput<object>()
        //    {
        //        Result = Vanrise.Entities.DeleteOperationResult.Failed
        //    };

        //    IGenericBusinessEntityDataManager dataManager = GenericDataDataManagerFactory.GetDataManager<IGenericBusinessEntityDataManager>();
        //    bool deleted = dataManager.DeleteGenericBusinessEntity(genericBusinessEntityId, businessEntityDefinitionId);

        //    if (deleted)
        //    {
        //        deleteOperationOutput.Result = Vanrise.Entities.DeleteOperationResult.Succeeded;
        //        CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired(businessEntityDefinitionId);
        //    }

        //    return deleteOperationOutput;
        //}
        //public IEnumerable<GenericBusinessEntity> GetGenericBusinessEntities(Guid businessEntityDefinitionId)
        //{
        //    var cachedGenericBusinessEntities = GetCachedGenericBusinessEntities(businessEntityDefinitionId);
        //    return cachedGenericBusinessEntities.Values;
        //}
        //public IEnumerable<GenericBusinessEntityInfo> GetGenericBusinessEntityInfo(Guid businessEntityDefinitionId, GenericBusinessEntityFilter filter)
        //{
        //    var cachedGenericBusinessEntities = GetCachedGenericBusinessEntities(businessEntityDefinitionId);
        //    if (filter != null)
        //    {
                
        //    }
        //    return cachedGenericBusinessEntities.MapRecords(GenericBusinessEntityInfoMapper);
        //}
        //public dynamic GetFieldPathValue(GenericBusinessEntity entity, string fieldPath)
        //{
        //    if (entity.Details == null)
        //        return null;
        //    Type entityType = entity.Details.GetType();
        //    var field = entityType.GetProperty(fieldPath);
        //    if (field == null)
        //        throw new NullReferenceException(String.Format("field {0}", fieldPath));
        //    return field.GetValue(entity.Details);
        //    //return entity.Details[fieldPath];
        //    //var obj = entityType.GetProperties();
        //    //var ds = obj.First(x => x.Name == fieldPath).GetValue(entity.Details);
        //    //return ds;
        //    //var entityField = entityType.GetField(fieldPath, BindingFlags.NonPublic | BindingFlags.Instance);
        //    //if (entityField == null)
        //    //    return null;
        //    //return entityField.GetValue(entity.Details);
        //}

        //public GenericBusinessEntityTitle GetBusinessEntityTitle(Guid businessEntityDefinitionId, long? genericBussinessEntityId = null)
        //{
           
        //    var businessEntityDefinition = GetBusinessEntityDefinition(businessEntityDefinitionId);
          
        //    GenericBusinessEntityTitle entityTitle = new GenericBusinessEntityTitle();
        //    entityTitle.Title = businessEntityDefinition.Title;
           
        //    if(genericBussinessEntityId !=null)
        //    {
        //        GenericBEDefinitionSettings definitionSettings = businessEntityDefinition.Settings as GenericBEDefinitionSettings;
        //        if (definitionSettings == null)
        //            throw new NullReferenceException("definitionSettings");          
               
        //        var genericBusinessEntity = GetGenericBusinessEntity((long)genericBussinessEntityId, businessEntityDefinitionId);
        //        entityTitle.EntityName = GetFieldDescription(genericBusinessEntity, definitionSettings.FieldPath, definitionSettings.DataRecordTypeId);
        //    }
        //    return entityTitle;
        //}

        //public Guid GetDataRecordTypeId(Guid businessEntityDefinitionId)
        //{
        //    BusinessEntityDefinition beDefinition = new BusinessEntityDefinitionManager().GetBusinessEntityDefinition(businessEntityDefinitionId);
        //    if (beDefinition == null)
        //        throw new NullReferenceException("beDefinition");
        //    if (beDefinition.Settings == null)
        //        throw new NullReferenceException("beDefinition.Settings");

        //    GenericBEDefinitionSettings gbeDefinitionSettings = beDefinition.Settings as GenericBEDefinitionSettings;
        //    if (gbeDefinitionSettings == null)
        //        throw new NullReferenceException("gbeDefinitionSettings");

        //    return gbeDefinitionSettings.DataRecordTypeId;
        //}

        //public bool DoesUserHaveViewAccess(Guid genericBEDefinitionId)
        //{
        //    int userId = SecurityContext.Current.GetLoggedInUserId();
        //    return DoesUserHaveViewAccess(userId, genericBEDefinitionId);
        //}
        //public bool DoesUserHaveViewAccess(int userId, Guid genericBEDefinitionId)
        //{
        //    var beDefinition = GetBusinessEntityDefinition(genericBEDefinitionId);
        //    if (beDefinition != null && beDefinition.Settings != null)
        //    {
        //        var settings = beDefinition.Settings as GenericBEDefinitionSettings;
        //        if (settings != null && settings.Security != null && settings.Security.ViewRequiredPermission != null)
        //            return DoesUserHaveAccess(userId, settings.Security.ViewRequiredPermission);
        //    }
        //    return true;
        //}
        //public bool DoesUserHaveAddAccess(Guid genericBEDefinitionId)
        //{
        //    var beDefinition = GetBusinessEntityDefinition(genericBEDefinitionId);

        //    if (beDefinition != null && beDefinition.Settings != null)
        //    {
        //        var settings = beDefinition.Settings as GenericBEDefinitionSettings;
        //        if (settings != null && settings.Security != null && settings.Security.AddRequiredPermission != null)
        //            return DoesUserHaveAccess(settings.Security.AddRequiredPermission);
        //    }
        //    return true;
        //}

       
        //public bool DoesUserHaveEditAccess(Guid genericBEDefinitionId)
        //{
        //    var beDefinition = GetBusinessEntityDefinition(genericBEDefinitionId);
        //    if (beDefinition != null && beDefinition.Settings != null)
        //    {
        //        var settings = beDefinition.Settings as GenericBEDefinitionSettings;
        //        if (settings != null && settings.Security != null && settings.Security.EditRequiredPermission != null)
        //            return DoesUserHaveAccess(settings.Security.EditRequiredPermission);
        //    }
        //    return true;
        //}
        //#endregion

        #region Public Methods
        public IDataRetrievalResult<GenericBusinessEntityDetail> GetFilteredGenericBusinessEntities(DataRetrievalInput<GenericBusinessEntityQuery> input)
        {
            Func<GenericBusinessEntity, bool> filterExpression = (genericBusinessEntity) =>
            {

                return true;
            };
            GenericBusinessEntityDefinitionManager genericBusinessEntityDefinitionManager = new GenericBusinessEntityDefinitionManager();
            var genericBEDefinitionSetting = genericBusinessEntityDefinitionManager.GetGenericBEDefinitionSettings(input.Query.BusinessEntityDefinitionId);
            genericBEDefinitionSetting.ThrowIfNull("genericBEDefinitionSetting", input.Query.BusinessEntityDefinitionId);
            DataRecordStorageManager dataRecordStorageManager = new DataRecordStorageManager();

            List<string> columns = new List<string>();
            List<string> columnTitles = new List<string>();
            genericBEDefinitionSetting.GridDefinition.ThrowIfNull("genericBEDefinitionSetting.GridDefinition");
            genericBEDefinitionSetting.GridDefinition.ColumnDefinitions.ThrowIfNull("genericBEDefinitionSetting.GridDefinition.ColumnDefinitions");
            foreach (var gridColumn in genericBEDefinitionSetting.GridDefinition.ColumnDefinitions)
            {
                columns.Add(gridColumn.FieldName);
                columnTitles.Add(gridColumn.FieldTitle);
            }
            string sortByColumnName = input.SortByColumnName;

            var storageRecords = dataRecordStorageManager.GetFilteredDataRecords(new DataRetrievalInput<DataRecordQuery>
            {
                FromRow = input.FromRow,
                ToRow = input.ToRow,
                SortByColumnName = sortByColumnName,
                IsAPICall = input.IsAPICall,
                IsSortDescending = input.IsSortDescending,
                ResultKey = input.ResultKey,
                GetSummary = input.GetSummary,
                DataRetrievalResultType = input.DataRetrievalResultType,
                Query = new DataRecordQuery
                {
                    Columns = columns,
                    //FilterGroup=,
                    //Filters=,
                    FromTime = DateTime.MinValue,
                    ColumnTitles = columnTitles,
                    DataRecordStorageIds = new List<Guid> { genericBEDefinitionSetting.DataRecordStorageId },
                    //Direction=,
                    LimitResult = 1000,
                    //SortColumns=,
                    //ToTime
                },
            }) as BigResult<DataRecordDetail>;

            BigResult<GenericBusinessEntityDetail> result = new BigResult<GenericBusinessEntityDetail>();
            if (storageRecords != null)
            {
                result.ResultKey = storageRecords.ResultKey;
                result.TotalCount = storageRecords.TotalCount;
                List<GenericBusinessEntityDetail> resultDetail = new List<GenericBusinessEntityDetail>();
                if (storageRecords.Data != null)
                {
                    foreach (var storageRecord in storageRecords.Data)
                    {
                        GenericBusinessEntityDetail genericBusinessEntityDetail = new GenericBusinessEntityDetail();
                        if (storageRecord.FieldValues != null)
                        {
                            genericBusinessEntityDetail.FieldValues = new GenericBusinessEntityValues();
                            foreach (var fieldValue in storageRecord.FieldValues)
                            {
                                genericBusinessEntityDetail.FieldValues.Add(fieldValue.Key, new GenericBusinessEntityValue
                                {
                                    Value = fieldValue.Value.Value,
                                    Description = fieldValue.Value.Description,
                                });
                            }
                        }
                        resultDetail.Add(genericBusinessEntityDetail);
                    }
                }

                result.Data = resultDetail;
            }
            //   var vrCases =  storageDataManager.GetFilteredDataRecords()
            //  return DataRetrievalManager.Instance.ProcessResult(input, vrCases.ToBigResult(input, filterExpression, VRCaseDetailMapper));
            return result;
        }
        public InsertOperationOutput<GenericBusinessEntityDetail> AddGenericBusinessEntity(GenericBusinessEntity genericBusinessEntity)
        {
            InsertOperationOutput<GenericBusinessEntityDetail> insertOperationOutput = new Vanrise.Entities.InsertOperationOutput<GenericBusinessEntityDetail>();
            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;
            Object insertedId;
            GenericBusinessEntityDefinitionManager genericBusinessEntityDefinitionManager = new GenericBusinessEntityDefinitionManager();
            var caseDefinitionSetting = genericBusinessEntityDefinitionManager.GetGenericBEDefinitionSettings(genericBusinessEntity.BusinessEntityDefinitionId);
            var storageDataManager = new DataRecordStorageManager().GetStorageDataManager(caseDefinitionSetting.DataRecordStorageId);

            bool insertActionSucc = storageDataManager.Insert(genericBusinessEntity.FieldValues, out insertedId);
            if (insertActionSucc)
            {
                genericBusinessEntity.GenericBusinessEntityId = (long)insertedId;
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                insertOperationOutput.InsertedObject = GenericBusinessEntityDetailMapper(genericBusinessEntity);
            }
            else
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.SameExists;
            return insertOperationOutput;
        }
        public UpdateOperationOutput<GenericBusinessEntityDetail> UpdateGenericBusinessEntity(GenericBusinessEntity genericBusinessEntity)
        {
            UpdateOperationOutput<GenericBusinessEntityDetail> updateOperationOutput = new UpdateOperationOutput<GenericBusinessEntityDetail>();
            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            GenericBusinessEntityDefinitionManager genericBusinessEntityDefinitionManager = new GenericBusinessEntityDefinitionManager();
            var caseDefinitionSetting = genericBusinessEntityDefinitionManager.GetGenericBEDefinitionSettings(genericBusinessEntity.BusinessEntityDefinitionId);
            var storageDataManager = new DataRecordStorageManager().GetStorageDataManager(caseDefinitionSetting.DataRecordStorageId);

            bool updateActionSucc = storageDataManager.Update(genericBusinessEntity.FieldValues);
            if (updateActionSucc)
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = GenericBusinessEntityDetailMapper(genericBusinessEntity);
            }
            else
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
            }

            return updateOperationOutput;
        }
        #endregion


        #region Private Methods

        private bool DoesUserHaveAccess(int userId, RequiredPermissionSettings requiredPermission)
        {
            SecurityManager secManager = new SecurityManager();
            if (!secManager.IsAllowed(requiredPermission, userId))
                return false;
            return true;

        }
        private bool DoesUserHaveAccess(RequiredPermissionSettings requiredPermission)
        {
            int userId = SecurityContext.Current.GetLoggedInUserId();
            SecurityManager secManager = new SecurityManager();
            if (!secManager.IsAllowed(requiredPermission, userId))
                return false;
            return true;

        }
        private Dictionary<string, DataRecordField> GetDataRecordTypeFields(Guid dataRecordTypeId)
        {
            var fields = _recordTypeManager.GetDataRecordTypeFields(dataRecordTypeId);
            if (fields == null)
                throw new NullReferenceException(String.Format("fields. dataRecordTypeId '{0}'", dataRecordTypeId));
            return fields;
        }
        //private string GetFieldDescription(GenericBusinessEntity genericBusinessEntity, string fieldPath, Guid dataRecordTypeId)
        //{
        //    var recordTypeFields = GetDataRecordTypeFields(dataRecordTypeId);
        //    var columnValue = GetFieldPathValue(genericBusinessEntity, fieldPath);
        //    if (columnValue != null)
        //    {
        //        var uiRuntimeField = _uiRuntimeManager.GetFieldType(fieldPath, recordTypeFields, dataRecordTypeId);
        //        return uiRuntimeField.GetDescription(columnValue);
        //    }
        //    return null;
        //}
        //private string GetFieldDescription<T>(GenericUIField field, GenericBusinessEntity genericBusinessEntity, string fieldPath, Guid dataRecordTypeId) where T : GenericUIRuntimeField
        //{
        //    var recordTypeFields = GetDataRecordTypeFields(dataRecordTypeId);
        //    var columnValue = GetFieldPathValue(genericBusinessEntity, fieldPath);
        //    if (columnValue != null)
        //    {
        //        var uiRuntimeField = _uiRuntimeManager.BuildRuntimeField<T>(field, recordTypeFields, dataRecordTypeId);
        //        //var value = (columnValue.Value != null) ? columnValue.Value : columnValue;
        //        return uiRuntimeField.FieldType.GetDescription(columnValue);
        //    }
        //    return null;
        //}
        //public Dictionary<long, GenericBusinessEntity> GetCachedGenericBusinessEntities(Guid businessDefinitionId)
        //{
        //    return CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject(String.Format("GetGenericBusinessEntities_{0}", businessDefinitionId), businessDefinitionId,
        //       () =>
        //       {
        //           IGenericBusinessEntityDataManager dataManager = GenericDataDataManagerFactory.GetDataManager<IGenericBusinessEntityDataManager>();
        //           IEnumerable<GenericBusinessEntity> genericBusinessEntities = dataManager.GetGenericBusinessEntitiesByDefinition(businessDefinitionId);
        //           return genericBusinessEntities.ToDictionary(kvp => kvp.GenericBusinessEntityId, kvp => kvp);
        //       });
        //}
        //IEnumerable<GenericFilterRuntimeField> GetGenericFilterRuntimeFields(Guid businessEntityDefinitionId)
        //{
        //    GenericManagementRuntime managementRuntime = new GenericUIRuntimeManager().GetManagementRuntime(businessEntityDefinitionId);
        //    if (managementRuntime == null)
        //        throw new NullReferenceException("managementRuntime");
        //    if (managementRuntime.Filter == null)
        //        throw new NullReferenceException("managementRuntime.Filter");
        //    if (managementRuntime.Filter.Fields == null || managementRuntime.Filter.Fields.Count == 0)
        //        throw new NullReferenceException("managementRuntime.Filter.Fields");
        //    return managementRuntime.Filter.Fields;
        //}
        //bool MatchGenericBusinessEntity(IEnumerable<GenericFilterRuntimeField> runtimeFilters, Dictionary<string, object> filterValuesByFieldPath, GenericBusinessEntity genericBusinessEntity)
        //{
        //    foreach (var runtimeFilter in runtimeFilters)
        //    {
        //        object filterValue;
        //        filterValuesByFieldPath.TryGetValue(runtimeFilter.FieldPath, out filterValue);

        //        if (filterValue == null)
        //            continue;

        //        dynamic fieldValue = GetFieldPathValue(genericBusinessEntity, runtimeFilter.FieldPath);
        //        if (fieldValue == null)
        //            continue;

        //        if (!runtimeFilter.FieldType.IsMatched(fieldValue, filterValue))
        //            return false;
        //    }

        //    return true;
        //}
        //bool IsEntityTitleValid(GenericBusinessEntity entity)
        //{
        //    BusinessEntityDefinition beDefinition = new BusinessEntityDefinitionManager().GetBusinessEntityDefinition(entity.BusinessEntityDefinitionId);
        //    if (beDefinition == null)
        //        throw new NullReferenceException("beDefinition");

        //    var genericBEDefinitionSettings = beDefinition.Settings as GenericBEDefinitionSettings;
        //    if (genericBEDefinitionSettings == null)
        //        throw new NullReferenceException("genericBEDefinitionSettings");

        //    string fieldPath = genericBEDefinitionSettings.FieldPath;
        //    if (fieldPath == null)
        //        throw new NullReferenceException("genericBEDefinitionSettings.FieldPath");

        //    dynamic entityTitle = GetFieldPathValue(entity, fieldPath);
        //    if (entityTitle == null)
        //        throw new NullReferenceException("entityTitle");

        //    string entityTitleString = entityTitle.ToString().ToLower();

        //    Dictionary<long, GenericBusinessEntity> cachedEntites = GetCachedGenericBusinessEntities(entity.BusinessEntityDefinitionId);
        //    GenericBusinessEntity entityMatch = cachedEntites.FindRecord(itm => (entity.GenericBusinessEntityId == 0 || itm.GenericBusinessEntityId != entity.GenericBusinessEntityId) && GetFieldPathValue(itm, fieldPath).ToString().ToLower() == entityTitleString);

        //    return (entityMatch == null);
        //}
        //void ConvertDetailsToDataRecord(GenericBusinessEntity genericBusinessEntity)
        //{
        //    if (genericBusinessEntity == null || genericBusinessEntity.Details == null)
        //        return;
        //    Guid dataRecordTypeId = GetDataRecordTypeId(genericBusinessEntity.BusinessEntityDefinitionId);
        //    genericBusinessEntity.Details = _recordTypeManager.ConvertDynamicToDataRecord(genericBusinessEntity.Details, dataRecordTypeId);
        //}
        #endregion

        #region Private Classes Old


        //public class CacheManager : Vanrise.Caching.BaseCacheManager<Guid>
        //{
        //    IGenericBusinessEntityDataManager _dataManager = GenericDataDataManagerFactory.GetDataManager<IGenericBusinessEntityDataManager>();
        //    ConcurrentDictionary<Guid, Object> _updateHandlesByRuleType = new ConcurrentDictionary<Guid, Object>();

        //    protected override bool ShouldSetCacheExpired(Guid parameter)
        //    {
        //        Object updateHandle;
        //        _updateHandlesByRuleType.TryGetValue(parameter, out updateHandle);
        //        bool isCacheExpired = _dataManager.AreGenericBusinessEntityUpdated(parameter, ref updateHandle);
        //        _updateHandlesByRuleType.AddOrUpdate(parameter, updateHandle, (key, existingHandle) => updateHandle);
        //        return isCacheExpired;
        //    }
        //}

        //public class GenericBusinessEntityLoggableEntity : VRLoggableEntityBase
        //{
        //    Guid _businessEntityDefinitionId;
        //    static BusinessEntityDefinitionManager s_BusinessEntityDefinitionManager = new BusinessEntityDefinitionManager();
        //    static GenericBusinessEntityManager s_genericBusinessEntityManager = new GenericBusinessEntityManager();
        //    public GenericBusinessEntityLoggableEntity(Guid businessEntityDefinitionId)
        //    {
        //        _businessEntityDefinitionId = businessEntityDefinitionId;
        //    }

        //    public override string EntityUniqueName
        //    {
        //        get { return String.Format("VR_GenericData_GenericBusinessEntity_{0}", _businessEntityDefinitionId); }
        //    }

        //    public override string EntityDisplayName
        //    {
        //        get { return s_BusinessEntityDefinitionManager.GetBusinessEntityDefinitionName(_businessEntityDefinitionId); }
        //    }

        //    public override string ViewHistoryItemClientActionName
        //    {
        //        get { return "VR_GenericData_GenericBusinessEntity_ViewHistoryItem"; }
        //    }


        //    public override object GetObjectId(IVRLoggableEntityGetObjectIdContext context)
        //    {
        //        GenericBusinessEntity genericBusinessEntity = context.Object.CastWithValidate<GenericBusinessEntity>("context.Object");
        //        return genericBusinessEntity.GenericBusinessEntityId;
        //    }

        //    public override string GetObjectName(IVRLoggableEntityGetObjectNameContext context)
        //    {

        //        GenericBusinessEntity genericBusinessEntity = context.Object.CastWithValidate<GenericBusinessEntity>("context.Object");
        //        var businessEntityTitle = s_genericBusinessEntityManager.GetBusinessEntityTitle( genericBusinessEntity.BusinessEntityDefinitionId,genericBusinessEntity.GenericBusinessEntityId);
        //        return (businessEntityTitle != null) ? businessEntityTitle.EntityName : null;
        //    }

        //    public override string ModuleName
        //    {
        //        get { return "Generic Data"; }
        //    }
        //}
        #endregion

        #region Mappers Old
         
        //private GenericBusinessEntityDetail GenericBusinessEntityDetailMapper(GenericBusinessEntity genericBusinessEntity)
        //{
        //    GenericBEDefinitionSettings definitionSettings = GetGenericBEDefinitionSettings(genericBusinessEntity.BusinessEntityDefinitionId);
        //    GenericBusinessEntityDetail entityDetail = new GenericBusinessEntityDetail();
        //    entityDetail.Entity = genericBusinessEntity;
        //    entityDetail.FieldValueDescriptions = new List<string>();
        //    foreach (var columnConfig in definitionSettings.ManagementDesign.GridDesign.Columns)
        //    {
        //        entityDetail.FieldValueDescriptions.Add(GetFieldDescription<GenericEditorRuntimeField>(columnConfig, genericBusinessEntity, columnConfig.FieldPath, definitionSettings.DataRecordTypeId));
        //    }
        //    return entityDetail;
        //}
        //private GenericBusinessEntityInfo GenericBusinessEntityInfoMapper(GenericBusinessEntity genericBusinessEntity)
        //{
        //    GenericBEDefinitionSettings definitionSettings = GetGenericBEDefinitionSettings(genericBusinessEntity.BusinessEntityDefinitionId);
        //    GenericBusinessEntityInfo entityInfo = new GenericBusinessEntityInfo();
        //    entityInfo.GenericBusinessEntityId = genericBusinessEntity.GenericBusinessEntityId;
        //    entityInfo.Name = GetFieldDescription(genericBusinessEntity, definitionSettings.FieldPath, definitionSettings.DataRecordTypeId);
        //    return entityInfo;
        //}
       
        #endregion

        #region Mappers
        private GenericBusinessEntityDetail GenericBusinessEntityDetailMapper(GenericBusinessEntity genericBusinessEntity)
        {
            GenericBusinessEntityDetail genericBusinessEntityDetail = new GenericBusinessEntityDetail();
            genericBusinessEntity.GenericBusinessEntityId = genericBusinessEntity.GenericBusinessEntityId;
            genericBusinessEntityDetail.FieldValues = new GenericBusinessEntityValues();
            foreach (var fieldValue in genericBusinessEntity.FieldValues)
            {
                genericBusinessEntityDetail.FieldValues.Add(fieldValue.Key, new GenericBusinessEntityValue
                {
                    Value = fieldValue.Value
                });
            }
            return genericBusinessEntityDetail;
        }

        #endregion

    }
    //public class GenericBusinessEntityFilter
    //{
    //    public int BusinessEntityDefinitionId { get; set; }
    //}
    //public class GenericBusinessEntityTitle
    //{
    //    public string Title { get; set; }
    //    public string EntityName { get; set; }
    //}
}
 