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
        GenericBusinessEntityDefinitionManager _genericBEDefinitionManager;
        DataRecordStorageManager _dataRecordStorageManager;
        public GenericBusinessEntityManager()
        {
            _genericBEDefinitionManager = new GenericBusinessEntityDefinitionManager();
            _dataRecordStorageManager = new DataRecordStorageManager();
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
      
        //public Vanrise.Entities.UpdateOperationOutput<GenericBusinessEntityDetail> UpdateGenericBusinessEntity(GenericBusinessEntity genericBusinessEntity)
        //{

        //    if (IsEntityTitleValid(genericBusinessEntity))
        //    {
        //        if (updated)
        //        {
        //            VRActionLogger.Current.TrackAndLogObjectUpdated(new GenericBusinessEntityLoggableEntity(genericBusinessEntity.BusinessEntityDefinitionId), genericBusinessEntity);
        //        }
        //    }
        //}        
        //public Vanrise.Entities.InsertOperationOutput<GenericBusinessEntityDetail> AddGenericBusinessEntity(GenericBusinessEntity genericBusinessEntity)
        //{            
        //    if (IsEntityTitleValid(genericBusinessEntity))
        //    {
        //            CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired(genericBusinessEntity.BusinessEntityDefinitionId);
        //            VRActionLogger.Current.TrackAndLogObjectAdded(new GenericBusinessEntityLoggableEntity(genericBusinessEntity.BusinessEntityDefinitionId), genericBusinessEntity);
            
        //}
     
        //public IEnumerable<GenericBusinessEntityInfo> GetGenericBusinessEntityInfo(Guid businessEntityDefinitionId, GenericBusinessEntityFilter filter)
        //{
        //    var cachedGenericBusinessEntities = GetCachedGenericBusinessEntities(businessEntityDefinitionId);
        //    if (filter != null)
        //    {
                
        //    }
        //    return cachedGenericBusinessEntities.MapRecords(GenericBusinessEntityInfoMapper);
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

        public GenericBusinessEntityRuntimeEditor GetGenericBusinessEntityEditorRuntime(Guid businessEntityDefinitionId, long? genericBusinessEntityId)
        {
            return new GenericBusinessEntityRuntimeEditor
            {
                GenericBusinessEntity = genericBusinessEntityId.HasValue ? GetGenericBusinessEntity(genericBusinessEntityId.Value, businessEntityDefinitionId) : null,
                DefinitionTitle = _genericBEDefinitionManager.GetGenericBEDefinitionTitle(businessEntityDefinitionId),
                GenericBEDefinitionSettings = _genericBEDefinitionManager.GetGenericBEDefinitionSettings(businessEntityDefinitionId),
                TitleFieldName = _genericBEDefinitionManager.GetGenericBEDefinitionTitleFieldName(businessEntityDefinitionId)
            };
        }
        public GenericBusinessEntity GetGenericBusinessEntity(long genericBusinessEntityId, Guid businessEntityDefinitionId)
        {
            var genericBEDefinitionSetting = _genericBEDefinitionManager.GetGenericBEDefinitionSettings(businessEntityDefinitionId);
            var idDataRecordField = _genericBEDefinitionManager.GetIdFieldTypeForGenericBE(businessEntityDefinitionId);
           
            List<string> columns = new List<string>();
            List<string> columnTitles = new List<string>();
            var dataRecordFields = _genericBEDefinitionManager.GetDataRecordTypeFieldsByBEDefinitionId(businessEntityDefinitionId);
            foreach (var field in dataRecordFields)
            {
                columns.Add(field.Key);
                columnTitles.Add(field.Value.Title);
            }

          //  GetGridColumnNamesAndTitles(businessEntityDefinitionId, out columns, out columnTitles);
            var storageRecords = _dataRecordStorageManager.GetFilteredDataRecords(new DataRetrievalInput<DataRecordQuery>
            {
                FromRow = 0,
                ToRow = 2,
                SortByColumnName = string.Format("FieldValues.{0}.Description", idDataRecordField.Name),
                Query = new DataRecordQuery
                {
                    Columns = columns,
                    Filters = new List<DataRecordFilter>
                    {
                        new DataRecordFilter {
                            FieldName = idDataRecordField.Name,
                            FilterValues =  new List<object> {
                                genericBusinessEntityId
                            }
                        }
                    },
                    FromTime = DateTime.MinValue,
                    ColumnTitles = columnTitles,
                    DataRecordStorageIds = new List<Guid> { genericBEDefinitionSetting.DataRecordStorageId },
                    //Direction=,
                    LimitResult = 1000,
                    //SortColumns=,
                    //ToTime
                },
            }) as BigResult<DataRecordDetail>;
            if (storageRecords.Data != null)
            {
                var item = storageRecords.Data.FirstOrDefault();
                if (item != null && item.FieldValues != null)
                {
                    Dictionary<string, Object> fieldValues = new Dictionary<string, Object>();
                    foreach (var fieldValue in item.FieldValues)
                    {
                        fieldValues.Add(fieldValue.Key, fieldValue.Value.Value);
                    }
                    return new GenericBusinessEntity
                    {
                        FieldValues = fieldValues,
                    };
                }
               
            }
            return null;
        }
        public IDataRetrievalResult<GenericBusinessEntityDetail> GetFilteredGenericBusinessEntities(DataRetrievalInput<GenericBusinessEntityQuery> input)
        {
            Func<GenericBusinessEntity, bool> filterExpression = (genericBusinessEntity) =>
            {

                return true;
            };
            var genericBEDefinitionSetting = _genericBEDefinitionManager.GetGenericBEDefinitionSettings(input.Query.BusinessEntityDefinitionId);
            genericBEDefinitionSetting.ThrowIfNull("genericBEDefinitionSetting", input.Query.BusinessEntityDefinitionId);
            List<string> columns = new List<string>();
            List<string> columnTitles = new List<string>();
            GetGridColumnNamesAndTitles(input.Query.BusinessEntityDefinitionId, out columns, out columnTitles);
            string sortByColumnName = input.SortByColumnName;

            var storageRecords = _dataRecordStorageManager.GetFilteredDataRecords(new DataRetrievalInput<DataRecordQuery>
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
                        if (genericBEDefinitionSetting.GridDefinition.GenericBEGridActions != null)
                        {
                            genericBusinessEntityDetail.AvailableGridActionIds = new List<Guid>();
                            foreach (var genericBEGridAction in genericBEDefinitionSetting.GridDefinition.GenericBEGridActions)
                            {
                                genericBusinessEntityDetail.AvailableGridActionIds.Add(genericBEGridAction.GenericBEGridActionId);
                            }
                        }
                        if (genericBEDefinitionSetting.GridDefinition.GenericBEGridViews != null)
                        {
                            genericBusinessEntityDetail.AvailableGridViewIds = new List<Guid>();
                            foreach (var genericBEGridView in genericBEDefinitionSetting.GridDefinition.GenericBEGridViews)
                            {
                                genericBusinessEntityDetail.AvailableGridViewIds.Add(genericBEGridView.GenericBEViewDefinitionId);
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
        public InsertOperationOutput<GenericBusinessEntityDetail> AddGenericBusinessEntity(GenericBusinessEntityToAdd genericBusinessEntityToAdd)
        {
            InsertOperationOutput<GenericBusinessEntityDetail> insertOperationOutput = new Vanrise.Entities.InsertOperationOutput<GenericBusinessEntityDetail>();
            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;
            Object insertedId;
            var caseDefinitionSetting = _genericBEDefinitionManager.GetGenericBEDefinitionSettings(genericBusinessEntityToAdd.BusinessEntityDefinitionId);
            var storageDataManager = _dataRecordStorageManager.GetStorageDataManager(caseDefinitionSetting.DataRecordStorageId);


            bool insertActionSucc = storageDataManager.Insert(genericBusinessEntityToAdd.FieldValues, out insertedId);
            if (insertActionSucc)
            {
                if (genericBusinessEntityToAdd.FieldValues != null)
                {
                    var idFieldType = _genericBEDefinitionManager.GetIdFieldTypeForGenericBE(genericBusinessEntityToAdd.BusinessEntityDefinitionId);
                    genericBusinessEntityToAdd.FieldValues.Add(idFieldType.Name, (long)insertedId);
                }
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                insertOperationOutput.InsertedObject = GenericBusinessEntityDetailMapper(genericBusinessEntityToAdd.BusinessEntityDefinitionId, genericBusinessEntityToAdd);
            }
            else
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.SameExists;
            return insertOperationOutput;
        }
        public UpdateOperationOutput<GenericBusinessEntityDetail> UpdateGenericBusinessEntity(GenericBusinessEntityToUpdate genericBusinessEntityToUpdate)
        {
            UpdateOperationOutput<GenericBusinessEntityDetail> updateOperationOutput = new UpdateOperationOutput<GenericBusinessEntityDetail>();
            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            var caseDefinitionSetting = _genericBEDefinitionManager.GetGenericBEDefinitionSettings(genericBusinessEntityToUpdate.BusinessEntityDefinitionId);
            var storageDataManager = _dataRecordStorageManager.GetStorageDataManager(caseDefinitionSetting.DataRecordStorageId);
            if (genericBusinessEntityToUpdate.FieldValues != null)
            {
                var idFieldType = _genericBEDefinitionManager.GetIdFieldTypeForGenericBE(genericBusinessEntityToUpdate.BusinessEntityDefinitionId);
                genericBusinessEntityToUpdate.FieldValues.Add(idFieldType.Name, genericBusinessEntityToUpdate.GenericBusinessEntityId);
            }
            bool updateActionSucc = storageDataManager.Update(genericBusinessEntityToUpdate.FieldValues);
            if (updateActionSucc)
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = GenericBusinessEntityDetailMapper(genericBusinessEntityToUpdate.BusinessEntityDefinitionId, genericBusinessEntityToUpdate); 
            }
            else
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
            }

            return updateOperationOutput;
        }
      
        #endregion


        #region Private Methods

        private void GetGridColumnNamesAndTitles(Guid businessEntityDefinitionId, out List<string> columnNames, out List<string> columnTitles)
        {
            var genericBEDefinitionSetting = _genericBEDefinitionManager.GetGenericBEDefinitionSettings(businessEntityDefinitionId);
            columnNames = new List<string>();
            columnTitles = new List<string>();
            genericBEDefinitionSetting.GridDefinition.ThrowIfNull("genericBEDefinitionSetting.GridDefinition");
            genericBEDefinitionSetting.GridDefinition.ColumnDefinitions.ThrowIfNull("genericBEDefinitionSetting.GridDefinition.ColumnDefinitions");
            foreach (var gridColumn in genericBEDefinitionSetting.GridDefinition.ColumnDefinitions)
            {
                columnNames.Add(gridColumn.FieldName);
                columnTitles.Add(gridColumn.FieldTitle);
            }
        }

        //private bool DoesUserHaveAccess(int userId, RequiredPermissionSettings requiredPermission)
        //{
        //    SecurityManager secManager = new SecurityManager();
        //    if (!secManager.IsAllowed(requiredPermission, userId))
        //        return false;
        //    return true;

        //}
        //private bool DoesUserHaveAccess(RequiredPermissionSettings requiredPermission)
        //{
        //    int userId = SecurityContext.Current.GetLoggedInUserId();
        //    SecurityManager secManager = new SecurityManager();
        //    if (!secManager.IsAllowed(requiredPermission, userId))
        //        return false;
        //    return true;

        //}
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
        private GenericBusinessEntityDetail GenericBusinessEntityDetailMapper(Guid businessEntityDefinitionId, GenericBusinessEntity genericBusinessEntity)
        {
            GenericBusinessEntityDetail genericBusinessEntityDetail = new GenericBusinessEntityDetail();
            genericBusinessEntityDetail.FieldValues = new GenericBusinessEntityValues();
            var dataRecordTypeFields = _genericBEDefinitionManager.GetDataRecordTypeFieldsByBEDefinitionId(businessEntityDefinitionId);

            foreach (var fieldValue in genericBusinessEntity.FieldValues)
            {
                var dataRecordTypeField = dataRecordTypeFields.FindRecord(x => x.Name == fieldValue.Key);
                if (dataRecordTypeField != null)
                {
                    genericBusinessEntityDetail.FieldValues.Add(fieldValue.Key, new GenericBusinessEntityValue
                    {
                        Value = fieldValue.Value,
                        Description = dataRecordTypeField.Type.GetDescription(fieldValue.Value)
                    });
                }
            }

            var gridDefinition = _genericBEDefinitionManager.GetGenericBEDefinitionGridDefinition(businessEntityDefinitionId);
            if (gridDefinition != null)
            {
                if (gridDefinition.GenericBEGridActions != null)
                {
                    genericBusinessEntityDetail.AvailableGridActionIds = new List<Guid>();
                    foreach (var genericBEGridAction in gridDefinition.GenericBEGridActions)
                    {
                        genericBusinessEntityDetail.AvailableGridActionIds.Add(genericBEGridAction.GenericBEGridActionId);
                    }
                }
                if (gridDefinition.GenericBEGridViews != null)
                {
                    genericBusinessEntityDetail.AvailableGridViewIds = new List<Guid>();
                    foreach (var genericBEGridView in gridDefinition.GenericBEGridViews)
                    {
                        genericBusinessEntityDetail.AvailableGridViewIds.Add(genericBEGridView.GenericBEViewDefinitionId);
                    }
                }
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
 